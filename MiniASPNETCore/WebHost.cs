namespace MiniASPNETCore;

public class WebHost : IWebHost
{
    private IServer _server;
    private RequestDelegate _handler;

    public WebHost(IServer server, RequestDelegate handler)
    {
        _server = server;
        _handler = handler;
    }

    public async Task StartAsync()
    {
        await _server.StartAsync(_handler);
    }
}

public interface IWebHost
{
    public Task StartAsync();
}