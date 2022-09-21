// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows;

using HashTool.ViewModels;

namespace HashTool.Views;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainWindowViewModel mainWindowViewModel = new();
        DataContext = mainWindowViewModel;
    }
}
