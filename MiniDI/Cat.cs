using System.Collections.Concurrent;

namespace MiniDI;

public class Cat : IServiceProvider, IDisposable
{

    internal readonly Cat _root;
    internal readonly ConcurrentDictionary<Type, ServiceRegistry> _registries;
    private readonly ConcurrentDictionary<Key, object> _services;
    private readonly ConcurrentBag<IDisposable> _disposables;
    private volatile bool _disposed;

    public Cat()
    {
        _registries = new ConcurrentDictionary<Type, ServiceRegistry>();
        _services = new ConcurrentDictionary<Key, object>();
        _root = this;
        _disposables = new ConcurrentBag<IDisposable>();
    }

    internal Cat(Cat parent)
    {
        _root = parent;
        _registries = parent._registries;
        _services = new ConcurrentDictionary<Key, object>();
        _disposables = new ConcurrentBag<IDisposable>();
    }

    public Cat Register(ServiceRegistry registry)
    {
        EnsureNotDisposed();

        if (_registries.TryGetValue(registry.ServiceType, out var existing))
        {
            _registries[registry.ServiceType] = registry;
            _registries[registry.ServiceType].next = existing;
        }
        else
        {
            _registries[registry.ServiceType] = registry;
        }

        return this;
    }

    private object GetServiceCore(ServiceRegistry registry, Type[] genericArguments)
    {
        var key = new Key(registry, genericArguments);

        switch (registry.Lifetime)
        {
            case Lifetime.Root:
                return GetOrCreate(_root._services, _root._disposables);
            case Lifetime.Self:
                return GetOrCreate(_services, _disposables);
            default:
                {
                    var service = registry.Factory(this, genericArguments);
                    if (service is IDisposable disposable)
                    {
                        _disposables.Add(disposable);
                    }

                    return service;
                }
        }

        object GetOrCreate(ConcurrentDictionary<Key, object> services, ConcurrentBag<IDisposable> disposables)
        {
            if (services.TryGetValue(key, out var service))
            {
                return service;
            }
            service = registry.Factory(this, genericArguments);
            services[key] = service;
            if (service is IDisposable disposable)
            {
                disposables.Add(disposable);
            }
            return service;
        }
    }

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(Cat) || serviceType == typeof(IServiceProvider))
        {
            return this;
        }

        ServiceRegistry registry;

        // ?
        if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var elementType = serviceType.GetGenericArguments()[0];
            if (!_registries.TryGetValue(elementType, out registry))
            {
                return Array.CreateInstance(elementType, 0);
            }

            var registris = registry.AsEnumerable();

            var services = registris.Select(it => GetServiceCore(it, Type.EmptyTypes)).ToArray();

            Array array = Array.CreateInstance(elementType, services.Length);
            services.CopyTo(array, 0);
            return array;
        }

        if (serviceType.IsGenericType && !_registries.ContainsKey(serviceType))
        {
            var definition = serviceType.GetGenericTypeDefinition();
            return _registries.TryGetValue(definition, out registry)
                ? GetServiceCore(registry, serviceType.GetGenericArguments())
                : null;
        }

        return _registries.TryGetValue(serviceType, out registry)
            ? GetServiceCore(registry, new Type[0])
            : null;
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    public void Dispose()
    {
        _disposed = true;
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
        _disposables.Clear();
        _services.Clear();
    }

}
