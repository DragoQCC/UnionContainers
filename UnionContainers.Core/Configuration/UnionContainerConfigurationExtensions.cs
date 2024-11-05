using Microsoft.Extensions.DependencyInjection;

namespace UnionContainers;

public static class UnionContainerConfigurationExtensions
{
    public static IServiceCollection AddUnionContainerConfiguration(this IServiceCollection services, Action<UnionContainerOptions>? configureOptions = null)
    {
        services.AddSingleton(new UnionContainerConfiguration(configureOptions));
        return services;
    }
}