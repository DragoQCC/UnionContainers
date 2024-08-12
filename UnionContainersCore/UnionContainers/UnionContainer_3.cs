using UnionContainers.Core.Common;
using UnionContainers.Core.Helpers;

namespace UnionContainers.Core.UnionContainers;

public record UnionContainer<T1,T2,T3> : UnionContainerBase<UnionContainer<T1,T2,T3>>, IUnionContainer<UnionContainer<T1,T2,T3>>,
    IResultTypeWrapper<T1,IResult1>, IResultTypeWrapper<T2,IResult2>, IResultTypeWrapper<T3,IResult3>
{
    
    //deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3)
    {
        value1 = default;
        value2 = default;
        value3 = default;

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
    
    public new static UnionContainer<T1, T2, T3> Create() => new();
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1,T2,T3>(T1? value) => new UnionContainer<T1,T2,T3>().SetValue(value);
    public static implicit operator UnionContainer<T1,T2,T3>(T2? value) => new UnionContainer<T1,T2,T3>().SetValue(value);
    public static implicit operator UnionContainer<T1,T2,T3>(T3? value) => new UnionContainer<T1,T2,T3>().SetValue(value);
    public static implicit operator UnionContainer<T1,T2,T3>(UnionContainer<T1> t1Container) => t1Container.TryConvertContainer<UnionContainer<T1>,UnionContainer<T1,T2,T3>>();
    public static implicit operator UnionContainer<T1,T2,T3>(UnionContainer<T2> t2Container) => t2Container.TryConvertContainer<UnionContainer<T2>,UnionContainer<T1,T2,T3>>();
    public static implicit operator UnionContainer<T1,T2,T3>(UnionContainer<T3> t3Container) => t3Container.TryConvertContainer<UnionContainer<T3>,UnionContainer<T1,T2,T3>>();
    public static implicit operator UnionContainer<T1,T2,T3>(UnionContainer<T1,T2> t1t2Container) => t1t2Container.TryConvertContainer<UnionContainer<T1,T2>,UnionContainer<T1,T2,T3>>();
    public static implicit operator UnionContainer<T1,T2,T3>(UnionContainer<T1,T3> t1t3Container) => t1t3Container.TryConvertContainer<UnionContainer<T1,T3>,UnionContainer<T1,T2,T3>>();
    public static implicit operator UnionContainer<T1,T2,T3>(UnionContainer<T2,T3> t2t3Container) => t2t3Container.TryConvertContainer<UnionContainer<T2,T3>,UnionContainer<T1,T2,T3>>();
    
}