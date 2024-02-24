namespace MiniDI;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MapToAttribute : Attribute
{
    public Type ServiceType { get; }
    public Lifetime Lifetime { get; }

    public MapToAttribute(Type serviceType, Lifetime lifetime)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class InjectionAttribute : Attribute { }
