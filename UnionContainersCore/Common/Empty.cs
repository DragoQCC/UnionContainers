using System.Runtime.InteropServices;

namespace UnionContainers.Core.Common;

/// <summary>
/// A type that represents an empty value <br/>
/// Can be used in place of null or void <br/>
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential, Size = 1)]
public record struct Empty
{
    public static Empty Nothing { get; } = new Empty();
    
    public static Empty Return() => Nothing;
}


