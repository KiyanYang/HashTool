// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using HashTool.Models;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HashTool.ViewModels
{
    public class HashResultViewModel : ObservableObject
    {
        public HashResultViewModel(List<HashResultModel> hashResults)
        {
            #region 初始化 HashResults

            hashResult = hashResults[0];
            hashAllResults = new(hashResults);
            hashResultItems = new();
            hashResults.ForEach(i => hashResultItems.Add(i));

            #endregion

            #region 初始化 ListBox 参数

            if (hashAllResults.Count > 1)
            {
                hashResultListBoxColWidth = "*";
                hashResultListBoxVisibility = Visibility.Visible;
            }
            else
            {
                hashResultListBoxColWidth = "Auto";
                hashResultListBoxVisibility = Visibility.Collapsed;
            }

            #endregion

            ViewInExplorerCommand = new RelayCommand<string>(ViewInExplorer);
            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);
        }

        #region Fields

        private readonly List<HashResultModel> hashAllResults;

        private bool? isLowerCase;
        private string? hashResultListBoxColWidth;
        private Visibility? hashResultListBoxVisibility;
        private int? selectedIndex;

        private HashResultModel hashResult;
        private ObservableCollection<HashResultModel> hashResultItems;

        #endregion

        #region Public Properties/Commands

        public bool IsLowerCase
        {
            get => isLowerCase ??= GetInstance<PropertiesSettingsModel>().IsLowerCase;
            set
            {
                SetProperty(ref isLowerCase, value);
                GetInstance<PropertiesSettingsModel>().IsLowerCase = value;
            }
        }
        public string HashResultListBoxColWidth
        {
            get => hashResultListBoxColWidth ??= "Auto";
            set => SetProperty(ref hashResultListBoxColWidth, value);
        }
        public Visibility HashResultListBoxVisibility
        {
            get => hashResultListBoxVisibility ??= Visibility.Visible;
            set => SetProperty(ref hashResultListBoxVisibility, value);
        }
        /// <summary>
        /// 当前所选择项目的索引。
        /// </summary>
        public int SelectedIndex
        {
            get => selectedIndex ??= 0;
            set
            {
                SetProperty(ref selectedIndex, value);
                ShowSelected(value);
            }
        }

        /// <summary>
        /// 用于展示的哈希结果。
        /// </summary>
        public HashResultModel HashResult
        {
            get => hashResult;
            set => SetProperty(ref hashResult, value);
        }
        /// <summary>
        /// 全部的哈希结果。
        /// </summary>
        public ObservableCollection<HashResultModel> HashResultItems
        {
            get => hashResultItems;
            set => SetProperty(ref hashResultItems, value);
        }

        public ICommand ViewInExplorerCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        #endregion

        #region Private Helpers

        private void ShowSelected(int index)
        {
            if (index >= 0 && index < hashAllResults.Count)
            {
                HashResult = hashAllResults[index];
            }
        }

        private void ViewInExplorer(string? path)
        {
            if (path == null)
                return;

            string fullPath = Path.GetFullPath(path);
            Process p = new();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = $"/select, {fullPath}";
            p.Start();
        }

        private void CopyToClipboard(string? text)
        {
            if (text == null)
                return;

            Clipboard.SetText(text);
            HandyControl.Controls.Growl.SuccessGlobal("已复制到剪贴板！");
        }

        #endregion
    }
}
