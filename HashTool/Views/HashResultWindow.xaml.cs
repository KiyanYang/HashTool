using System;
using System.Collections.Generic;
using System.Windows;

using HashTool.Models;
using HashTool.ViewModels;

namespace HashTool.Views
{
    /// <summary>
    /// HashResultWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HashResultWindow : Window
    {
        public HashResultWindow(List<HashResultModel> hashResultModels)
        {
            InitializeComponent();

            HashResultViewModel hashResultViewModel = new(hashResultModels);
            DataContext = hashResultViewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DataContext = null;
        }
    }
}
