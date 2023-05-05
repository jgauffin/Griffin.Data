using Griffin.Data.Tests.Queries.Implementation.Subjects;

namespace Griffin.Data.Tests.Queries.Implementation;

internal class ServiceProviderStub : IServiceProvider
{
    private readonly Dictionary<Type, object> _instances = new();

    public object? GetService(Type serviceType)
    {
        return _instances.TryGetValue(serviceType, out var instance) ? instance : null;
    }

    public void Register<T>(MyQueryHandler handler)
    {
        _instances[typeof(T)] = handler;
    }
}
