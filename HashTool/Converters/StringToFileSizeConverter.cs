using System;
using System.Globalization;
using System.Windows.Data;

using HashTool.Helpers;

namespace HashTool.Converters
{
    internal class StringToFileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long val;
            if (long.TryParse((string)value, out val))
            {
                return CommonHelper.FileSizeFormatter(val);
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
