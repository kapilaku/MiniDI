namespace MiniASPNETCore;

public interface IApplicationBuilder
{
    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware);
    RequestDelegate Build();
}

public class ApplicationBuilder : IApplicationBuilder
{

    private readonly ICollection<Func<RequestDelegate, RequestDelegate>> middlewares = new List<Func<RequestDelegate, RequestDelegate>>();

    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
    {
        middlewares.Add(middleware);
        return this;
    }
    public RequestDelegate Build()
    {
        RequestDelegate next = _ => { _.Response.StatusCode = 404; return Task.CompletedTask; };

        foreach (var middleware in middlewares.Reverse())
        {
            next = middleware(next);
        }

        return next;
    }
}
