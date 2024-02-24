namespace MiniDI;

public class ServiceRegistry
{
    public Type ServiceType { get; }
    public Lifetime Lifetime { get; }
    public Func<Cat, Type[], object> Factory { get; }

    internal ServiceRegistry next;

    public ServiceRegistry(Type serviceType, Lifetime lifetime, Func<Cat, Type[], object> factory)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;
        Factory = factory;
    }

    internal IEnumerable<ServiceRegistry> AsEnumerable()
    {
        var list = new List<ServiceRegistry>();
        for (var self = this; self != null; self = self.next)
        {
            list.Add(self);
        }
        return list;
    }
}