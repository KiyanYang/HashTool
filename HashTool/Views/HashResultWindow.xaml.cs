// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows;

using HashTool.Models;
using HashTool.ViewModels;

namespace HashTool.Views;

/// <summary>
/// HashResultWindow.xaml 的交互逻辑
/// </summary>
public sealed partial class HashResultWindow : Window
{
    public HashResultWindow(List<HashResultModel> hashResults)
    {
        InitializeComponent();

        if (hashResults.Count > 1)
        {
            HashResultListBoxCol.Width = new(1, GridUnitType.Star);
            HashResultListBox.Visibility = Visibility.Visible;
            HashResultSplitter.Visibility = Visibility.Visible;
        }
        else
        {
            HashResultListBoxCol.Width = new(0, GridUnitType.Auto);
            HashResultListBox.Visibility = Visibility.Collapsed;
            HashResultSplitter.Visibility = Visibility.Collapsed;
        }

        HashResultViewModel vm = new(hashResults);
        DataContext = vm;
    }
}
