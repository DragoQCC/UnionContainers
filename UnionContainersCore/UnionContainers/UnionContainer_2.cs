using UnionContainers.Core.Common;
using UnionContainers.Core.Helpers;

namespace UnionContainers.Core.UnionContainers;

public sealed record UnionContainer<T1,T2> : UnionContainerBase<UnionContainer<T1,T2>> , IUnionContainer<UnionContainer<T1,T2>> , IResultTypeWrapper<T1,IResult1>, IResultTypeWrapper<T2,IResult2>
{
    // deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2)
    {
        value1 = default;
        value2 = default;

        switch (ValueState.Value)
        {
            case T1 t1:
                value1 = t1;
                break;
            case T2 t2:
                value2 = t2;
                break;
        }
    }
    
    
    public UnionContainer()
    {}
    public UnionContainer(T1 value) : base(value)
    {}
    public UnionContainer(T2 value) : base(value)
    {}

    public new static UnionContainer<T1, T2> Create() => new();
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1, T2>(T1? value) => new UnionContainer<T1,T2>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2>(T2? value) => new UnionContainer<T1,T2>().SetValue(value);
    public static implicit operator UnionContainer<T1, T2>(UnionContainer<T1> t1Container) => t1Container.TryConvertContainer<UnionContainer<T1>,UnionContainer<T1,T2>>();
    public static implicit operator UnionContainer<T1, T2>(UnionContainer<T2> t2Container) => t2Container.TryConvertContainer<UnionContainer<T2>,UnionContainer<T1,T2>>();
    
}