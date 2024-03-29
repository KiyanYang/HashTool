﻿// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HashTool.Converters;

internal sealed class Boolean2VisibilityReConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null && !(bool)value ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
