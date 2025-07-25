using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace XNBStudioGUI;

public class ImageBindingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Image img) return null;
        using var converted = img.CloneAs<Rgba32>();
        var bmp = GetBMP(converted);
        return bmp;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
    
    private WriteableBitmap GetBMP(Image<Rgba32> _imgState)
    {
        var bmp = new WriteableBitmap(_imgState.Width, _imgState.Height, _imgState.Metadata.HorizontalResolution, _imgState.Metadata.VerticalResolution, PixelFormats.Bgra32, null);

        bmp.Lock();
        try
        {

            using Image<Rgba32> _image = _imgState;
            _image.ProcessPixelRows(accessor =>
            {
                var backBuffer = bmp.BackBuffer;

                for (var y = 0; y < _imgState.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                    for (var x = 0; x < _imgState.Width; x++)
                    {
                        var backBufferPos = backBuffer + (y * _imgState.Width + x) * 4;
                        var rgba = pixelRow[x];
                        var color = rgba.A << 24 | rgba.R << 16 | rgba.G << 8 | rgba.B;

                        System.Runtime.InteropServices.Marshal.WriteInt32(backBufferPos, color);
                    }
                }
            });

            bmp.AddDirtyRect(new Int32Rect(0, 0, _imgState.Width, _imgState.Height));
        }
        finally
        {
            bmp.Unlock();
        }
        return bmp;
    }
}