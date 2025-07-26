using SixLabors.ImageSharp;

namespace PortableXNB.Formats;

public class Texture2D
{
    public XnbSurfaceFormat Format { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public List<Image> Mips { get; set; } = [];
}