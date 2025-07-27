using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace XNBStudioGUI;

public class OpacityToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is float fv) value = (double)fv;
        if (value is int iv) value = (double)iv;
        if (value is double val)
        {
            if (val is > 1 and <= 100) val /= 100;
            val = Math.Clamp(val, 0, 1);
            var rgbValue = (byte)(val * 255);
            return Color.FromRgb(rgbValue, rgbValue, rgbValue);
        }
        
        return Colors.Black;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}