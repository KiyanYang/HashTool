using System;
using System.Globalization;
using System.Windows.Data;

namespace HashTool.Converters
{
    internal class ProportionStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (values.Length == 2) ? $"{values[0]} / {values[1]}" : string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
