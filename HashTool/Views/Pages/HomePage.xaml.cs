using System;
using System.Windows;
using System.Windows.Controls;

using HashTool.ViewModels;

namespace HashTool.Views.Pages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            homeViewModel = new();
            DataContext = homeViewModel;
        }

        private HomePageViewModel homeViewModel;

        public HomePageViewModel HomeViewModel
        {
            get => homeViewModel;
        }

        private void TextBoxInput_PreviewDragOver(object sender, DragEventArgs e)  //不能使用PreviewDragEnter, 否则在TextBox内无法捕获数据
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
                e.Handled = true;  //不清楚此语句是如何工作的, 但是如果没有则不会调用Drop, 可能对拖放事件级别和高级别的输入事件争用处理
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TextBoxInput_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                var path = (string[])e.Data.GetData(DataFormats.FileDrop);
                homeViewModel.MainInput.Input = path[0];
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message);
            }
        }
    }
}
