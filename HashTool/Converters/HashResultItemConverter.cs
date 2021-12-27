using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace HashTool.Converters
{
    internal class HashResultItemConverter : IValueConverter
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
                else if (arg == "文本")
                {
                    if (value is string multiString)
                        return multiString.ReplaceLineEndings("  ");
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
