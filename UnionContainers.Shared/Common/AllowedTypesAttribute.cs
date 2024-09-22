using System;

namespace UnionContainers.Shared.Common;


/// <summary>
/// When applied this attribute will limit the types that can be used for a field, property, parameter, or return value <br/>
/// Example:
/// <code>
/// //This limits the types that can be assigned to the property to Employee and Manager
/// //uses a source code analyzer to enforce the type limitations during design/compile time <br/>
/// [AllowedTypes{Employee,Manager}]
/// public dynamic TestProperty { get; set;}
/// </code>
/// Can also be applied to generics to limit the types that can be used for the generic type <br/>
/// <code>
/// public class UnSignedNumbersOnly{[AllowedTypes{byte, ushort, uint, ulong, nuint}] T}
/// {
/// // code
/// }
/// </code>
/// </summary>
/// <param name="allowDerivedTypes"></param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter)]
public abstract class AllowedTypesAttribute(bool allowDerivedTypes = false) : Attribute
{
    //For the moment this is disabled
    //public bool AllowDerivedTypes { get; set; } = allowDerivedTypes;
}

public class AllowedTypesAttribute<T1> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}

public class AllowedTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : AllowedTypesAttribute
{
    public AllowedTypesAttribute(bool allowDerivedTypes = false) : base(allowDerivedTypes)
    { }
}