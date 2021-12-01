using System.Windows;
using System.Windows.Controls;

namespace HashTool.View
{
    /// <summary>
    /// HashResultPage.xaml 的交互逻辑
    /// </summary>
    public partial class HashResultPage : Page
    {
        public HashResultPage()
        {
            InitializeComponent();
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
