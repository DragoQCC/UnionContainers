using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnionContainersAnalyzersAndSourceGen.Analyzers.UnionContainerAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TryGetValueAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "UNCT005";
    private const string Category = "Usage";
    private static readonly LocalizableString Title = "Incompatible type assignment from TryGetValue";
    private static readonly LocalizableString MessageFormat = "The type '{0}' may not be compatible with the value returned by TryGetValue. Consider using pattern matching or type checking.";
    private static readonly LocalizableString Description = "Warns about potential type mismatches when assigning the result of TryGetValue to a variable.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
    

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;
        var memberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;

        if (memberAccessExpression == null || memberAccessExpression.Name.ToString() != "TryGetValue")
        {
            return;
        }

        var containerType = context.SemanticModel.GetTypeInfo(memberAccessExpression.Expression).Type as INamedTypeSymbol;
        
        var variableDeclaration = GetParentVariableDeclaration(invocationExpression);

        if (variableDeclaration == null || containerType == null || !containerType.IsGenericType || !containerType.Name.StartsWith("UnionContainer"))
        {
            return;
        }
        
        ImmutableArray<ITypeSymbol?> genericTypes = containerType.TypeArguments;
        var assignedType = context.SemanticModel.GetTypeInfo(variableDeclaration.Type).Type;
        
        if(assignedType == null || genericTypes.Length == 0)
        {
            return;
        }
        
        //if its an exact type match we can just return here
        if (genericTypes.Contains(assignedType))
        {
            return;
        }
        //check for derived types and implicit conversions
        if (IsAssignableToAny(assignedType, genericTypes, context.SemanticModel.Compilation))
        {
            return;
        }
        var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), assignedType?.ToDisplayString());
        context.ReportDiagnostic(diagnostic);
    }
    
    private VariableDeclarationSyntax? GetParentVariableDeclaration(SyntaxNode node)
    {
        bool shortCircuit = false;
        int counter = 0;
        while (shortCircuit is false)
        {
            if (node.Parent is VariableDeclarationSyntax variableDeclaration)
            {
                shortCircuit = true;
                return variableDeclaration;
            }
            if (node.Parent != null)
            {
                counter++;
                node = node.Parent;
            }
            else
            {
                shortCircuit = true;
            }
            if (counter > 10)
            {
                shortCircuit = true;
            }
        }
        return null;
    }
    
    private bool IsAssignableToAny(ITypeSymbol? assignedType, ImmutableArray<ITypeSymbol?> genericTypes, Compilation compilation)
    {
        foreach (var genericType in genericTypes)
        {
            if (genericType.IsAssignableTo(assignedType, compilation))
            {
                return true;
            }
        }
        return false;
    }
}

internal static class TypeSymbolExt
{
    internal static bool IsAssignableTo(this ITypeSymbol inspectedType, ITypeSymbol? destinationType, Compilation compilation)
    {
        return inspectedType.Equals(destinationType) || compilation.ClassifyConversion(inspectedType, destinationType).IsImplicit;
    }
}