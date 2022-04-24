// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

using HashTool.Models.Controls;
using HashTool.Views.Pages;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HashTool.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            ShowPageCommand = new RelayCommand<string>(ShowPage);
        }

        #region Fields

        private Page? _currentPage;
        private HomePage? _homePage;
        private HelpPage? _helpPage;
        private AboutPage? _aboutPage;

        #endregion

        #region Public Properties/Commands

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
        public ProgressBarModel TaskbarProgress
        {
            get => HomePage.TaskbarProgress;
        }
        public ICommand ShowPageCommand { get; }

        #endregion

        #region Helper

        private void ShowPage(string? title)
        {
            CurrentPage = title switch
            {
                "主页" => HomePage,
                "帮助" => HelpPage,
                "关于" => AboutPage,
                _ => HomePage,
            };
        }

        #endregion
    }
}
