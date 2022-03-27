using System.Windows.Input;

using HashTool.Models;
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
            page = new();
            homePage = new();
            helpPage = new();
            aboutPage = new();

            ShowPage("home");

            ShowPageCommand = new RelayCommand<string>(ShowPage);
        }

        #region Fields

        private PageModel page;
        private HomePage homePage;
        private HelpPage helpPage;
        private AboutPage aboutPage;

        #endregion

        #region Public Properties/Commands

        public PageModel Page
        {
            get => page;
            set => SetProperty(ref page, value);
        }
        public ProgressBarModel TaskbarProgress
        {
            get => homePage.HomeViewModel.TaskbarProgress;
        }
        public ICommand ShowPageCommand { get; }

        #endregion

        #region Helper

        private void ShowPage(string? pageName)
        {
            switch (pageName)
            {
                case "home":
                    page.Page = homePage;
                    page.Name = "主页";
                    break;

                case "help":
                    page.Page = helpPage;
                    page.Name = "帮助";
                    break;

                case "about":
                    page.Page = aboutPage;
                    page.Name = "关于";
                    break;
            }
        }

        #endregion
    }
}
