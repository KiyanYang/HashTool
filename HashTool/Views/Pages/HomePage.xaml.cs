using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using HashTool.ViewModels;

using Microsoft.Win32;

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

        public HomePageViewModel HomeViewModel { get => homeViewModel; }

        private void comboBoxInputMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                Dispatcher.Invoke(() =>
                {
                    if (simplePanelFiles != null)
                    {
                        if (comboBox.SelectedItem is "文件夹")
                        {
                            simplePanelFiles.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            simplePanelFiles.Visibility = Visibility.Collapsed;
                        }
                        progressBarFile.Value = progressBarFile.Minimum;
                        progressBarFiles.Value = progressBarFiles.Minimum;
                    }
                });
            }
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
                string[] path = (string[])e.Data.GetData(DataFormats.FileDrop);
                Dispatcher.Invoke(new Action(() =>
                {
                    textBoxInput.Text = path[0];
                }));
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message);
            }
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxInputMode.Text == "文件夹")
            {
                HandyControl.Controls.Growl.InfoGlobal("选择文件夹内的任意文件来获取文件夹的地址");
            }
            OpenFileDialog dialog = new OpenFileDialog();  //选择文件文件对话框
            dialog.Multiselect = false;  //是否支持多个文件的打开？
            dialog.Title = "请选择文件";  //标题
            dialog.Filter = "文件(*.*)|*.*";  //文件类型
            if (dialog.ShowDialog() == true)
            {
                if (comboBoxInputMode.Text == "文件夹")
                {
                    textBoxInput.Text = Path.GetDirectoryName(dialog.FileName); // 获取文件夹路径
                }
                else
                {
                    textBoxInput.Text = dialog.FileName;  // 获取文件路径
                }
            }
        }

        private void TextBoxVerify_TextChanged(object sender, TextChangedEventArgs e)
        {
            string hash1 = textBoxVerify1.Text.Trim();
            string hash2 = textBoxVerify2.Text.Trim();
            if (hash1 == string.Empty || hash2 == string.Empty)
            {
                badgeVerify.ShowBadge = false;
            }
            else
            {
                badgeVerify.ShowBadge = true;
                if (string.Equals(hash1, hash2, StringComparison.OrdinalIgnoreCase))
                {
                    badgeVerify.Text = "相同";
                    badgeVerify.SetResourceReference(StyleProperty, "BadgeSuccess");
                }
                else
                {
                    badgeVerify.Text = "不相同";
                    badgeVerify.SetResourceReference(StyleProperty, "BadgeDanger");
                }
            }
        }
    }
}
