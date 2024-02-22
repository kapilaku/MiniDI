using System.Text;

namespace MiniASPNETCore;

public static partial class Extensions
{
    public static IWebHostBuilder UseHttpListenerServer(this IWebHostBuilder webHostBuilder)
    {
        var server = new HttpListenServer();
        return webHostBuilder.UseServer(server);
    }

    public static Task WriteAsync(this HttpResponse response, string content)
    {
        var buffer = Encoding.UTF8.GetBytes(content);
        return response.Body.WriteAsync(buffer, 0, buffer.Length);
    }
}
