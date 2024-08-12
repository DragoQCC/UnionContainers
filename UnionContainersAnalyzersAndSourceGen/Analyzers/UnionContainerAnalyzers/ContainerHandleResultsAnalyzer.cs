using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnionContainersAnalyzersAndSourceGen.Analyzers.UnionContainerAnalyzers;

/*[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HandleResultAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "UNCT007";
    private const string Category = "Usage";
    private static readonly LocalizableString Title = "Invalid TryHandleResult usage";
    private static readonly LocalizableString MessageFormat = "The number of actions passed to TryHandleResult does not match the number of generic type parameters of the UnionContainer, this may result in un matched types not being handled.";
    private static readonly LocalizableString Description = "Detects when TryHandleResult is invoked with an incorrect number of actions.";
    
    private const string DiagnosticId2 = "UNCT009";
    private const string Category2 = "Usage";
    private static readonly LocalizableString Title2 = "Invalid TryHandleResult usage";
    private static readonly LocalizableString MessageFormat2 = "The Types {0} passed to TryHandleResult do not match the generic type parameters of the UnionContainer {1}", PassedTypes, ContainerTypes;
    private static readonly LocalizableString Description2 = "Detects when TryHandleResult is invoked with incorrect types";
    
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
    private static readonly DiagnosticDescriptor Rule2 = new DiagnosticDescriptor(DiagnosticId2, Title2, MessageFormat2, Category2, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description2);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule, Rule2];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethodInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeMethodInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;
        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccess || memberAccess.Name.Identifier.Text != "TryHandleResult")
        {
            return;
        }
        var containerType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;
        var genericArguments = containerType?.TypeArguments;

        if (genericArguments == null || !genericArguments.Value.Any())
        {
            return;
        }
    
        var actionArguments = invocationExpressionSyntax.ArgumentList.Arguments;

        if (actionArguments.Count == 0)
        {
            return;
        }
    
        int matchingActionCount = 0;
        foreach (var argument in actionArguments)
        {
            if (argument.Expression is not LambdaExpressionSyntax lambda)
            {
                continue;
            }

            IParameterSymbol parameterSymbol = null;

            if (lambda is SimpleLambdaExpressionSyntax simpleLambda)
            {
                parameterSymbol = context.SemanticModel.GetDeclaredSymbol(simpleLambda.Parameter) as IParameterSymbol;
            }
            else if (lambda is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                parameterSymbol = context.SemanticModel.GetDeclaredSymbol(parenthesizedLambda.ParameterList.Parameters.FirstOrDefault()) as IParameterSymbol;
            }

            if (parameterSymbol == null)
            {
                continue;
            }

            var parameterType = parameterSymbol.Type;
            if (genericArguments.Value.Any(genericArgument => IsAssignableTo(parameterType, genericArgument)))
            {
                matchingActionCount++;
            }
            else if (!IsExceptionType(parameterType))
            {
                var typeMismatchDiag = Diagnostic.Create(Rule2, lambda.GetLocation(), parameterType.ToString(), string.Join(", ", genericArguments.Value.Select(g => g.ToString())));
                context.ReportDiagnostic(typeMismatchDiag);
            }
        }

        if (genericArguments.Value.Length > matchingActionCount)
        {
            var diagnostic = Diagnostic.Create(Rule, invocationExpressionSyntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
    
    private bool IsAssignableTo(ITypeSymbol? typeSymbol, ITypeSymbol? targetType)
    {
        return typeSymbol != null && targetType != null &&
               (SymbolEqualityComparer.Default.Equals(typeSymbol, targetType) ||
                typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, targetType)) ||
                IsAssignableTo(typeSymbol.BaseType, targetType));
    }
    
    private bool IsExceptionType(ITypeSymbol typeSymbol)
    {
        return typeSymbol != null && (typeSymbol.Name == "Exception" || (typeSymbol.BaseType != null && IsExceptionType(typeSymbol.BaseType)));
    }
}*/

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HandleResultAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "UNCT007";
    private const string Category = "Usage";
    private static readonly LocalizableString Title = "Potential unhandled types in TryHandleResult chain";
    private static readonly LocalizableString MessageFormat = "The TryHandleResult chain may not handle all types from the UnionContainer. Consider adding handlers for: {0}";
    private static readonly LocalizableString Description = "Detects when a chain of TryHandleResult calls might not handle all types from the original UnionContainer.";
    
    private const string DiagnosticId2 = "UNCT009";
    private const string Category2 = "Usage";
    private static readonly LocalizableString Title2 = "Invalid TryHandleResult usage";
    private static readonly LocalizableString MessageFormat2 = "The Type {0} passed to TryHandleResult does not match any of the generic type parameters of the UnionContainer {1}";
    private static readonly LocalizableString Description2 = "Detects when TryHandleResult is invoked with incorrect types";
    
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);
    private static readonly DiagnosticDescriptor Rule2 = new DiagnosticDescriptor(DiagnosticId2, Title2, MessageFormat2, Category2, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description2);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule, Rule2);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeTryHandleResultChain, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeTryHandleResultChain(SyntaxNodeAnalysisContext context)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;
        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccess || memberAccess.Name.Identifier.Text != "TryHandleResult")
        {
            return;
        }

        var (containerType, chainStart) = FindOriginalContainerTypeAndChainStart(context, invocationExpressionSyntax);
        if (containerType == null || !containerType.TypeArguments.Any())
        {
            return;
        }

        var handledTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        AnalyzeTryHandleResultChainRecursive(context, chainStart, containerType, handledTypes);

        var unhandledTypes = containerType.TypeArguments.Where(t => !handledTypes.Contains(t)).ToList();
        if (unhandledTypes.Any())
        {
            var diagnostic = Diagnostic.Create(Rule, chainStart.GetLocation(), string.Join(", ", unhandledTypes.Select(t => t.Name)));
            context.ReportDiagnostic(diagnostic);
        }
    }

    private void AnalyzeTryHandleResultChainRecursive(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, INamedTypeSymbol containerType, HashSet<ITypeSymbol> handledTypes)
    {
        foreach (var argument in invocation.ArgumentList.Arguments)
        {
            if (argument.Expression is not LambdaExpressionSyntax lambda)
            {
                continue;
            }

            var parameterSymbol = GetLambdaParameterSymbol(context, lambda);
            if (parameterSymbol == null)
            {
                continue;
            }

            var parameterType = parameterSymbol.Type;
            var matchedType = containerType.TypeArguments.FirstOrDefault(t => IsAssignableTo(parameterType, t));
            if (matchedType != null)
            {
                handledTypes.Add(matchedType);
            }
            else if (!IsExceptionType(parameterType))
            {
                var typeMismatchDiag = Diagnostic.Create(Rule2, lambda.GetLocation(), parameterType.Name, string.Join(", ", containerType.TypeArguments.Select(t => t.Name)));
                context.ReportDiagnostic(typeMismatchDiag);
            }
        }

        // Check for next TryHandleResult call in the chain
        if (invocation.Parent is MemberAccessExpressionSyntax nextMemberAccess &&
            nextMemberAccess.Parent is InvocationExpressionSyntax nextInvocation &&
            nextMemberAccess.Name.Identifier.Text == "TryHandleResult")
        {
            AnalyzeTryHandleResultChainRecursive(context, nextInvocation, containerType, handledTypes);
        }
    }

    private (INamedTypeSymbol, InvocationExpressionSyntax) FindOriginalContainerTypeAndChainStart(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
    {
        var currentNode = invocation;
        INamedTypeSymbol containerType = null;
        InvocationExpressionSyntax chainStart = invocation;

        while (true)
        {
            if (currentNode.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var typeInfo = context.SemanticModel.GetTypeInfo(memberAccess.Expression);
                if (typeInfo.Type is INamedTypeSymbol namedType && namedType.Name == "UnionContainer")
                {
                    containerType = namedType;
                    break;
                }

                if (memberAccess.Expression is InvocationExpressionSyntax parentInvocation)
                {
                    chainStart = currentNode;
                    currentNode = parentInvocation;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return (containerType, chainStart);
    }

    private IParameterSymbol GetLambdaParameterSymbol(SyntaxNodeAnalysisContext context, LambdaExpressionSyntax lambda)
    {
        if (lambda is SimpleLambdaExpressionSyntax simpleLambda)
        {
            return context.SemanticModel.GetDeclaredSymbol(simpleLambda.Parameter) as IParameterSymbol;
        }
        else if (lambda is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
        {
            return context.SemanticModel.GetDeclaredSymbol(parenthesizedLambda.ParameterList.Parameters.FirstOrDefault()) as IParameterSymbol;
        }
        return null;
    }

    private bool IsAssignableTo(ITypeSymbol typeSymbol, ITypeSymbol targetType)
    {
        return typeSymbol != null && targetType != null &&
               (SymbolEqualityComparer.Default.Equals(typeSymbol, targetType) ||
                typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, targetType)) ||
                IsAssignableTo(typeSymbol.BaseType, targetType));
    }

    private bool IsExceptionType(ITypeSymbol typeSymbol)
    {
        return typeSymbol != null && (typeSymbol.Name == "Exception" || 
               (typeSymbol.BaseType != null && IsExceptionType(typeSymbol.BaseType)));
    }
}