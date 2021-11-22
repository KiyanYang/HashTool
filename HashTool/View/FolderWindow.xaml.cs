using System.Windows;
using System.Windows.Controls;

using HashTool.ViewModel;

namespace HashTool.View
{
    /// <summary>
    /// FolderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FolderWindow : Window
    {
        private HashAndProgress? _hashAndProgress;
        public FolderWindow(HashAndProgress hashAndProgress)
        {
            InitializeComponent();

            _hashAndProgress = hashAndProgress;
            textBoxFolderPath.Text = hashAndProgress.folderPath;
            listViewFileNames.ItemsSource = hashAndProgress.fileNames;
            simpleStackPanelFile.DataContext = hashAndProgress.hashResult;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Clipboard.SetText((string)(button.Tag));
                HandyControl.Controls.Growl.SuccessGlobal("已复制到剪贴板！");
            }
        }

        private void listViewFileNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (listView.SelectedItem is HashResultFileName hashResultFileName)
                {
                    if (_hashAndProgress != null)
                    {
                        _hashAndProgress.SetResult(hashResultFileName.FullName);
                    }
                }
            }
        }
    }
}
