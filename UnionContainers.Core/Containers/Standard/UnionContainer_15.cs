/*using UnionContainers.Containers.Base;

namespace UnionContainers.Containers.Standard;

public sealed record UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> 
    : UnionContainerBase<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>>, IUnionContainer<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>>
{
    //deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6, out T7? value7, out T8? value8, out T9? value9, out T10? value10, out T11? value11, out T12? value12, out T13? value13, out T14? value14, out T15? value15)
    {
        value1 = default;
        value2 = default;
        value3 = default;
        value4 = default;
        value5 = default;
        value6 = default;
        value7 = default;
        value8 = default;
        value9 = default;
        value10 = default;
        value11 = default;
        value12 = default;
        value13 = default;
        value14 = default;
        value15 = default;

        switch (ValueState.Value)
        {
            case T1 t1:
                value1 = t1;
                break;
            case T2 t2:
                value2 = t2;
                break;
            case T3 t3:
                value3 = t3;
                break;
            case T4 t4:
                value4 = t4;
                break;
            case T5 t5:
                value5 = t5;
                break;
            case T6 t6:
                value6 = t6;
                break;
            case T7 t7:
                value7 = t7;
                break;
            case T8 t8:
                value8 = t8;
                break;
            case T9 t9:
                value9 = t9;
                break;
            case T10 t10:
                value10 = t10;
                break;
            case T11 t11:
                value11 = t11;
                break;
            case T12 t12:
                value12 = t12;
                break;
            case T13 t13:
                value13 = t13;
                break;
            case T14 t14:
                value14 = t14;
                break;
            case T15 t15:
                value15 = t15;
                break;
        }
    }
    
    public UnionContainer()
    {}
    public UnionContainer(T1 value) : base(value)
    {}
    public UnionContainer(T2 value) : base(value)
    {}
    public UnionContainer(T3 value) : base(value)
    {}
    public UnionContainer(T4 value) : base(value)
    {}
    public UnionContainer(T5 value) : base(value)
    {}
    public UnionContainer(T6 value) : base(value)
    {}
    public UnionContainer(T7 value) : base(value)
    {}
    public UnionContainer(T8 value) : base(value)
    {}
    public UnionContainer(T9 value) : base(value)
    {}
    public UnionContainer(T10 value) : base(value)
    {}
    public UnionContainer(T11 value) : base(value)
    {}
    public UnionContainer(T12 value) : base(value)
    {}
    public UnionContainer(T13 value) : base(value)
    {}
    public UnionContainer(T14 value) : base(value)
    {}
    public UnionContainer(T15 value) : base(value)
    {}
    
    public new static UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> Create() => new();
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T2? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T3? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T4? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T5? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T6? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T7? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T8? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T9? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T10? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T11? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T12? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T13? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T14? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T15? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>().SetValue(value);
    
}*/