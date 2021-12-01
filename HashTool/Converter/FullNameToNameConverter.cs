using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace HashTool.Converter
{
    internal class FullNameToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string arg)
            {
                if (arg == "文件")
                {
                    if (value is string fullName)
                        return Path.GetFileName(fullName);
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
