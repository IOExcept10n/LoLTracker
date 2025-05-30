using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LoLTracker.ViewModels
{
    public class ProbabilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double probability)
            {
                return $"{probability * 100:F2}%";
            }
            return "-.--%";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}