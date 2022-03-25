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
            this.hashResult = hashResults[0];
            this.hashAllResults = new(hashResults);
            this.hashResultItems = new();
            foreach (var i in hashResults)
            {
                this.hashResultItems.Add(i);
            }
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

            #region 初始化 Command
            ShowSelectedCommand = new RelayCommand<int>(ShowSelected);
            ViewInExplorerCommand = new RelayCommand<string>(ViewInExplorer);
            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);
            #endregion
        }

        #region Fields

        private HashResultModel hashResult;
        private ObservableCollection<HashResultModel> hashResultItems;
        private List<HashResultModel> hashAllResults;
        private string? hashResultListBoxColWidth;
        private Visibility hashResultListBoxVisibility;
        private bool isLowerCase = Properties.Settings.Default.IsLowerCase;

        #endregion

        #region Public Properties/Commands

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
        public string? HashResultListBoxColWidth
        {
            get => hashResultListBoxColWidth;
            set => SetProperty(ref hashResultListBoxColWidth, value);
        }
        public Visibility HashResultListBoxVisibility
        {
            get => hashResultListBoxVisibility;
            set => SetProperty(ref hashResultListBoxVisibility, value);
        }
        public bool IsLowerCase
        {
            get => isLowerCase;
            set
            {
                SetProperty(ref isLowerCase, value);
                Properties.Settings.Default.IsLowerCase = value;
                Properties.Settings.Default.Save();

                // 刷新界面内容
                string str;
                foreach (var i in HashResult.Items)
                {
                    str = i.Value;
                    i.Value = string.Empty;
                    i.Value = str;
                }
            }
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
