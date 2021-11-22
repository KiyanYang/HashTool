using System.Windows;
using System.Windows.Controls;

using HashTool.ViewModel;

namespace HashTool.View
{
    /// <summary>
    /// StringAndFileResultWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StringAndFileResultWindow : Window
    {
        public StringAndFileResultWindow(HashAndProgress hashAndProgress)
        {
            InitializeComponent();
            this.DataContext = hashAndProgress.hashResult;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Clipboard.SetText((string)(button.Tag));
                HandyControl.Controls.Growl.SuccessGlobal("已复制到剪贴板！");
            }
        }
    }
}
