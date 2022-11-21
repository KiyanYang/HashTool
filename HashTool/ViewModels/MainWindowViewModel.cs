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

    #region Fields

    private PropertiesSettingsModel? _propertiesSettings;

    private Page? _currentPage;
    private HomePage? _homePage;
    private SettingPage? _settingPage;
    private HelpPage? _helpPage;
    private AboutPage? _aboutPage;

    #endregion

    #region Public Properties

    public PropertiesSettingsModel PropertiesSettings
    {
        get => _propertiesSettings ??= GetInstance<PropertiesSettingsModel>();
    }

    public Page CurrentPage
    {
        get => _currentPage ??= HomePage;
        set => SetProperty(ref _currentPage, value);
    }
    public HomePage HomePage
    {
        get => _homePage ??= new();
        set => SetProperty(ref _currentPage, value);
    }
    public SettingPage SettingPage
    {
        get => _settingPage ??= new();
        set => SetProperty(ref _currentPage, value);
    }
    public HelpPage HelpPage
    {
        get => _helpPage ??= new();
        set => SetProperty(ref _currentPage, value);
    }
    public AboutPage AboutPage
    {
        get => _aboutPage ??= new();
        set => SetProperty(ref _currentPage, value);
    }
    public ProgressBarModel TaskbarProgress => HomePage.TaskbarProgress;

    #endregion

    #region Command

    [RelayCommand]
    private void ShowPage(string? title)
    {
        CurrentPage = title switch
        {
            "主页" => HomePage,
            "设置" => SettingPage,
            "帮助" => HelpPage,
            "关于" => AboutPage,
            _ => HomePage,
        };
    }

    #endregion
}
