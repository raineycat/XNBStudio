using System.Text;
using PortableXNB.Compression;
using PortableXNB.TypeReaders;

namespace PortableXNB;

public class XnbFile : IDisposable
{
    public XnbPlatform Platform { get; set; }
    public XnbFlags Flags { get; set; }
    public XnbVersion Version { get; set; }
    public Dictionary<string, int> TypeReaders { get; set; } = [];
    public int SharedResourceCount { get; set; }
    public Stream Contents { get; set; }
    
    public bool IsHiDefProfile
    {
        get => Flags.HasFlag(XnbFlags.HiDefProfile);
        set
        {
            if (value)
            {
                Flags |= XnbFlags.HiDefProfile;
            }
            else
            {
                Flags &= ~(XnbFlags.HiDefProfile);
            }
        }
    }

    public bool IsCompressed => Flags.HasFlag(XnbFlags.CompressedWithLZX) || Flags.HasFlag(XnbFlags.CompressedWithLZ4);

    public static XnbFile LoadFrom(Stream s)
    {
        var xnb = new XnbFile();
        
        if (((char[])['X', 'N', 'B']).Any(c => s.ReadByte() != (byte)c))
        {
            throw new XnbException("Invalid magic number");
        }

        xnb.Platform = (XnbPlatform)s.ReadByte();
        if (!Enum.GetValues<XnbPlatform>().Contains(xnb.Platform))
        {
            throw new XnbException($"Invalid platform '{xnb.Platform}'");
        }

        xnb.Version = (XnbVersion)s.ReadByte();
        if (!Enum.GetValues<XnbVersion>().Contains(xnb.Version))
        {
            throw new XnbException($"Invalid version '{xnb.Version}'");
        }

        xnb.Flags = (XnbFlags)s.ReadByte();

        var reader = new BinaryReader(s, Encoding.ASCII, true);
        var compressedSize = reader.ReadUInt32();
        byte[] buffer;

        if (xnb.IsCompressed)
        {
            var decompressedSize = reader.ReadUInt32();
            using var ms = new MemoryStream((int)decompressedSize);

            if (xnb.Flags.HasFlag(XnbFlags.CompressedWithLZX))
            {
                using var lzx = new LzxDecoderStream(s, (int)decompressedSize, (int)compressedSize);
                lzx.CopyTo(ms);
            }
            else
            {
                throw new XnbException("Unsupported compression type!");
            }
            
            buffer = ms.GetBuffer();
        }
        else
        {
            buffer = new byte[compressedSize];
            compressedSize = (uint)s.Read(buffer);
        }

        var actualStream = new MemoryStream(buffer);
        reader.Dispose();
        reader = new BinaryReader(actualStream, Encoding.ASCII, true);

        if (xnb.Version is XnbVersion.XnaFramework3_1 or XnbVersion.XnaGameStudio4)
        {
            var numTypeReaders = reader.Read7BitEncodedInt();
            //xnb.TypeReaders = new Dictionary<string, int>();
            for (var i = 0; i < numTypeReaders; i++)
            {
                var name = reader.ReadString();
                var version = reader.ReadInt32();
                xnb.TypeReaders.Add(name, version);
            }
        
            xnb.SharedResourceCount = reader.Read7BitEncodedInt();
        }
        
        var contentsStartPos = (int)reader.BaseStream.Position;
        // Log.Debug("XNB content starts at: {Pos}", contentsStartPos);
        
        // xnb.Contents = buffer[contentsStartPos..];
        xnb.Contents = new MemoryStream(buffer, contentsStartPos, (int)(reader.BaseStream.Length - contentsStartPos));
        
        return xnb;
    }

    public void Dispose()
    {
        Contents.Dispose();
    }
}