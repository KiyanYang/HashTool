using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using HashTool.ViewModel;

namespace HashTool.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private HashAndProgress? _hashAndProgress;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region 输入行
        private void comboBoxInputMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (sender is ComboBox comboBox)
                {
                    if (simplePanelFiles != null)
                    {
                        if (((ComboBoxItem)comboBox.SelectedItem).Content is "文件夹")
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
                }
            });
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
            if (comboBoxInputMode.Text == "文件夹" || comboBoxInputMode.Text == "文本")
            {
                HandyControl.Controls.MessageBox.Info(string.Join($"注意：当前输入模式为“{comboBoxInputMode.Text}”，",
                    "如果要计算文件请在计算前将模式选择“文件”。"));
            }
            OpenFileDialog dialog = new OpenFileDialog();  //选择文件文件对话框
            dialog.Multiselect = false;  //是否支持多个文件的打开？
            dialog.Title = "请选择文件";  //标题
            dialog.Filter = "文件(*.*)|*.*";  //文件类型
            if (dialog.ShowDialog() == true)
            {
                textBoxInput.Text = dialog.FileName;  //获取文件路径
            }
        }
        #endregion

        #region 对比行
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
        #endregion

        #region 按钮行
        private void ButtonSaveHistory_Click(object sender, RoutedEventArgs e)
        {
            if (HashResultAllHistory.allHistory.Count > 0)
            {
                SaveResult(HashResultAllHistory.allHistory);
            }
            else
            {
                HandyControl.Controls.MessageBox.Warning("暂无历史记录！");
            }
        }
        private void ButtonSaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (_hashAndProgress != null)
            {
                SaveResult(_hashAndProgress.GetFormatterHashResult());
            }
            else
            {
                HandyControl.Controls.MessageBox.Warning("暂无计算结果！");
            }
        }
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (progressBarFile.Value > progressBarFile.Minimum && progressBarFile.Value < progressBarFile.Maximum)
            {
                return;
            }

            InputValue inputValue = GetInputValue();

            if (comboBoxInputMode.Text == "文件夹")
            {
                _hashAndProgress = new(Dispatcher, inputValue, progressBarFile, progressBarFiles);
            }
            else
            {
                _hashAndProgress = new(Dispatcher, inputValue, progressBarFile, progressBarFiles);
            }
        }
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            if (progressBarFile.Value > progressBarFile.Minimum && progressBarFile.Value < progressBarFile.Maximum)
            {
                switch ((string)buttonRest.Content)
                {
                    case "暂停":
                        if (_hashAndProgress is not null)
                        {
                            _hashAndProgress.HashReset();
                            buttonRest.Content = "继续";
                            buttonRest.SetResourceReference(StyleProperty, "ButtonSuccess");
                        }
                        break;
                    case "继续":
                        if (_hashAndProgress is not null)
                        {
                            _hashAndProgress.HashSet();
                            buttonRest.Content = "暂停";
                            buttonRest.SetResourceReference(StyleProperty, "ButtonPrimary");
                        }
                        break;
                }
            }
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (progressBarFile.Value > progressBarFile.Minimum && progressBarFile.Value < progressBarFile.Maximum)
            {
                if (_hashAndProgress != null)
                {
                    _hashAndProgress.HashCancel();
                    _hashAndProgress = null;
                }

                progressBarFile.Value = progressBarFile.Minimum;
                progressBarFiles.Value = progressBarFiles.Minimum;
                buttonRest.Content = "暂停";
                buttonRest.SetResourceReference(StyleProperty, "ButtonPrimary");
                HandyControl.Controls.Growl.SuccessGlobal("已取消！");
            }
        }
        private void ButtonViewResult_Click(object sender, RoutedEventArgs e)
        {
            if (_hashAndProgress != null)
            {
                if (_hashAndProgress.mode == "文件夹")
                {
                    FolderWindow folderWindow = new(_hashAndProgress);
                    folderWindow.Show();
                }
                else
                {
                    StringAndFileResultWindow stringAndFileResultWindow = new(_hashAndProgress);
                    stringAndFileResultWindow.Show();
                }
            }
            else
            {
                HandyControl.Controls.MessageBox.Warning("暂无计算结果！");
            }
        }
        #endregion

        private InputValue GetInputValue()
        {
            InputValue inputValue = new();

            Dispatcher.Invoke(() =>
            {
                inputValue.inputMode = comboBoxInputMode.Text;
                if (inputValue.inputMode == "文本")
                {
                    inputValue.input = textBoxInput.Text;
                }
                else
                {
                    inputValue.input = textBoxInput.Text.Trim();
                }
                inputValue.isCheckedDict.Add("MD5", checkBoxMD5.IsChecked);
                inputValue.isCheckedDict.Add("CRC32", checkBoxCRC32.IsChecked);
                inputValue.isCheckedDict.Add("SHA1", checkBoxSHA1.IsChecked);
                inputValue.isCheckedDict.Add("SHA256", checkBoxSHA256.IsChecked);
                inputValue.isCheckedDict.Add("SHA384", checkBoxSHA384.IsChecked);
                inputValue.isCheckedDict.Add("SHA512", checkBoxSHA512.IsChecked);
            });
            return inputValue;
        }

        private static void SaveResult(List<Dictionary<string, string>> result)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.FileName = "Hash 结果";
            saveFileDialog.Filter = "yaml 文件 (*.yaml)|*.yaml|json 文件 (*.json)|*.json|文本文件 (*.txt)|*.txt";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        Serializer.Yaml(saveFileDialog.FileName, result);
                        break;
                    case 2:
                        Serializer.Json(saveFileDialog.FileName, result);
                        break;
                    case 3:
                        Serializer.Text(saveFileDialog.FileName, result);
                        break;
                }
            }
        }
    }
}
