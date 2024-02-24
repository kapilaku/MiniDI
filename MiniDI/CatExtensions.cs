using System.Runtime.InteropServices;

namespace MiniDI;

public static class CatExtensions
{
    public static Cat Register(this Cat cat, Type from, Type to, Lifetime lifetime)
    {
        Func<Cat, Type[], object> factory =
            (_, arguments) => Create(_, to, arguments);
        cat.Register(new ServiceRegistry(from, lifetime, factory));
        return cat;
    }

    public static Cat Register<TFrom, TTo>(this Cat cat, Lifetime lifetime)
    {
        return cat.Register(typeof(TFrom), typeof(TTo), lifetime);
    }

    public static Cat Register(this Cat cat, Type serviceType, object instance)
    {
        Func<Cat, Type[], object> factory = (_, arguments) => instance;
        cat.Register(new ServiceRegistry(serviceType, Lifetime.Root, factory));
        return cat;
    }

    public static Cat Register<TService>(this Cat cat, TService instance)
    {
        Func<Cat, Type[], object> factory = (_, arguments) => instance;
        cat.Register(new ServiceRegistry(typeof(TService), Lifetime.Root, factory));
        return cat;
    }

    public static Cat Register(this Cat cat, Type serviceType, Func<Cat, object> factory, Lifetime lifetime)
    {
        cat.Register(new ServiceRegistry(serviceType, lifetime, (_, arguments) => factory(_)));
        return cat;
    }

    public static Cat Register<TService>(this Cat cat, Func<Cat, TService> factory, Lifetime lifetime)
    {
        cat.Register(new ServiceRegistry(typeof(TService), lifetime, (_, arugments) => factory(_)));
        return cat;
    }

    private static object Create(Cat cat, Type type, Type[] genericArguments)
    {
        if (genericArguments.Length > 0)
        {
            type = type.MakeGenericType(genericArguments);
        }
        var constructors = type.GetConstructors();
        if (constructors.Length == 0)
        {
            throw new InvalidOperationException($"Cannot create the instance of {type} which doesn't have a public constructor");
        }

        var constructor = constructors.FirstOrDefault(it => it.GetCustomAttributes(false).OfType<InjectionAttribute>().Any());

        constructor ??= constructors.First();

        var parameters = constructor.GetParameters();
        if (parameters.Length == 0)
        {
            return Activator.CreateInstance(type);
        }
        var arguments = new object[parameters.Length];
        for (var index = 0; index < parameters.Length; index++)
        {
            arguments[index] = cat.GetService(parameters[index].ParameterType);
        }

        return constructor.Invoke(arguments);
    }

    public static T GetService<T>(this Cat cat)
    {
        return (T)cat.GetService(typeof(T));
    }
    public static IEnumerable<T> GetServices<T>(this Cat cat)
    {
        return cat.GetService<IEnumerable<T>>();
    }
    public static Cat CreateChild(this Cat cat)
    {
        return new Cat(cat);
    }
}
