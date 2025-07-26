using System.Text;
using PortableXNB.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PortableXNB.TypeReaders;

public class Texture3DReader : ITypeReader
{
    public string Name => "Texture reader (3D)";
    public string DefaultFileExtension => "bt3d";

    public bool MatchType(string type)
    {
        if (type.StartsWith("Microsoft.Xna.Framework.Content.Texture3DReader")) return true;
        return false;
    }

    public object LoadAsset(XnbFile xnb)
    {
        // xnb.Contents.Seek(0, SeekOrigin.Begin);
        using var r = new BinaryReader(xnb.Contents, Encoding.Default, true);

        var poly = r.ReadByte();
        if (poly > 0)
        {
            return LoadImage3D(r);
        }
        else
        {
            throw new XnbException("Invalid poly byte");
        }
    }
    
    private static Texture3D LoadImage3D(BinaryReader r)
    {
        var surfaceFormat = (XnbSurfaceFormat)r.ReadInt32();
        var img = new Texture3D
        {
            Format = surfaceFormat,
            Width = r.ReadInt32(),
            Height = r.ReadInt32(),
            Depth = r.ReadInt32()
        };
        var dataLength = r.ReadInt32();

        if (surfaceFormat != XnbSurfaceFormat.Color)
        {
            throw new XnbException("Texture3D decompression not implemented!");
        }

        var buffer = r.ReadBytes(dataLength);
        var sliceSize = img.Width * img.Height * 4;
        for (var z = 0; z < img.Depth; z++)
        {
            var sliceOffset = z * sliceSize;
            var slice = Image.LoadPixelData<Bgra32>(buffer.AsSpan(sliceOffset, sliceSize), img.Width, img.Height);
            img.Slices.Add(slice);
        }
        
        return img;
    }
}