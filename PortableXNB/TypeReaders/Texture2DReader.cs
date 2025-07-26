using System.Text;
using PortableXNB.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Squish;

namespace PortableXNB.TypeReaders;

public class Texture2DReader : ITypeReader
{
    public string Name => "Texture reader (2D)";
    public string DefaultFileExtension => "png";

    public bool MatchType(string type)
    {
        if (type.StartsWith("Microsoft.Xna.Framework.Content.Texture2DReader")) return true;
        return false;
    }

    public object LoadAsset(XnbFile xnb)
    {
        // xnb.Contents.Seek(0, SeekOrigin.Begin);
        using var r = new BinaryReader(xnb.Contents, Encoding.Default, true);

        var poly = r.ReadByte();
        if (poly > 0)
        {
            return LoadImage(r, xnb.Version);
        }
        else
        {
            throw new XnbException("Invalid poly byte");
        }
    }
    
    public static Texture2D LoadImage(BinaryReader r, XnbVersion version)
    {
        var tex = new Texture2D();
        var formatCode = r.ReadInt32();
        
        // why did this change??
        // surely there's no need for this to change????
        var format = version switch
        {
            XnbVersion.XnaGameStudio4 => (XnbSurfaceFormat)formatCode,
            XnbVersion.XnaFramework3_1 => formatCode switch
            {
                1 => XnbSurfaceFormat.Color,
                28 => XnbSurfaceFormat.Dxt1,
                30 => XnbSurfaceFormat.Dxt3,
                32 => XnbSurfaceFormat.Dxt5,
                _ => throw new XnbException("Unsupported (legacy) surface format")
            },
            _ => throw new XnbException("Unknown XNB version")
        };
        
        var width = r.ReadUInt32();
        var height = r.ReadUInt32();
        var mipCount = r.ReadUInt32();

        for (var mip = 0; mip < mipCount; mip++)
        {
            var dataSize = r.ReadUInt32();
            var buffer = new byte[dataSize];
            r.BaseStream.Read(buffer);
            Image img;

            switch (format)
            {
                case XnbSurfaceFormat.Color:
                    img = Image.LoadPixelData<Rgba32>(buffer, (int)width, (int)height);
                    break;

                case XnbSurfaceFormat.Dxt1:
                case XnbSurfaceFormat.Dxt3:
                case XnbSurfaceFormat.Dxt5:
                {
                    // guess the size based on an RGBA image
                    var outBuffer = new byte[4 * width * height];
                    var flags = format switch
                    {
                        XnbSurfaceFormat.Dxt1 => SquishFlags.kDxt1,
                        XnbSurfaceFormat.Dxt3 => SquishFlags.kDxt3,
                        XnbSurfaceFormat.Dxt5 => SquishFlags.kDxt5,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    Squish.Squish.DecompressImage(outBuffer, (int)width, (int)height, buffer, flags);
                    img = Image.LoadPixelData<Rgba32>(outBuffer, (int)width, (int)height);
                    break;
                }
                
                case XnbSurfaceFormat.Rgba1010102:
                    img = Image.LoadPixelData<Rgba1010102>(buffer, (int)width, (int)height);
                    break;
                
                case XnbSurfaceFormat.Rg32:
                    img = Image.LoadPixelData<Rg32>(buffer, (int)width, (int)height);
                    break;
                
                case XnbSurfaceFormat.Rgba64:
                    img = Image.LoadPixelData<Rgba64>(buffer, (int)width, (int)height);
                    break;
                
                case XnbSurfaceFormat.Alpha8:
                    img = Image.LoadPixelData<A8>(buffer, (int)width, (int)height);
                    break;

            default:
                    throw new XnbException("Unsupported surface format! " + format);
            }
            
            tex.Mips.Add(img);
        }

        tex.Format = format;
        tex.Width = width;
        tex.Height = height;
        return tex;
    }
}