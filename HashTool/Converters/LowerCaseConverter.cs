// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;

using HashTool.Common;

namespace HashTool.Converters
{
    internal class LowerCaseConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3
                && values[0] is string val
                && values[1] is string arg
                && arg != HashAlgorithmNames.QuickXor
                && values[2] is bool isLowerCase
                && isLowerCase)
                return val.ToLower();
            else
                return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
