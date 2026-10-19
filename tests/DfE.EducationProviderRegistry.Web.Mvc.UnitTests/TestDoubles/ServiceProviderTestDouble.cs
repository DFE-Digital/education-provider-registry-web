using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.TestDoubles;

public sealed class ServiceProviderTestDouble : IServiceProvider, IKeyedServiceProvider
{
    private readonly Dictionary<(Type Type, object? Key), object> _services = [];

    public static ServiceProviderTestDouble Create() => new();

    public void AddKeyedService<T>(object key, T service)
        where T : class
    {
        _services[(typeof(T), key)] = service;
    }

    public object? GetService(Type serviceType)
    {
        _services.TryGetValue((serviceType, null), out object? service);

        return service;
    }

    public object? GetKeyedService(
        Type serviceType,
        object? serviceKey)
    {
        _services.TryGetValue(
            (serviceType, serviceKey),
            out object? service);

        return service;
    }

    public object GetRequiredKeyedService(
        Type serviceType,
        object? serviceKey)
    {
        return GetKeyedService(serviceType, serviceKey)
            ?? throw new InvalidOperationException(
                $"No service for type '{serviceType}' and key '{serviceKey}'.");
    }
}
