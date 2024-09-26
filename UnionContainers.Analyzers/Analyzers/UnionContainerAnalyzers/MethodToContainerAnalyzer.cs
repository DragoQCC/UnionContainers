using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnionContainersAnalyzersAndSourceGen.Analyzers.UnionContainerAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MethodToContainerAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "UNCT010";
    private const string Category = "Type Safety";

    private static readonly LocalizableString Title = "Invalid type usage";

    private static readonly LocalizableString MessageFormat =
            "The return type '{0}' is not allowed Return types for MethodToContainer must be one of the specified types: {1} or a derived type",
        ReturnType,
        allowedTypes;

    private static readonly LocalizableString Description = "Ensures that the used type is one of the allowed types specified by the AllowedTypesAttribute.";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [ Rule ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;
        // Check if it's a MethodToContainer call
        bool isMethodToContainer = false;
        INamedTypeSymbol? containerType = null;
        ImmutableArray<ITypeSymbol> containerGenerics = [ ];

        if (invocationExpressionSyntax.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            if (memberAccess.Name.Identifier.Text == "MethodToContainer")
            {
                isMethodToContainer = true;
                containerType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;
                containerGenerics = containerType?.TypeArguments ?? ImmutableArray<ITypeSymbol>.Empty;
            }
        }
        else if (invocationExpressionSyntax.Expression is GenericNameSyntax genericName)
        {
            if (genericName.Identifier.Text == "MethodToContainer")
            {
                isMethodToContainer = true;
                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax);
                if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                {
                    containerType = methodSymbol.ReturnType as INamedTypeSymbol;
                    containerGenerics = containerType?.TypeArguments ?? ImmutableArray<ITypeSymbol>.Empty;
                }
            }
        }

        if (!isMethodToContainer || containerType == null)
        {
            return;
        }

        var targetGenerics = ImmutableArray<INamedTypeSymbol>.Empty;

        if (containerGenerics.Any())
        {
            foreach (ITypeSymbol? generic in containerGenerics)
            {
                //string genericText = generic.ToString();
                if (generic is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Name == "UnionContainer")
                {
                    //string nestedGenerics = genericText.Split('<', '>')[1];
                    ImmutableArray<ITypeSymbol> nestedGenerics = namedTypeSymbol.TypeArguments;
                    //LogMessage($"Nested generics found in container type: {string.Join(", ", nestedGenerics)}");

                    foreach (ITypeSymbol? nestedGeneric in nestedGenerics)
                    {
                        if (nestedGeneric is INamedTypeSymbol nestedNamedType)
                        {
                            targetGenerics = targetGenerics.Add(nestedNamedType);
                        }
                        else if (nestedGeneric is ITypeSymbol typeSymbol)
                        {
                            //LogMessage($"Nested generic is not a named type symbol, but a type symbol: {typeSymbol.Name}");
                        }
                    }
                }
            }
        }

        if (targetGenerics.IsEmpty)
        {
            return;
        }

        // Find the lambda expression
        LambdaExpressionSyntax? lambdaExpression = invocationExpressionSyntax.DescendantNodes().OfType<LambdaExpressionSyntax>().FirstOrDefault();
        if (lambdaExpression == null)
        {
            return;
        }

        // Analyze return statements within the lambda
        IEnumerable<ReturnStatementSyntax> returnStatements = lambdaExpression.DescendantNodes().OfType<ReturnStatementSyntax>();

        foreach (ReturnStatementSyntax? returnStatement in returnStatements)
        {
            if (returnStatement.Expression == null)
            {
                continue;
            }

            ITypeSymbol? returnType = context.SemanticModel.GetTypeInfo(returnStatement.Expression).Type;

            if (targetGenerics.All(genericArgument => !IsAssignableTo(returnType, genericArgument)))
            {
                var typeMismatchDiag = Diagnostic.Create(Rule, returnStatement.GetLocation(), returnType?.ToString(), string.Join(", ", targetGenerics.Select(g => g.ToString())));
                context.ReportDiagnostic(typeMismatchDiag);
            }
        }
    }

    private bool IsAssignableTo
        (ITypeSymbol? typeSymbol, ITypeSymbol? targetType)
        => typeSymbol != null
            && targetType != null
            && (SymbolEqualityComparer.Default.Equals
                    (typeSymbol, targetType)
                || typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, targetType))
                || IsAssignableTo(typeSymbol.BaseType, targetType));
}