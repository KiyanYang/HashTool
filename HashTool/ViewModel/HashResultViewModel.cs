using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using HashTool.Model;
using HashTool.View;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HashTool.ViewModel
{
    public class HashResultViewModel : ObservableObject
    {
        public HashResultViewModel(List<HashResultModel> hashResults)
        {
            hashResult = hashResults[0];
            this.hashResults = hashResults;
            hashResultPage = new();
            if (hashResults.Count > 1)
            {
                hashResultListPage = new();
                hashResultListPageColWidth = "*";
                hashResultListPageVisibility = Visibility.Visible;
            }
            else
            {
                hashResultListPageColWidth = "Auto";
                hashResultListPageVisibility = Visibility.Collapsed;
            }
            ShowSelectedCommand = new RelayCommand<int>(ShowSelected);
        }

        #region Fields

        private HashResultModel hashResult;
        private List<HashResultModel> hashResults;
        private HashResultPage hashResultPage;
        private HashResultListPage? hashResultListPage;
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
        public HashResultPage HashResultPage
        {
            get => hashResultPage;
            set => SetProperty(ref hashResultPage, value);
        }
        public HashResultListPage? HashResultListPage
        {
            get => hashResultListPage;
            set => SetProperty(ref hashResultListPage, value);
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

        #endregion

        #region Private Helpers

        private void ShowSelected(int parameter)
        {
            if (parameter is int index)
                HashResult = HashResults[index];
        }

        #endregion
    }
}
