using Microsoft.CodeAnalysis.Diagnostics;

namespace UnionContainersAnalyzersAndSourceGen.Helpers;

public static class Pathing
{
    public static string GetProjectPath(this SyntaxNodeAnalysisContext context)
    {
        return context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.ProjectDir", out var result) ? result : null;
    }
}