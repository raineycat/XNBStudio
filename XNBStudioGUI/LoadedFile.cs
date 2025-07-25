using System.IO;
using PortableXNB;

namespace XNBStudioGUI;

public class LoadedFile(string filePath)
{
    public string FileName => Path.GetFileNameWithoutExtension(FilePath);
    public string FilePath { get; } = filePath;
    public XnbFile? Data { get; private set; }
    public Exception? Error { get; private set; }

    public bool IsValid => Data != null;
    public bool IsError => Error != null;

    public static LoadedFile Open(string path)
    {
        var file = new LoadedFile(path);
        try
        {
            using var s = File.OpenRead(path);
            file.Data = XnbFile.LoadFrom(s);
        }
        catch (Exception e)
        {
            file.Error = e;
        }

        return file;
    }
}