using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LeetCodeManager.Converters;

public class StatusToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value?.ToString() switch
        {
            "Solved"      => new SolidColorBrush(Color.FromRgb(0x4E, 0xC9, 0xB0)),
            "Need Review" => new SolidColorBrush(Color.FromRgb(0xCE, 0x91, 0x78)),
            _             => new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80))
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

public class DifficultyToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value?.ToString() switch
        {
            "Easy"   => new SolidColorBrush(Color.FromRgb(0x4E, 0xC9, 0xB0)),
            "Medium" => new SolidColorBrush(Color.FromRgb(0xFF, 0xA1, 0x16)),
            "Hard"   => new SolidColorBrush(Color.FromRgb(0xF4, 0x47, 0x47)),
            _        => new SolidColorBrush(Color.FromRgb(0xD4, 0xD4, 0xD4))
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
