﻿using System.Windows.Controls;

using HashTool.ViewModels;

namespace HashTool.Views.Pages
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();

            AboutPageViewModel aboutViewModel = new();

            DataContext = aboutViewModel;
        }
    }
}
