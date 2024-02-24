using System.Reflection;

namespace MiniDI;

public class Program
{
    static void Main(string[] args)
    {
        var root = new Cat()
            .Register<IFoo, Foo>(Lifetime.Transient)
            .Register<IBar>(_ => new Bar(), Lifetime.Self)
            .Register<IBaz, Baz>(Lifetime.Root)
            .Register(Assembly.GetEntryAssembly());

        var cat1 = root.CreateChild();
        var cat2 = root.CreateChild();

        void GetService<TService>(Cat cat)
        {
            cat.GetService<TService>();
        }

        GetService<IFoo>(cat1);
        GetService<IBar>(cat1);
        GetService<IBaz>(cat1);
        Console.WriteLine();
        GetService<IFoo>(cat2);
        GetService<IBar>(cat2);
        GetService<IBaz>(cat2);
    }
}
