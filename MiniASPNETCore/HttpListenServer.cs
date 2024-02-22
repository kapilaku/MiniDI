using System.Net;

namespace MiniASPNETCore;

public interface IServer
{
    public Task StartAsync(RequestDelegate handler);
}

public class HttpListenServer : IServer
{
    private readonly HttpListener _listener;
    private readonly string[] _urls;

    public HttpListenServer(params string[] urls)
    {
        _listener = new HttpListener();
        _urls = urls.Any() ? urls : new string[] { "http://localhost:8080/" };
    }

    public async Task StartAsync(RequestDelegate handler)
    {
        Array.ForEach(_urls, url => _listener.Prefixes.Add(url));
        _listener.Start();
        while (true)
        {
            var listenerContext = await _listener.GetContextAsync();
            var feature = new HttpListenerFeature(listenerContext);

            var features = new FeatureCollection()
                .Set<IHttpRequestFeature>(feature)
                .Set<IHttpResponseFeature>(feature);

            var httpContext = new HttpContext(features);

            await handler(httpContext);

            listenerContext.Response.Close();
        }
    }
}
