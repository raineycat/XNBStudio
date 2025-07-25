using SixLabors.ImageSharp;

namespace PortableXNB;

public class XnbTexture2D
{
    public XnbSurfaceFormat Format { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public List<Image> Mips { get; set; } = [];
}