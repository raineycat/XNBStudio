using SixLabors.ImageSharp;

namespace PortableXNB.Formats;

public class Texture3D
{
    public XnbSurfaceFormat Format { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }

    public List<Image> Slices = [];
}