// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using HashTool.Models;

namespace HashTool.ViewModels
{
    public class HashResultViewModel : ObservableObject
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

            ViewInExplorerCommand = new RelayCommand<string>(ViewInExplorer);
            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);
        }

        #region Fields

        private bool? _isLowerCase;
        private string? _hashResultListBoxColWidth;
        private Visibility? _hashResultListBoxVisibility;
        private int? _selectedIndex;
        private PropertiesSettingsModel? _propertiesSettings;

        private HashResultModel _selectedHashResult;
        private ObservableCollection<HashResultModel> _hashResultItems;

        #endregion

        #region Public Properties/Commands

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
        public string HashResultListBoxColWidth
        {
            get => _hashResultListBoxColWidth ??= "Auto";
            set => SetProperty(ref _hashResultListBoxColWidth, value);
        }

        /// <summary>
        /// 左侧结果列表的可见性。
        /// </summary>
        public Visibility HashResultListBoxVisibility
        {
            get => _hashResultListBoxVisibility ??= Visibility.Visible;
            set => SetProperty(ref _hashResultListBoxVisibility, value);
        }

        /// <summary>
        /// 当前所选择项目的索引。
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex ??= 0;
            set
            {
                SetProperty(ref _selectedIndex, value);
                ShowSelected(value);
            }
        }

        /// <summary>
        /// 用于展示的哈希结果。
        /// </summary>
        public HashResultModel SelectedHashResult
        {
            get => _selectedHashResult;
            set => SetProperty(ref _selectedHashResult, value);
        }

        /// <summary>
        /// 全部的哈希结果。
        /// </summary>
        public ObservableCollection<HashResultModel> HashResultItems
        {
            get => _hashResultItems;
            set => SetProperty(ref _hashResultItems, value);
        }

        public ICommand ViewInExplorerCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        #endregion

        #region Private Helpers

        private void ShowSelected(int index)
        {
            if (index >= 0 && index < HashResultItems.Count)
            {
                SelectedHashResult = HashResultItems[index];
            }
        }

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

        private void CopyToClipboard(string? text)
        {
            if (text == null)
            {
                return;
            }

            Clipboard.SetText(text);
            HandyControl.Controls.Growl.SuccessGlobal("已复制到剪贴板！");
        }

        #endregion
    }
}
