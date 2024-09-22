using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnionContainersAnalyzersAndSourceGen.Analyzers.UnionContainerAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionContainerCreateValidationAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "UNCT001";
    private const string Category = "Usage";

    private static readonly LocalizableString Title = "Invalid argument type for UnionContainer creation or value setting";
    private static readonly LocalizableString MessageFormat = "The type '{0}' is not a valid argument type for {1}. The argument must be one of the specified generic types or an UnionContainer of the same type.";
    private static readonly LocalizableString Description = "Detects when CreateWithValue or SetValueState is given an invalid type.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
    }

    private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;
        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax || memberAccessExpressionSyntax.Name.Identifier.Text != "SetValue")
        {
            return;
        }
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax);

        if (symbolInfo.Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }
        var UnionContainerType = GetUnionContainerType(context.SemanticModel, invocationExpressionSyntax);

        if (UnionContainerType == null)
        {
            return;
        }
        var argumentType = context.SemanticModel.GetTypeInfo(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression).Type;

        if (IsValidArgumentType(argumentType, UnionContainerType))
        {
            return;
        }
        var methodName = memberAccessExpressionSyntax.Name.Identifier.Text;
        var diagnostic = Diagnostic.Create(Rule, invocationExpressionSyntax.GetLocation(), argumentType?.ToDisplayString(), $"{UnionContainerType.ToDisplayString()}.{methodName}");

        context.ReportDiagnostic(diagnostic);
    }

    private void AnalyzeAssignmentExpression(SyntaxNodeAnalysisContext context)
    {
        var assignmentExpressionSyntax = (AssignmentExpressionSyntax)context.Node;

        if (assignmentExpressionSyntax.Right is not InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return;
        }

        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax || memberAccessExpressionSyntax.Name.Identifier.Text != "CreateWithValue")
        {
            return;
        }
        var leftSymbol = context.SemanticModel.GetSymbolInfo(assignmentExpressionSyntax.Left).Symbol;

        if (leftSymbol?.GetTypeSymbol() is not INamedTypeSymbol UnionContainerType || !UnionContainerType.Name.StartsWith("UnionContainer"))
        {
            return;
        }
        var argumentType = context.SemanticModel.GetTypeInfo(invocationExpressionSyntax.ArgumentList.Arguments[0].Expression).Type;

        if (IsValidArgumentType(argumentType, UnionContainerType))
        {
            return;
        }
        var methodName = memberAccessExpressionSyntax.Name.Identifier.Text;
        var diagnostic = Diagnostic.Create(Rule, invocationExpressionSyntax.GetLocation(), argumentType.ToDisplayString(), $"{UnionContainerType.ToDisplayString()}.{methodName}");

        context.ReportDiagnostic(diagnostic);
    }
    
    private INamedTypeSymbol? GetUnionContainerType(SemanticModel semanticModel, InvocationExpressionSyntax invocationExpressionSyntax)
    {
        if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return null;
        }

        if (semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression).Type is INamedTypeSymbol receiverType && receiverType.Name.StartsWith("UnionContainer"))
        {
            return receiverType;
        }

        return null;
    }
    
    private bool IsValidArgumentType(ITypeSymbol? argumentType, INamedTypeSymbol? UnionContainerType)
    {
        if (argumentType == null || UnionContainerType == null)
        {
            return false;
        }
        if (argumentType.Name.StartsWith("UnionContainer") && SymbolEqualityComparer.Default.Equals(argumentType, UnionContainerType))
        {
            // The argument is an UnionContainer of the same type, so it's valid
            return true;
        }
        else if (UnionContainerType.TypeArguments.Any(t => IsAssignableTo(argumentType, t)))
        {
            // The argument is one of the generic type parameters or a derived type, so it's valid
            return true;
        }
        return false;
    }

    private bool IsAssignableTo(ITypeSymbol? typeSymbol, ITypeSymbol? targetType)
    {
        return typeSymbol != null && targetType != null &&
               (SymbolEqualityComparer.Default.Equals(typeSymbol, targetType) ||
                typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, targetType)) ||
                IsAssignableTo(typeSymbol.BaseType, targetType));
    }
}

public static class SymbolExtensions
{
    public static ITypeSymbol? GetTypeSymbol(this ISymbol symbol)
    {
        return symbol switch
        {
            ILocalSymbol localSymbol => localSymbol.Type,
            IParameterSymbol parameterSymbol => parameterSymbol.Type,
            IPropertySymbol propertySymbol => propertySymbol.Type,
            IFieldSymbol fieldSymbol => fieldSymbol.Type,
            _ => null
        };
    }
}