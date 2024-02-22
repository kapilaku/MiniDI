namespace MiniASPNETCore;

public class WebHostBuilder : IWebHostBuilder
{
    private IServer _server;
    private readonly List<Action<IApplicationBuilder>> _configures = new List<Action<IApplicationBuilder>>();

    public IWebHost Build()
    {
        var builder = new ApplicationBuilder();
        foreach (var configure in _configures)
        {
            configure(builder);
        }

        return new WebHost(_server, builder.Build());
    }

    public IWebHostBuilder Configure(Action<IApplicationBuilder> configure)
    {
        _configures.Add(configure);
        return this;
    }

    public IWebHostBuilder UseServer(IServer server)
    {
        _server = server;
        return this;
    }
}

public interface IWebHostBuilder
{
    public IWebHostBuilder UseServer(IServer server);
    public IWebHostBuilder Configure(Action<IApplicationBuilder> configure);
    public IWebHost Build();
}