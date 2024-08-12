using UnionContainers.Core.Common;

namespace UnionContainers.Core.UnionContainers;

public sealed record UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8> : UnionContainerBase<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8>>, IUnionContainer<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8>>,
    IResultTypeWrapper<T1,IResult1>, IResultTypeWrapper<T2,IResult2>, IResultTypeWrapper<T3,IResult3>, IResultTypeWrapper<T4,IResult4>, 
    IResultTypeWrapper<T5,IResult5>, IResultTypeWrapper<T6,IResult6>, IResultTypeWrapper<T7,IResult7>, IResultTypeWrapper<T8,IResult8>
{
    
    //deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6, out T7? value7, out T8? value8)
    {
        value1 = default;
        value2 = default;
        value3 = default;
        value4 = default;
        value5 = default;
        value6 = default;
        value7 = default;
        value8 = default;

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
    
    public new static UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8> Create() => new();
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T1? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T2? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T3? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T4? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T5? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T6? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T7? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>(T8? value) => new UnionContainer<T1, T2, T3, T4, T5, T6, T7, T8>().SetValue(value);
}