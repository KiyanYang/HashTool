using System;
using System.Globalization;
using System.Windows.Data;

namespace HashTool.Converter
{
    internal class ProportionStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                return $"{values[0]} / {values[1]}";
            }
            else
            {
                return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
