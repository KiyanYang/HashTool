using System;
using System.Globalization;
using System.Windows.Data;

namespace HashTool.Converters
{
    internal class LowerCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Properties.Settings.Default.IsLowerCase
                && value is string val
                && parameter is string arg
                && arg != "QuickXor")
                return val.ToLower();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
