// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.Input;

using HashTool.Models;

namespace HashTool.ViewModels;

public sealed partial class HashResultViewModel : ObservableObject
{
    public HashResultViewModel(List<HashResultModel> hashResults)
    {
        _selectedHashResult = hashResults[0];
        _hashResultItems = new(hashResults);
    }

    #region Public Properties

    public PropertiesSettingsModel PropertiesSettings => GetInstance<PropertiesSettingsModel>();

    /// <summary>
    /// 结果是否小写。
    /// </summary>
    [ObservableProperty]
    private bool _isLowerCase = GetInstance<PropertiesSettingsModel>().IsLowerCase;

    /// <summary>
    /// 当前所选择项目的索引。
    /// </summary>
    [ObservableProperty]
    private int _selectedIndex = 0;

    /// <summary>
    /// 用于展示的哈希结果。
    /// </summary>
    [ObservableProperty]
    private HashResultModel _selectedHashResult;

    /// <summary>
    /// 全部的哈希结果。
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<HashResultModel> _hashResultItems;

    #endregion

    #region Command

    [RelayCommand]
    private void CopyToClipboard(string? text)
    {
        if (text == null)
        {
            return;
        }

        Clipboard.SetText(text);
        HandyControl.Controls.Growl.SuccessGlobal("已复制到剪贴板！");
    }

    #endregion Command

    partial void OnSelectedIndexChanged(int value)
    {
        if (value >= 0 && value < HashResultItems.Count)
        {
            SelectedHashResult = HashResultItems[value];
        }
    }
}
