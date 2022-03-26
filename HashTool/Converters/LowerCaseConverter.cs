using System;
using System.Globalization;
using System.Windows.Data;

using HashTool.Helpers;
using HashTool.Models.Enums;

namespace HashTool.Converters
{
    internal class LowerCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (PropertiesHelper.Settings.IsLowerCase
                && value is string val
                && parameter is AlgorithmEnum arg
                && arg != AlgorithmEnum.QuickXor)
                return val.ToLower();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
