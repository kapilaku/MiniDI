using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Text;

namespace FileSystem;

public interface IFileSystem
{
    void ShowStructure(Action<int, string> print);
    Task<string> ReadAllTextAsync(string path);
}

public class FileSystem : IFileSystem
{
    private readonly IFileProvider _fileProvider;

    public FileSystem(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public async Task<string> ReadAllTextAsync(string path)
    {
        var fileInfo = _fileProvider.GetFileInfo(path);
        Byte[] buffer;
        if (fileInfo.Exists)
        {
            using (var stream = fileInfo.CreateReadStream())
            {
                buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                return Encoding.Default.GetString(buffer);

            }
        }
        return null;
    }

    public void ShowStructure(Action<int, string> print)
    {
        int indent = -1;
        Print("");

        void Print(string subpath)
        {
            indent++;
            foreach (var fileInfo in _fileProvider.GetDirectoryContents(subpath))
            {
                print(indent, fileInfo.Name);
                if (fileInfo.IsDirectory)
                {
                    Print($@"{subpath}\{fileInfo.Name}");
                }
            }
            indent--;
        }
    }
}
