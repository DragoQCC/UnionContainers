using System.Collections.Concurrent;
using System.Reflection;

namespace UnionContainers.Core.Helpers;

internal static class TypeExtensions
{
    internal static readonly ConcurrentDictionary<MethodBase, IReadOnlyList<Type>> ParameterMap = new ConcurrentDictionary<MethodBase, IReadOnlyList<Type>>();
    
}

public static class MethodBaseExtensions
{
    /// <summary>
    /// Gets the types for a method's parameters.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static IReadOnlyList<Type> GetParameterTypes(this MethodBase method) => 
        TypeExtensions.ParameterMap.GetOrAdd(method, c => c.GetParameters().Select(p => p.ParameterType).ToArray());
    
}