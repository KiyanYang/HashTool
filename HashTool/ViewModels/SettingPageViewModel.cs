// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.Input;

using HashTool.Models;
using HashTool.Models.Controls;

namespace HashTool.ViewModels;

internal sealed partial class SettingPageViewModel : ObservableObject
{
    public SettingPageViewModel()
    {
    }

    private static PropertiesSettingsModel s_propertiesSettings = GetInstance<PropertiesSettingsModel>();

    #region Public Properties

    /// <summary>
    /// 结果小写。
    /// </summary>
    [ObservableProperty]
    private CheckBoxModel _isLowerCase = new() { Content = "结果小写", IsChecked = s_propertiesSettings.IsLowerCase };

    /// <summary>
    /// 主界面置顶。
    /// </summary>
    [ObservableProperty]
    private CheckBoxModel _mainWindowTopmost = new() { Content = "主界面置顶", IsChecked = s_propertiesSettings.MainWindowTopmost };

    /// <summary>
    /// 结果界面置顶。
    /// </summary>
    [ObservableProperty]
    private CheckBoxModel _hashResultWindowTopMost = new() { Content = "结果界面置顶", IsChecked = s_propertiesSettings.HashResultWindowTopmost };

    #endregion

    #region Command

    [RelayCommand]
    private void SaveSetting()
    {
        s_propertiesSettings.IsLowerCase = IsLowerCase.IsChecked ?? false;
        s_propertiesSettings.MainWindowTopmost = MainWindowTopmost.IsChecked ?? false;
        s_propertiesSettings.HashResultWindowTopmost = HashResultWindowTopMost.IsChecked ?? false;
    }

    [RelayCommand]
    private void CancelSetting()
    {
        IsLowerCase.IsChecked = s_propertiesSettings.IsLowerCase;
        MainWindowTopmost.IsChecked = s_propertiesSettings.MainWindowTopmost;
        HashResultWindowTopMost.IsChecked = s_propertiesSettings.HashResultWindowTopmost;
    }

    #endregion
}
