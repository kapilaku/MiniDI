namespace MiniDI;

internal class Key : IEquatable<Key>
{
    public ServiceRegistry Registry { get; }
    public Type[] GenericArguments { get; }

    public Key(ServiceRegistry registry, Type[] genericArguments)
    {
        Registry = registry;
        GenericArguments = genericArguments;
    }

    public bool Equals(Key? other)
    {
        if (ReferenceEquals(null, other)) return false;

        if (Registry != other.Registry) return false;

        if (GenericArguments.Length != other.GenericArguments.Length) return false;

        for (var i = 0; i < GenericArguments.Length; i++)
        {
            if (GenericArguments[i] != other.GenericArguments[i]) return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Key key ? Equals(key) : false;
    }

    public override int GetHashCode()
    {
        var hashCode = Registry.GetHashCode();
        foreach(var key in GenericArguments)
        {
            hashCode ^= key.GetHashCode();
        }
        return hashCode;
    }
}
