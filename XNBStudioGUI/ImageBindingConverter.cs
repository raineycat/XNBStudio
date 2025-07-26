using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XNBStudioGUI;

public class ImageBindingConverter : IValueConverter
{
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(IntPtr value);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is SixLabors.ImageSharp.Image imgSharp)
        {
            return GetBMP(imgSharp);
        }

        if(value is System.Drawing.Image imgDrawing)
        {
            var bitmap = new System.Drawing.Bitmap(imgDrawing);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }

        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
    
    private unsafe WriteableBitmap GetBMP(Image _imgState)
    {
        var bmp = new WriteableBitmap(
            _imgState.Width, 
            _imgState.Height, 
            _imgState.Metadata.HorizontalResolution, 
            _imgState.Metadata.VerticalResolution, 
            PixelFormats.Bgra32, 
            null);

        bmp.Lock();
        try
        {
            var bbuf = new Span<byte>((void*)bmp.BackBuffer, bmp.BackBufferStride * bmp.PixelHeight);
            using var _image = _imgState.CloneAs<Bgra32>();
            _image.CopyPixelDataTo(bbuf);
            bmp.AddDirtyRect(new Int32Rect(0, 0, _imgState.Width, _imgState.Height));
        }
        finally
        {
            bmp.Unlock();
        }
        return bmp;
    }
}