// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows.Controls;

using CommunityToolkit.Mvvm.Input;

using HashTool.Models;
using HashTool.Models.Controls;
using HashTool.Views.Pages;

namespace HashTool.ViewModels;

internal sealed partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel()
    {
    }

    #region Public Properties

    public PropertiesSettingsModel PropertiesSettings => GetInstance<PropertiesSettingsModel>();

    [ObservableProperty]
    private Page _currentPage = GetInstance<HomePage>();

    public ProgressBarModel TaskbarProgress => GetInstance<HomePage>().TaskbarProgress;

    #endregion

    #region Command

    [RelayCommand]
    private void ShowPage(string? title)
    {
        CurrentPage = title switch
        {
            "主页" => GetInstance<HomePage>(),
            "设置" => GetInstance<SettingPage>(),
            "帮助" => GetInstance<HelpPage>(),
            "关于" => GetInstance<AboutPage>(),
            _ => GetInstance<HomePage>(),
        };
    }

    #endregion
}
