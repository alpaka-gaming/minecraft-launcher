using System;
using System.Globalization;
using Avalonia.Data.Converters;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable OperatorIsCanBeUsed

namespace Launcher.Converters
{
    public class NullValidatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(string))
                return !string.IsNullOrWhiteSpace((string)value);
            else
                return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(string))
                return string.IsNullOrWhiteSpace((string)value);
            else
                return value == null;
        }
    }
}