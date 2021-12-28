using System.Collections.Generic;
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
            hashResult = hashResults[0];
            this.hashResults = hashResults;
            if (hashResults.Count > 1)
            {
                hashResultListPageColWidth = "*";
                hashResultListPageVisibility = Visibility.Visible;
            }
            else
            {
                hashResultListPageColWidth = "Auto";
                hashResultListPageVisibility = Visibility.Collapsed;
            }
            ShowSelectedCommand = new RelayCommand<int>(ShowSelected);
            ViewInExplorerCommand = new RelayCommand<string>(ViewInExplorer);
            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);
        }

        #region Fields

        private HashResultModel hashResult;
        private List<HashResultModel> hashResults;
        private string? hashResultListPageColWidth;
        private Visibility hashResultListPageVisibility;

        #endregion

        #region Public Properties/Commands

        public HashResultModel HashResult
        {
            get => hashResult;
            set => SetProperty(ref hashResult, value);
        }
        public List<HashResultModel> HashResults
        {
            get => hashResults;
            set => SetProperty(ref hashResults, value);
        }
        public string? HashResultListPageColWidth
        {
            get => hashResultListPageColWidth;
            set => SetProperty(ref hashResultListPageColWidth, value);
        }
        public Visibility HashResultListPageVisibility
        {
            get => hashResultListPageVisibility;
            set => SetProperty(ref hashResultListPageVisibility, value);
        }
        public ICommand ShowSelectedCommand { get; }
        public ICommand ViewInExplorerCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        #endregion

        #region Private Helpers

        private void ShowSelected(int index)
        {
            if (index >= 0)
                HashResult = HashResults[index];
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
