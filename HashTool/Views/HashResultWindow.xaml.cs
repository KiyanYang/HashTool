using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        private void Frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                var content = frame.Content as FrameworkElement;
                if (content != null)
                {
                    content.DataContext = DataContext;
                }
            }
        }
    }
}
