using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FileSystem;

internal class Program
{
    static async Task Main(string[] args)
    {
        await MonitorFileChanges();
    }

    static async Task MonitorFileChanges()
    {
        using var fileProvider = new PhysicalFileProvider(@"c:\test\");
        string? origin = null;
        ChangeToken.OnChange(() => fileProvider.Watch("data.txt"), CallBack);
        while (true)
        {
            File.WriteAllText(@"c:\test\data.txt", DateTime.UtcNow.ToString());
            await Task.Delay(5000);
        }
        async void CallBack()
        {
            var stream = fileProvider.GetFileInfo("data.txt").CreateReadStream();
            {
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                var current = Encoding.Default.GetString(buffer);
                if (current != origin)
                {
                    Console.WriteLine(origin = current);
                    // ? origin = current;
                }
            }
        }
    }

    static async Task ReadFileContentAsync()
    {
        var content = await new ServiceCollection()
            .AddSingleton<IFileProvider>(new PhysicalFileProvider(@"c:\test"))
            .AddSingleton<IFileSystem, FileSystem>()
            .BuildServiceProvider()
            .GetService<IFileSystem>()
            .ReadAllTextAsync("test.txt");

        Console.WriteLine(content);

        Debug.Assert(content == File.ReadAllText(@"c:\test\test.txt"));
    }

    static void ReadDirectories()
    {
        new ServiceCollection()
            .AddSingleton<IFileProvider>(new PhysicalFileProvider(@"c:\Users\Public"))
            .AddSingleton<IFileSystem, FileSystem>()
            .BuildServiceProvider()
            .GetService<IFileSystem>()
            .ShowStructure(Print);
    }

    static async Task ReadEmbeddedFileAsync()
    {
        var content = await new ServiceCollection()
            .AddSingleton<IFileProvider>(new EmbeddedFileProvider(Assembly.GetEntryAssembly()))
            .AddSingleton<IFileSystem, FileSystem>()
            .BuildServiceProvider()
            .GetService<IFileSystem>()
            .ReadAllTextAsync("test.txt");

        Console.WriteLine(content);

        Debug.Assert(content == File.ReadAllText(@"c:\test\test.txt"));
    }

    static void Print(int layer, string name) => Console.WriteLine($"{new string(' ', layer * 4)}{name}");
}
