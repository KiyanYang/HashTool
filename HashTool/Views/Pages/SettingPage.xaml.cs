﻿// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows.Controls;

using HashTool.ViewModels;

namespace HashTool.Views.Pages;

/// <summary>
/// SettingPage.xaml 的交互逻辑
/// </summary>
public sealed partial class SettingPage : Page
{
    public SettingPage()
    {
        InitializeComponent();

        SettingPageViewModel settingPageViewModel = new();
        DataContext = settingPageViewModel;
    }
}
