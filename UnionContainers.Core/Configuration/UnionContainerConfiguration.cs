namespace UnionContainers;

public class UnionContainerConfiguration
{
    public UnionContainerOptions Options { get; }
    
    public UnionContainerConfiguration(Action<UnionContainerOptions>? configureOptions)
    {
        Options = new UnionContainerOptions();
        configureOptions?.Invoke(Options);
    }
    
}