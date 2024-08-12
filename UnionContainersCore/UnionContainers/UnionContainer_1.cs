using UnionContainers.Core.Common;
using UnionContainers.Core.Helpers;

namespace UnionContainers.Core.UnionContainers;

public sealed record UnionContainer<T1> : UnionContainerBase<UnionContainer<T1>>, IUnionContainer<UnionContainer<T1>>, IResultTypeWrapper<T1, IResult1>
{
    /// <summary>
    /// Containers with a single generic type can try to extract the value directly <br/>
    /// Note: This method will return default has no value set <br/>
    /// </summary>
    /// <returns></returns>
    public new T1? TryGetValue()
    {
        try
        {
            return this.HasResult() ? (T1?)ValueState.Value : default;
        }
        catch (Exception e)
        {
            return default;
        }
    }
    
    public UnionContainer()
    {}
    
    public UnionContainer(T1 value) : base(value)
    {}
    
    //conversion operators & constructors & deconstruction
    public static implicit operator UnionContainer<T1>(T1? value) => new UnionContainer<T1>().SetValue(value);
    public new static UnionContainer<T1> Create() => new();
}