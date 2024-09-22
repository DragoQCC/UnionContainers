using System;
using System.Collections.Generic;

namespace UnionContainers.Shared.Common;

/// <summary>
/// Opposite of the <see cref="AllowedTypesAttribute"/> this attribute will deny the types that can be used for a field, property, parameter, or return value <br/>
/// This is useful if you want a generic or dynamic type to be any type except a few specific types <br/>
/// Example:
/// <code>
/// // Allows any type except for long and ulong
///public T Add{[DeniedTypes{long,ulong}]T}(T number1, T number2) where T : struct, IAdditionOperators{T, T, T}
///{
///  return number1 + number2;
///}
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter)]
public abstract class DeniedTypesAttribute() : Attribute
{ }

public class DeniedTypesAttribute<T1> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> : DeniedTypesAttribute
{ }

public class DeniedTypesAttribute<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> : DeniedTypesAttribute
{ }

public class DeniedTypeExtAttribute(List<Type> deniedTypes) : DeniedTypesAttribute
{
    public List<Type> DeniedTypes { get; private set; } = deniedTypes;
}