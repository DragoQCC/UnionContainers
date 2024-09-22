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
public class DeniedTypesAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "UNCT008";
    private const string Category = "Type Safety";

    private static readonly LocalizableString Title = "UNCT008: Invalid type usage";
    private static readonly LocalizableString MessageFormat = "The type '{0}' is not allowed";
    private static readonly LocalizableString Description = "Ensures that the used type is not one of the denied types specified by the DeniedTypesAttribute.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
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
        if (leftSymbol != null) AnalyzeSymbol(context, leftSymbol, assignmentExpression.Right);
    }

    private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
        if (propertySymbol != null) AnalyzeSymbol(context, propertySymbol, propertyDeclaration.Initializer?.Value);
    }

    private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;
        var variableDeclaration = fieldDeclaration.Declaration.Variables.First();
        var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variableDeclaration);
        if (fieldSymbol != null) AnalyzeSymbol(context, fieldSymbol, variableDeclaration.Initializer?.Value);
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
            var deniedTypesAttribute = returnAttributes.FirstOrDefault(attr => attr.AttributeClass?.BaseType?.Name == nameof(DeniedTypesAttribute));

            if (deniedTypesAttribute == null)
            {
                continue;
            }
            var deniedTypes = deniedTypesAttribute.AttributeClass?.TypeArguments.ToList();
            var returnType = context.SemanticModel.GetTypeInfo(returnStatement.Expression).Type;
            
            if(deniedTypes == null || deniedTypes.Count == 0 || returnType == null)
            {
                continue;
            }

            if (!deniedTypes.Contains(returnType))
            {
                continue;
            }
            
            //if return type is the nullable version of the allowed type continue
            if ((returnType.BaseType is null || deniedTypes?.First() is null) || (returnType.BaseType == deniedTypes?.First() && returnType.NullableAnnotation == NullableAnnotation.Annotated))
            {
                continue;
            }
            
            var diagnostic = Diagnostic.Create(Rule, returnStatement.Expression.GetLocation(), "From return type: "+returnType?.ToDisplayString());
            context.ReportDiagnostic(diagnostic);
        }
        
        // Checks for denied types in the declared parameters
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
        if (containingType != null && containingType.IsStatic)
        {
             /*If this is a reduced extension method, get the original method symbol
             example of a reduced form is the extension method "Namespace.ExtensionClass.Foo<T>(T obj)"
             the original method would look more like "Namespace.ExtendedClass.Foo<ConcreteType>(ConcreteType obj)"
             in this case, we need the syntax of the original method to get the type arguments and the reduced method to get the proper method signature*/
            var reducedSymbolMethod = methodSymbol.ReducedFrom;
            if (reducedSymbolMethod != null)
            {
                var methodTypeParameters = methodSymbol.TypeParameters;
                ImmutableArray<ITypeSymbol> methodTypeArguments = methodSymbol.TypeArguments;

                for (int i = 0; i < methodTypeParameters.Length; i++)
                {
                    var typeParameterSymbol = methodTypeParameters[i];
                    var typeArgumentSymbol = methodTypeArguments[i];
                    if(reducedSymbolMethod.TypeParameters == null || reducedSymbolMethod.TypeParameters.Length == 0)
                    {
                        continue;
                    }
                    var reducedAttributes = reducedSymbolMethod.TypeParameters[i].GetAttributes();
                    var reducedDeniedTypesAttribute = reducedAttributes.FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(DeniedTypesAttribute));
                    if (reducedDeniedTypesAttribute == null)
                    {
                        continue;
                    }
                    List<ITypeSymbol>? deniedTypes = reducedDeniedTypesAttribute.AttributeClass?.TypeArguments.ToList();

                    if(deniedTypes == null || deniedTypes.Count == 0)
                    {
                        continue;
                    }
                    
                    if (!IsDeniedType(typeArgumentSymbol, deniedTypes!))
                    {
                        continue;
                    }
                    var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), typeArgumentSymbol?.ToDisplayString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
        
        //Checks for denied types in the type arguments (generics) of the method
        for (int i = 0; i < methodSymbol.TypeArguments.Length; i++)
        {
            
            var typeArgumentSymbol = methodSymbol.TypeArguments[i];
            if (methodSymbol.TypeParameters.Length <= i)
            {
                continue;
            }
            var typeParameterSymbol = methodSymbol.TypeParameters[i];
            var attributes = typeParameterSymbol.GetAttributes();
            var deniedTypesAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(DeniedTypesAttribute));

            if (deniedTypesAttribute == null)
            {
                continue;
            }
            List<ITypeSymbol>? deniedTypes = deniedTypesAttribute.AttributeClass?.TypeArguments.ToList();

            if(deniedTypes == null || deniedTypes.Count == 0)
            {
                continue;
            }
            
            if (!IsDeniedType(typeArgumentSymbol, deniedTypes!))
            {
                continue;
            }
            var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), typeArgumentSymbol.ToDisplayString());
            context.ReportDiagnostic(diagnostic);
        }

        //Checks for denied types in the method parameters (arguments)
        for (int i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var parameterSymbol = methodSymbol.Parameters[i];
            if(invocationExpression.ArgumentList.Arguments.Count <= i)
            {
                continue;
            }
            var argumentExpression = invocationExpression.ArgumentList.Arguments[i].Expression;
            AnalyzeSymbol(context, parameterSymbol, argumentExpression);
        }
    }
    
    private void AnalyzeGenericClassUse(SyntaxNodeAnalysisContext context)
    {
        var genericName = (GenericNameSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(genericName).Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }
        var attributes = typeSymbol.GetAttributes();
        var deniedTypesAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass?.BaseType?.Name == nameof(DeniedTypesAttribute));

        var deniedTypes = deniedTypesAttribute?.AttributeClass?.TypeArguments.ToList();

        if (deniedTypes is null || deniedTypes.Count == 0)
        {
            return;
        }
        for (int i = 0; i < typeSymbol.TypeArguments.Length; i++)
        {
            var typeArgumentSymbol = typeSymbol.TypeArguments[i];
            if( genericName.TypeArgumentList.Arguments.Count <= i)
            {
                continue;
            }
            var typeArgumentExpression = genericName.TypeArgumentList.Arguments[i];

            if (!IsDeniedType(typeArgumentSymbol, deniedTypes))
            {
                continue;
            }
            var diagnostic = Diagnostic.Create(Rule, typeArgumentExpression.GetLocation(), typeArgumentSymbol.ToDisplayString());
            context.ReportDiagnostic(diagnostic);
        }
    }

    
    private void AnalyzeSymbol(SyntaxNodeAnalysisContext context, ISymbol? symbol, ExpressionSyntax? expressionSyntax)
    {
        if (symbol == null || expressionSyntax == null)
        {
            return;
        }

        var attributes = symbol.GetAttributes();
        var deniedTypesAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(DeniedTypesAttribute));

        if (deniedTypesAttribute == null || deniedTypesAttribute.ConstructorArguments.Length == 0)
        {
            return;
        }

        var deniedTypes = deniedTypesAttribute.ConstructorArguments[0].Values.Select(v => v.Value as ITypeSymbol).ToList();
        var usedType = context.SemanticModel.GetTypeInfo(expressionSyntax).Type;

        if (usedType is null || deniedTypes.Count == 0 || !IsDeniedType(usedType, deniedTypes))
        {
            return;
        }
        var diagnostic = Diagnostic.Create(Rule, expressionSyntax.GetLocation(), "From analyze symbol: "+ usedType?.ToDisplayString());
        context.ReportDiagnostic(diagnostic);
    }

    private bool IsDeniedType(ITypeSymbol usedType, List<ITypeSymbol?> deniedTypes)
    {
        return deniedTypes.Contains(usedType);
    }
    
}