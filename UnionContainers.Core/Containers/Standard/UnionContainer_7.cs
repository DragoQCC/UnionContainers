﻿/*using UnionContainers.Containers.Base;

namespace UnionContainers.Containers.Standard;

public sealed record UnionContainer<T1, T2, T3, T4, T5, T6, T7> : UnionContainerBase<UnionContainer<T1,T2,T3,T4,T5,T6,T7>>, IUnionContainer<UnionContainer<T1,T2,T3,T4,T5,T6,T7>>
{


    //deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6, out T7? value7)
    {
        value1 = default;
        value2 = default;
        value3 = default;
        value4 = default;
        value5 = default;
        value6 = default;
        value7 = default;

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

    public new static UnionContainer<T1,T2,T3,T4,T5,T6,T7> Create() => new();

    // Implicit conversion operators
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T1? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T2? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T3? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T4? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T5? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T6? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T7? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7>().SetValue(value);
}*/

