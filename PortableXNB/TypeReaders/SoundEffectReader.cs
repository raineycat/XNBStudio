using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;
using PortableXNB.Formats;

namespace PortableXNB.TypeReaders;

public class SoundEffectReader : ITypeReader
{
    public string Name => "Sound effect reader";
    public string DefaultFileExtension => "wav";

    public bool MatchType(string type)
    {
        if (type.StartsWith("Microsoft.Xna.Framework.Content.SoundEffectReader")) return true;
        return false;
    }

    public object LoadAsset(XnbFile xnb)
    {
        // xnb.Contents.Seek(0, SeekOrigin.Begin);
        using var r = new BinaryReader(xnb.Contents, Encoding.Default, true);

        var poly = r.ReadByte();
        if (poly > 0)
        {
            return LoadSound(r);
        }
        else
        {
            throw new XnbException("Invalid poly byte");
        }
    }

    private static unsafe SoundEffect LoadSound(BinaryReader r)
    {
        var sfx = new SoundEffect();
        
        var headerSize = r.ReadInt32();
        var header = r.ReadBytes(headerSize);
        fixed (byte* headerPtr = header)
        {
            sfx.WaveFormat = Marshal.PtrToStructure<WaveFormat>(new IntPtr(headerPtr))!;
        }

        var dataSize = r.ReadInt32();
        sfx.SampleBuffer = r.ReadBytes(dataSize);

        sfx.LoopStart = r.ReadInt32();
        sfx.LoopLength = r.ReadInt32();
        sfx.Duration = TimeSpan.FromMilliseconds(r.ReadInt32());

        return sfx;
    }
}