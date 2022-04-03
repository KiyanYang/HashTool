// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using HashTool.Helpers;
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

            this.hashResult = hashResults[0];
            this.hashAllResults = new(hashResults);
            this.hashResultItems = new();
            hashResults.ForEach(i => this.hashResultItems.Add(i));

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

            ShowSelectedCommand = new RelayCommand<int>(ShowSelected);
            ViewInExplorerCommand = new RelayCommand<string>(ViewInExplorer);
            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);
        }

        #region Fields

        private List<HashResultModel> hashAllResults;

        private bool? isLowerCase;
        private string? hashResultListBoxColWidth;
        private Visibility? hashResultListBoxVisibility;

        private HashResultModel hashResult;
        private ObservableCollection<HashResultModel> hashResultItems;

        #endregion

        #region Public Properties/Commands

        public bool IsLowerCase
        {
            get => isLowerCase ??= PropertiesHelper.Settings.IsLowerCase;
            set
            {
                SetProperty(ref isLowerCase, value);
                PropertiesHelper.Settings.IsLowerCase = value;
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

        public HashResultModel HashResult
        {
            get => hashResult;
            set => SetProperty(ref hashResult, value);
        }
        public ObservableCollection<HashResultModel> HashResultItems
        {
            get => hashResultItems;
            set => SetProperty(ref hashResultItems, value);
        }

        public ICommand ShowSelectedCommand { get; }
        public ICommand ViewInExplorerCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        #endregion

        #region Private Helpers

        private void ShowSelected(int index)
        {
            if (index >= 0)
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
