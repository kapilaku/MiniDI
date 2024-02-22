using System.Collections.Specialized;

namespace MiniASPNETCore;

public class HttpContext
{
    public HttpRequest Request { get; }
    public HttpResponse Response { get; }

    public HttpContext(IFeatureCollection features)
    {
        Request = new HttpRequest(features);
        Response = new HttpResponse(features);
    }
}

public interface IHttpRequestFeature
{
    public Uri Url { get; }
    public NameValueCollection Headers { get; }
    public Stream Body { get; }
}

public interface IHttpResponseFeature
{
    public NameValueCollection Headers { get; }
    public Stream Body { get; }
    public int StatusCode { get; set; }
}

public class HttpRequest
{
    private readonly IHttpRequestFeature _feature;

    public Uri Url => _feature.Url;
    public NameValueCollection Headers => _feature.Headers;
    public Stream Body => _feature.Body;

    public HttpRequest(IFeatureCollection features)
    {
        _feature = features.Get<IHttpRequestFeature>();
    }
}

public class HttpResponse
{
    private readonly IHttpResponseFeature _feature;

    public NameValueCollection Headers => _feature.Headers;
    public Stream Body => _feature.Body;
    public int StatusCode { get => _feature.StatusCode; set => _feature.StatusCode = value; }

    public HttpResponse(IFeatureCollection features)
    {
        _feature = features.Get<IHttpResponseFeature>();
    }
}