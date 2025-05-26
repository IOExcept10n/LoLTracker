using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace LoLTracker.ViewModels
{
    public class ProbabilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double probability)
            {
                return $"{probability * 100:0.00}%";
            }
            return "-.--%";
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
