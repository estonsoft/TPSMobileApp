using System.Globalization;

namespace TPSMobileApp.Controls;

class StringToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var valueAsString = value?.ToString() ?? string.Empty;

        return valueAsString switch
        {
            "" => Colors.Transparent,
            "Accent" => Application.Current?.Resources.TryGetValue("AccentColor", out var accent) == true
                ? (Color)accent
                : Colors.Blue,
            _ => Color.FromArgb(valueAsString)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
