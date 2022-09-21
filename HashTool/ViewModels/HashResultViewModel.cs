// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.Input;

using HashTool.Models;

namespace HashTool.ViewModels;

public sealed partial class HashResultViewModel : ObservableObject
{
    public HashResultViewModel(List<HashResultModel> hashResults)
    {
        #region 初始化 HashResults

        _selectedHashResult = hashResults[0];
        _hashResultItems = new(hashResults);

        #endregion

        #region 初始化 ListBox 参数

        if (hashResults.Count > 1)
        {
            _hashResultListBoxColWidth = "*";
            _hashResultListBoxVisibility = Visibility.Visible;
        }
        else
        {
            _hashResultListBoxColWidth = "Auto";
            _hashResultListBoxVisibility = Visibility.Collapsed;
        }

        #endregion
    }

    #region Fields

    private bool? _isLowerCase;
    private PropertiesSettingsModel? _propertiesSettings;

    #endregion

    #region Public Properties

    public PropertiesSettingsModel PropertiesSettings
    {
        get => _propertiesSettings ??= GetInstance<PropertiesSettingsModel>();
    }

    /// <summary>
    /// 复选框，结果是否小写。
    /// </summary>
    public bool IsLowerCase
    {
        get => _isLowerCase ??= GetInstance<PropertiesSettingsModel>().IsLowerCase;
        set => SetProperty(ref _isLowerCase, value);
    }

    /// <summary>
    /// 左侧结果列表的列宽。
    /// </summary>
    [ObservableProperty]
    private string _hashResultListBoxColWidth = "Auto";

    /// <summary>
    /// 左侧结果列表的可见性。
    /// </summary>
    [ObservableProperty]
    private Visibility _hashResultListBoxVisibility = Visibility.Visible;

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
    private void ViewInExplorer(string? path)
    {
        if (path == null)
        {
            return;
        }

        string fullPath = Path.GetFullPath(path);
        Process p = new();
        p.StartInfo.FileName = "explorer.exe";
        p.StartInfo.Arguments = $"/select, {fullPath}";
        p.Start();
    }

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
