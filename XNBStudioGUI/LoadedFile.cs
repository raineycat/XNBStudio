using System.IO;
using PortableXNB;
using PortableXNB.Formats;
using PortableXNB.TypeReaders;
using SixLabors.ImageSharp;

namespace XNBStudioGUI;

public class LoadedFile(string filePath)
{
    public string FileName => Path.GetFileNameWithoutExtension(FilePath);
    public string FilePath { get; } = filePath;
    public XnbFile? Data { get; private set; }
    public List<object> Assets { get; } = [];
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
            
            if (file.Data.TypeReaders.Count > 0)
            {
                var firstType = file.Data.TypeReaders.Keys.First();
                var tr = TypeReaderManager.GetReaderForType(firstType);
            
                if (tr != null)
                {
                    var asset = tr.LoadAsset(file.Data);
                    file.Assets.Add(asset);
                }
            }
        }
        catch (Exception e)
        {
            file.Error = e;
        }

        return file;
    }
}