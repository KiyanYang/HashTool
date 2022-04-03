// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

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
