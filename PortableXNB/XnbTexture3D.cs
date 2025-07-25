using SixLabors.ImageSharp;

namespace PortableXNB;

public class XnbTexture3D
{
    public XnbSurfaceFormat Format { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }

    public List<Image> Slices = [];
}