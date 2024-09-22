using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using UnionContainers.Shared.Common;

namespace UnionContainersAnalyzersAndSourceGen.Analyzers.AttributeTypeAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AllowedTypesAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "UNCT006";
    private const string Category = "Type Safety";

    private static readonly LocalizableString Title = "Invalid type usage";
    private static readonly LocalizableString MessageFormat = "The type '{0}' is not allowed. Allowed types are: {1}.";
    private static readonly LocalizableString Description = "Ensures that the used type is one of the allowed types specified by the AllowedTypesAttribute.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeGenericClassUse, SyntaxKind.GenericName);
        context.RegisterSyntaxNodeAction(AnalyzeMethodInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
    }
    
    private void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignmentExpression = (AssignmentExpressionSyntax)context.Node;
        var leftSymbol = context.SemanticModel.GetSymbolInfo(assignmentExpression.Left).Symbol;
        AnalyzeSymbol(context, leftSymbol, assignmentExpression.Right);
    }

    private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
        AnalyzeSymbol(context, propertySymbol, propertyDeclaration.Initializer?.Value);
    }

    private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;
        var variableDeclaration = fieldDeclaration.Declaration.Variables.First();
        var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variableDeclaration);
        AnalyzeSymbol(context, fieldSymbol, variableDeclaration.Initializer?.Value);
    }

    private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
        
        if (methodSymbol == null)
        {
            return;
        }
        // Analyze return type
        var returnStatements = methodDeclaration.DescendantNodes().OfType<ReturnStatementSyntax>();
        foreach (var returnStatement in returnStatements)
        {
            if (returnStatement.Expression == null)
            {
                continue;
            }
            //make sure the return statement is not within a local function, or lambda
            if (returnStatement.Ancestors().Any(a => a is LocalFunctionStatementSyntax || a is ParenthesizedLambdaExpressionSyntax))
            {
                continue;
            }
            var returnAttributes = methodSymbol.GetReturnTypeAttributes();
            var allowedTypesAttribute = returnAttributes.FirstOrDefault(attr => attr.AttributeClass?.BaseType?.Name == nameof(AllowedTypesAttribute));

            if (allowedTypesAttribute == null)
            {
                continue;
            }
            var allowedTypes = allowedTypesAttribute.AttributeClass?.TypeArguments.ToList();
            var returnType = context.SemanticModel.GetTypeInfo(returnStatement.Expression).Type;

            if(allowedTypes == null || allowedTypes.Count == 0 || returnType == null)
            {
                continue;
            }
            if (allowedTypes.Contains(returnType))
            {
                continue;
            }
            //if return type is the nullable version of the allowed type continue
            if (returnType.BaseType == allowedTypes?.First() && returnType.NullableAnnotation == NullableAnnotation.Annotated)
            {
                continue;
            }
            var allowedTypesString = string.Join(", ", allowedTypes?.Select(t => t.ToDisplayString()) ?? Array.Empty<string>());
            var diagnostic = Diagnostic.Create(Rule, returnStatement.Expression.GetLocation(), returnType.ToDisplayString(), allowedTypesString);
            context.ReportDiagnostic(diagnostic);
        }
        // Analyze parameter types
        var parameterStatements = methodDeclaration.DescendantNodes().OfType<ParameterSyntax>();
        foreach (var parameter in parameterStatements)
        {
            var parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parameter);
            ExpressionSyntax? parameterExp = parameter.Default?.Value;
            AnalyzeSymbol(context, parameterSymbol, parameterExp);
        }
    }
    
    private void AnalyzeMethodInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        // Check if the containing type is a static class
        //this is meant to help inspect extension methods which require a bit of extra work to analyze
        var containingType = methodSymbol.ContainingType;
        if (containingType is { IsStatic: true })
        {
             /*If this is a reduced extension method, get the original method symbol
             example of a reduced form is the extension method "Namespace.ExtensionClass.Foo<T>(T obj)"
             the original method would look more like "Namespace.ExtendedClass.Foo<ConcreteType>(ConcreteType obj)"
             in this case, we need the syntax of the original method to get the type arguments and the reduced method to get the proper method signature*/
            var reducedSymbolMethod = methodSymbol.ReducedFrom;
            if (reducedSymbolMethod != null)
            {
                var methodTypeParameters = methodSymbol.TypeParameters;
                var methodTypeArguments = methodSymbol.TypeArguments;

                for (int i = 0; i < methodTypeParameters.Length; i++)
                {
                    var typeParameterSymbol = methodTypeParameters[i];
                    var typeArgumentSymbol = methodTypeArguments[i];
                    var reducedAttributes = reducedSymbolMethod.TypeParameters[i].GetAttributes();
                    var reducedAllowedTypesAttribute = reducedAttributes.FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(AllowedTypesAttribute));
                    if (reducedAllowedTypesAttribute == null)
                    {
                        continue;
                    }
                    var allowedTypes = reducedAllowedTypesAttribute.AttributeClass?.TypeArguments.ToList();
                    var allowedTypesString = string.Join(", ", allowedTypes?.Select(t => t.ToDisplayString()) ?? Array.Empty<string>());
                    var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), typeArgumentSymbol.ToDisplayString(), allowedTypesString);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
        
        //Checks for denied types in the type arguments (generics) of the method
        for (int i = 0; i < methodSymbol.TypeArguments.Length; i++)
        {
            var typeArgumentSymbol = methodSymbol.TypeArguments[i];
            var typeParameterSymbol = methodSymbol.TypeParameters[i];
            var attributes = typeParameterSymbol.GetAttributes();
            var allowedTypesAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(AllowedTypesAttribute));

            if (allowedTypesAttribute == null)
            {
                continue;
            }
            var allowedTypes = allowedTypesAttribute.AttributeClass?.TypeArguments.ToList();
            if (allowedTypes == null)
            {
                continue;
            }
            if (IsAllowedType(typeArgumentSymbol, allowedTypes))
            {
                continue;
            }
            var allowedTypesString = string.Join(", ", allowedTypes.Select(t => t.ToDisplayString()));
            var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), typeArgumentSymbol.ToDisplayString(), allowedTypesString);
            context.ReportDiagnostic(diagnostic);
        }

        //Checks for denied types in the method parameters (arguments)
        for (int i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameterSymbol = methodSymbol.Parameters[i];
            if(i < invocationExpression.ArgumentList.Arguments.Count)
            {
                var argumentExpression = invocationExpression.ArgumentList.Arguments[i].Expression;
                AnalyzeSymbol(context, parameterSymbol, argumentExpression);
            }
        }
    }
    
    private void AnalyzeGenericClassUse(SyntaxNodeAnalysisContext context)
    {
        var genericName = (GenericNameSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(genericName).Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }
        for (int i = 0; i < typeSymbol.TypeArguments.Length; i++)
        {
            //if symbol is a generic T skip it
            if (typeSymbol.TypeArguments[i].Name == "T")
            {
                continue;
            }
            if(i < typeSymbol.TypeParameters.Length)
            {
                var typeParameterSymbol = typeSymbol.TypeParameters[i];
                var typeArgumentExpression = genericName.TypeArgumentList.Arguments[i];
                AnalyzeSymbol(context, typeParameterSymbol, typeArgumentExpression);
            }
        }
    }

    
    private void AnalyzeSymbol(SyntaxNodeAnalysisContext context, ISymbol? symbol, ExpressionSyntax? expressionSyntax)
    {
        if (symbol == null || expressionSyntax == null)
        {
            return;
        }
        var attributes = symbol.GetAttributes();
        var allowedTypesAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(AllowedTypesAttribute));

        var allowedTypes = allowedTypesAttribute?.AttributeClass?.TypeArguments.ToList();
        
        if (allowedTypes == null)
        {
            return;
        }
        
        var usedType = context.SemanticModel.GetTypeInfo(expressionSyntax).Type;

        if (usedType != null && IsAllowedType(usedType, allowedTypes))
        {
            return;
        }
        var allowedTypesString = string.Join(", ", allowedTypes.Select(t => t.ToDisplayString()));
        var diagnostic = Diagnostic.Create(Rule, expressionSyntax.GetLocation(), usedType?.ToDisplayString(), allowedTypesString);
        context.ReportDiagnostic(diagnostic);
    }
    
    private bool IsAllowedType(ITypeSymbol usedType, List<ITypeSymbol> allowedTypes)
    {
        return allowedTypes.Contains(usedType);
    }
}