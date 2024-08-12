using UnionContainers.Core.Common;
using UnionContainers.Core.Helpers;
namespace UnionContainers.Core.UnionContainers;


public sealed record UnionContainer<T1, T2, T3, T4, T5> : UnionContainerBase<UnionContainer<T1,T2,T3,T4,T5>>, IUnionContainer<UnionContainer<T1,T2,T3,T4,T5>>,
    IResultTypeWrapper<T1,IResult1>, IResultTypeWrapper<T2,IResult2>, IResultTypeWrapper<T3,IResult3>, IResultTypeWrapper<T4,IResult4>, IResultTypeWrapper<T5,IResult5>
{
    //deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5)
    {
        value1 = default;
        value2 = default;
        value3 = default;
        value4 = default;
        value5 = default;

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
    
    public new static UnionContainer<T1,T2,T3,T4,T5> Create() => new();
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(T1? value) => new UnionContainer<T1, T2, T3, T4, T5>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(T2? value) => new UnionContainer<T1, T2, T3, T4, T5>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(T3? value) => new UnionContainer<T1, T2, T3, T4, T5>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(T4? value) => new UnionContainer<T1, T2, T3, T4, T5>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(T5? value) => new UnionContainer<T1, T2, T3, T4, T5>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(UnionContainer<T1> t1Container) => t1Container.TryConvertContainer(new UnionContainer<T1, T2, T3, T4, T5>());
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(UnionContainer<T2> t2Container) => t2Container.TryConvertContainer(new UnionContainer<T1, T2, T3, T4, T5>());
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(UnionContainer<T3> t3Container) => t3Container.TryConvertContainer(new UnionContainer<T1, T2, T3, T4, T5>());
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(UnionContainer<T4> t4Container) => t4Container.TryConvertContainer(new UnionContainer<T1, T2, T3, T4, T5>());
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5>(UnionContainer<T5> t5Container) => t5Container.TryConvertContainer(new UnionContainer<T1, T2, T3, T4, T5>());
}

