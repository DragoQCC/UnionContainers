using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnionContainersAnalyzersAndSourceGen.Analyzers.UnionContainerAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ContainerConvertorAnalyzer : DiagnosticAnalyzer
{
    private const string InvalidContainerConversionDiagnosticId = "UNCT004";
    private const string Category = "Usage";
    private static readonly string InvalidContainerConversionTitle = "Invalid Container Conversion";
    private static readonly string InvalidContainerConversionMessageFormat = "The source container type '{0}' cannot be converted to the target container type '{1}'";
    private static readonly string InvalidContainerConversionDescription = "The target container type must contain all the generic types of the source container type.";

    private static readonly DiagnosticDescriptor ContainerConversionRule = new
    (
        InvalidContainerConversionDiagnosticId, InvalidContainerConversionTitle, InvalidContainerConversionMessageFormat, Category, DiagnosticSeverity.Error, true,
        InvalidContainerConversionDescription
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ContainerConversionRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
    {
        var invocationExpr = (InvocationExpressionSyntax)context.Node;
        var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;

        if (memberAccessExpr?.Name.ToString() != "TryConvertContainer")
        {
            return;
        }

        ITypeSymbol? sourceContainerType = ModelExtensions.GetTypeInfo(context.SemanticModel, memberAccessExpr.Expression).Type;

        if (invocationExpr.ArgumentList.Arguments[0].Expression is not TypeOfExpressionSyntax typeOfExpr)
        {
            return;
        }

        ITypeSymbol? targetContainerType = ModelExtensions.GetTypeInfo(context.SemanticModel, typeOfExpr.Type).Type;

        if (sourceContainerType is not INamedTypeSymbol sourceNamedType || targetContainerType is not INamedTypeSymbol targetNamedType)
        {
            return;
        }

        ImmutableArray<ITypeSymbol> sourceGenerics = sourceNamedType.TypeArguments;
        ImmutableArray<ITypeSymbol> targetGenerics = targetNamedType.TypeArguments;

        if (sourceGenerics.All(x => targetGenerics.Contains(x)))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(ContainerConversionRule, invocationExpr.GetLocation(), sourceNamedType.ToDisplayString(), targetNamedType.ToDisplayString());
        context.ReportDiagnostic(diagnostic);
    }
}