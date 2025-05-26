using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace LoLTracker.ViewModels
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible; // true -> IsVisible = true
            }
            return false; // по умолчанию скрыто
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
