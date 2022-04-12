// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Input;

using HashTool.Common;
using HashTool.Helpers;
using HashTool.Models;
using HashTool.Models.Controls;
using HashTool.Views;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;

namespace HashTool.ViewModels
{
    public class HomePageViewModel : ObservableObject
    {
        public HomePageViewModel()
        {
            hashInput = new();
            // 初始化哈希算法 CheckBox
            var selectedHashAlgorithm = Properties.Settings.Default.SelectedHashAlgorithm;
            foreach (var name in new HashAlgorithmNames())
            {
                hashInput.CheckBoxItems.Add(new(name, selectedHashAlgorithm.Contains(name)));
            }

            BrowserDialogCommand = new RelayCommand(BrowserDialog);
            ShowResultCommand = new RelayCommand<List<HashResultModel>>(ShowResult);
            SaveResultCommand = new RelayCommand<List<HashResultModel>>(SaveResult);
            StartCommand = new RelayCommand(Start);
            ResetCommand = new RelayCommand(Reset);
            CancelCommand = new RelayCommand(Cancel);
            SetSelectedHashAlgorithmCommand = new RelayCommand(SetSelectedHashAlgorithm);

            InitializeBackgroundWorker();
        }

        #region Fields & Private Properties

        private bool? isTextMode;
        private string? hashValueVerify1;
        private string? hashValueVerify2;

        private BadgeModel? badgeVerify;
        private ButtonModel? buttonStart;
        private ButtonModel? buttonReset;
        private ButtonModel? buttonCancel;
        private HashInputModel hashInput;
        private ProgressBarModel? progressBarSingle;
        private ProgressBarModel? progressBarMulti;
        private ProgressBarModel? taskbarProgress;
        private List<HashResultModel>? hashResults;
        private List<HashResultModel>? hashResultHistory;

        private readonly BackgroundWorker bgWorker = new();
        private readonly ManualResetEventSlim mres = new(true);

        #endregion

        #region Public Properties/Commands

        /// <summary>
        /// 是否是文本模式
        /// </summary>
        public bool IsTextMode
        {
            get => isTextMode ??= false;
            set => SetProperty(ref isTextMode, value);
        }
        public string HashValueVerify1
        {
            get => hashValueVerify1 ??= string.Empty;
            set
            {
                SetProperty(ref hashValueVerify1, value.Trim());
                VerifyHashValue();
            }
        }
        public string HashValueVerify2
        {
            get => hashValueVerify2 ??= string.Empty;
            set
            {
                SetProperty(ref hashValueVerify2, value.Trim());
                VerifyHashValue();
            }
        }

        public BadgeModel BadgeVerify
        {
            get => badgeVerify ??= new(string.Empty, false);
            set => SetProperty(ref badgeVerify, value);
        }
        public ButtonModel ButtonStart
        {
            get => buttonStart ??= new("开始");
            set => SetProperty(ref buttonStart, value);
        }
        public ButtonModel ButtonReset
        {
            get => buttonReset ??= new("暂停", false);
            set => SetProperty(ref buttonReset, value);
        }
        public ButtonModel ButtonCancel
        {
            get => buttonCancel ??= new("取消", false);
            set => SetProperty(ref buttonCancel, value);
        }
        public HashInputModel HashInput
        {
            get => hashInput;
            set => SetProperty(ref hashInput, value);
        }
        public string HashInputMode
        {
            get => HashInput.Mode;
            set
            {
                HashInput.Mode = value;
                IsTextMode = value == "文本";
            }
        }
        public ProgressBarModel ProgressBarSingle
        {
            get => progressBarSingle ??= new(minimum: 0, maximum: 1000);
            set => SetProperty(ref progressBarSingle, value);
        }
        public ProgressBarModel ProgressBarMulti
        {
            get => progressBarMulti ??= new(minimum: 0, maximum: 1);
            set => SetProperty(ref progressBarMulti, value);
        }
        public ProgressBarModel TaskbarProgress
        {
            get => taskbarProgress ??= new(minimum: 0, maximum: 1);
            set => SetProperty(ref taskbarProgress, value);
        }
        public List<HashResultModel> HashResults
        {
            get => hashResults ??= new();
            set => SetProperty(ref hashResults, value);
        }
        public List<HashResultModel> HashResultHistory
        {
            get => hashResultHistory ??= new();
        }

        public ICommand BrowserDialogCommand { get; }
        public ICommand ShowResultCommand { get; }
        public ICommand SaveResultCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetSelectedHashAlgorithmCommand { get; }

        #endregion

        #region Helper

        private void BrowserDialog()
        {
            if (HashInput.Mode == HashInputModel.ModeItems[0])
            {
                OpenFileDialog dialog = new();  //选择文件文件对话框
                dialog.Multiselect = false;  //是否支持多个文件的打开？
                dialog.Title = "请选择文件";  //标题
                dialog.Filter = "文件(*.*)|*.*";  //文件类型
                if (dialog.ShowDialog() == true)
                {
                    // 获取文件路径
                    HashInput.Input = dialog.FileName;
                }
            }
            else if (HashInput.Mode == HashInputModel.ModeItems[1])
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // 获取文件路径
                    HashInput.Input = dialog.SelectedPath;
                }
            }
        }
        private void ShowResult(List<HashResultModel>? results)
        {
            if (results != null && results.Count > 0)
            {
                HashResultWindow hashResultWindow = new(results);
                hashResultWindow.Show();
            }
            else
            {
                HandyControl.Controls.MessageBox.Warning("暂无计算结果！");
            }
        }
        private void SaveResult(List<HashResultModel>? results)
        {
            if (results != null && results.Count > 0)
            {
                SaveFileDialog saveFileDialog = new();

                saveFileDialog.FileName = $"HashTool 结果_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
                saveFileDialog.Filter = "YAML (*.yaml)|*.yaml|JSON (*.json)|*.json|纯文本 (*.txt)|*.txt|XML (*.xml)|*.xml";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    var res = SerializerHelper.BuildHashResult(results);
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            SerializerHelper.Yaml(saveFileDialog.FileName, res);
                            break;
                        case 2:
                            SerializerHelper.Json(saveFileDialog.FileName, res);
                            break;
                        case 3:
                            SerializerHelper.Text(saveFileDialog.FileName, res);
                            break;
                        case 4:
                            SerializerHelper.Xml(saveFileDialog.FileName, res);
                            break;
                    }
                }
            }
            else
            {
                HandyControl.Controls.MessageBox.Warning("暂无计算结果！");
            }
        }
        private void Start()
        {
            if (!bgWorker.IsBusy)
            {
                mres.Set();
                HashResults.Clear();
                bgWorker.RunWorkerAsync(HashInput);
                ButtonStart.IsEnabled = false;
                ButtonReset.IsEnabled = true;
                ButtonCancel.IsEnabled = true;
            }
        }
        private void Reset()
        {
            if (bgWorker.IsBusy)
            {
                switch (ButtonReset.Content)
                {
                    case "暂停":
                        ButtonReset.Content = "继续";
                        mres.Reset();
                        break;
                    case "继续":
                        ButtonReset.Content = "暂停";
                        mres.Set();
                        break;
                }
            }
        }
        private void Cancel()
        {
            if (bgWorker.IsBusy)
            {
                mres.Set();
                bgWorker.CancelAsync();
                ButtonReset.Content = "暂停";
            }
        }
        private void VerifyHashValue()
        {
            if (string.IsNullOrEmpty(HashValueVerify1) || string.IsNullOrEmpty(HashValueVerify2))
            {
                BadgeVerify.ShowBadge = false;
            }
            else
            {
                BadgeVerify.ShowBadge = true;
                if (string.Equals(HashValueVerify1, HashValueVerify2, StringComparison.OrdinalIgnoreCase))
                {
                    BadgeVerify.Text = "相同";
                    BadgeVerify.SetStyle("BadgeSuccess");
                }
                else
                {
                    BadgeVerify.Text = "不相同";
                    BadgeVerify.SetStyle("BadgeDanger");
                }
            }
        }
        /// <summary>
        /// 设置 Properties.Settings.Default 中的 SelectedHashAlgorithm。
        /// </summary>
        private void SetSelectedHashAlgorithm()
        {
            var setting = Properties.Settings.Default;
            setting.SelectedHashAlgorithm.Clear();
            foreach (var i in HashInput.CheckBoxItems)
            {
                if (i.IsChecked == true)
                    setting.SelectedHashAlgorithm.Add(i.Content);
            }
            setting.Save();
        }

        #region BackgroundWorker

        private void InitializeBackgroundWorker()
        {
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
        }
        private void bgWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            ProgressBarSingle.Value = ProgressBarSingle.Minimum;
            ProgressBarMulti.Value = ProgressBarMulti.Minimum;
            TaskbarProgress.Value = TaskbarProgress.Minimum;
            if (sender is BackgroundWorker worker && e.Argument is HashInputModel input)
            {
                switch (input.Mode)
                {
                    case "文本":
                        ProgressBarMulti.Maximum = 1;
                        TaskbarProgress.Maximum = 1;
                        e.Result = ComputeHashHelper.HashString(input, worker, ProgressBarSingle.Maximum);
                        break;
                    case "文件":
                        if (File.Exists(input.Input.Trim()))
                        {
                            input.Input = input.Input.Trim();
                            ProgressBarMulti.Maximum = 1;
                            TaskbarProgress.Maximum = 1;
                            e.Result = ComputeHashHelper.HashFile(mres, worker, e, input, ProgressBarSingle.Maximum);
                            break;
                        }
                        else
                        {
                            HandyControl.Controls.MessageBox.Error("文件读取错误！\n请检查路径是否错误或文件是否存在。");
                            return;
                        }
                    case "文件夹":
                        if (Directory.Exists(input.Input.Trim()))
                        {
                            input.Input = input.Input.Trim();
                            int length = new DirectoryInfo(input.Input).GetFiles().Length;
                            if (length > 0)
                            {
                                ProgressBarMulti.Maximum = length;
                                TaskbarProgress.Maximum = length;
                                e.Result = ComputeHashHelper.HashFolder(mres, worker, e, input, ProgressBarSingle.Maximum);
                                break;
                            }
                            else
                            {
                                HandyControl.Controls.MessageBox.Error("文件夹内无文件。");
                                return;
                            }
                        }
                        else
                        {
                            HandyControl.Controls.MessageBox.Error("文件夹读取错误！\n请检查路径是否错误或文件夹是否存在。");
                            return;
                        }
                }
            }
        }
        private void bgWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                HandyControl.Controls.MessageBox.Error(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                ProgressBarSingle.Value = ProgressBarSingle.Minimum;
                ProgressBarMulti.Value = ProgressBarMulti.Minimum;
                TaskbarProgress.Value = TaskbarProgress.Minimum;
                HandyControl.Controls.Growl.SuccessGlobal("已取消！");
            }
            else
            {
                if (e.Result is HashResultModel result)
                {
                    HashResults.Add(result);
                    HashResultHistory.Add(result);
                }
                else if (e.Result is List<HashResultModel> results)
                {
                    HashResults.AddRange(results);
                    HashResultHistory.AddRange(results);
                }
            }
            ButtonStart.IsEnabled = true;
            ButtonReset.IsEnabled = false;
            ButtonCancel.IsEnabled = false;
        }
        private void bgWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            ProgressBarSingle.Value = (e.ProgressPercentage - 1) % 1000 + 1;
            ProgressBarMulti.Value = Math.Floor(e.ProgressPercentage / 1000.0);
            TaskbarProgress.Value = e.ProgressPercentage / 1000.0 / TaskbarProgress.Maximum;
        }

        #endregion BackgroundWorker

        #endregion
    }
}
