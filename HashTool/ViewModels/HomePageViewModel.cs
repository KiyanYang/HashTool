using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Input;

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
            buttonStart = new("开始");
            buttonReset = new("暂停", false);
            buttonCancel = new("取消", false);

            #region 初始化界面输入

            mainInput = new();
            var sAlgorithm = Properties.Settings.Default.SelectedHashAlgorithm;
            mainInput.CheckBoxItems.Add(new CheckBoxModel("MD5", sAlgorithm.Contains("MD5")));
            mainInput.CheckBoxItems.Add(new CheckBoxModel("CRC32", sAlgorithm.Contains("CRC32")));
            mainInput.CheckBoxItems.Add(new CheckBoxModel("SHA1", sAlgorithm.Contains("SHA1")));
            mainInput.CheckBoxItems.Add(new CheckBoxModel("SHA256", sAlgorithm.Contains("SHA256")));
            mainInput.CheckBoxItems.Add(new CheckBoxModel("SHA384", sAlgorithm.Contains("SHA384")));
            mainInput.CheckBoxItems.Add(new CheckBoxModel("SHA512", sAlgorithm.Contains("SHA512")));
            mainInput.CheckBoxItems.Add(new CheckBoxModel("QuickXor", sAlgorithm.Contains("QuickXor")));

            #endregion

            progressBarSingle = new(minimum: 0, maximum: 1000);
            progressBarMulti = new(minimum: 0, maximum: 1);
            taskbarProgress = new(minimum: 0, maximum: 1);

            bgWorker = new();
            resetEvent = new(true);
            hashResults = new();
            hashResultHistory = new();

            ShowResultCommand = new RelayCommand<List<HashResultModel>>(ShowResult);
            SaveResultCommand = new RelayCommand<List<HashResultModel>>(SaveResult);
            StartCommand = new RelayCommand(Start);
            ResetCommand = new RelayCommand(Reset);
            CancelCommand = new RelayCommand(Cancel);
            SetSelectedHashAlgorithmCommand = new RelayCommand(SetSelectedHashAlgorithm);

            InitializeBackgroundWorker();
        }

        #region Fields

        private ButtonModel buttonStart;
        private ButtonModel buttonReset;
        private ButtonModel buttonCancel;
        private HashInputModel mainInput;
        private ProgressBarModel progressBarSingle;
        private ProgressBarModel progressBarMulti;
        private ProgressBarModel taskbarProgress;

        private BackgroundWorker bgWorker;
        private ManualResetEvent resetEvent;
        private List<HashResultModel> hashResults;
        private List<HashResultModel> hashResultHistory;

        #endregion

        #region Public Properties/Commands

        public ButtonModel ButtonStart
        {
            get => buttonStart;
            set => SetProperty(ref buttonStart, value);
        }
        public ButtonModel ButtonReset
        {
            get => buttonReset;
            set => SetProperty(ref buttonReset, value);
        }
        public ButtonModel ButtonCancel
        {
            get => buttonCancel;
            set => SetProperty(ref buttonCancel, value);
        }
        public HashInputModel MainInput
        {
            get => mainInput;
            set => SetProperty(ref mainInput, value);
        }
        public ProgressBarModel ProgressBarSingle
        {
            get => progressBarSingle;
            set => SetProperty(ref progressBarSingle, value);
        }
        public ProgressBarModel ProgressBarMulti
        {
            get => progressBarMulti;
            set => SetProperty(ref progressBarMulti, value);
        }
        public ProgressBarModel TaskbarProgress
        {
            get => taskbarProgress;
            set => SetProperty(ref taskbarProgress, value);
        }
        public List<HashResultModel> HashResults
        {
            get => hashResults;
            set => SetProperty(ref hashResults, value);
        }
        public List<HashResultModel> HashResultHistory
        {
            get => hashResultHistory;
            set => SetProperty(ref hashResultHistory, value);
        }

        public ICommand ShowResultCommand { get; }
        public ICommand SaveResultCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetSelectedHashAlgorithmCommand { get; }

        #endregion

        #region Helper

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
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.FileName = "HashTool 结果_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
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
                resetEvent = new(true);
                hashResults.Clear();
                bgWorker.RunWorkerAsync(CommonHelper.DeepCopy(MainInput));
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
                        resetEvent.Reset();
                        break;
                    case "继续":
                        ButtonReset.Content = "暂停";
                        resetEvent.Set();
                        break;
                }
            }
        }
        private void Cancel()
        {
            if (bgWorker.IsBusy)
            {
                resetEvent.Set();
                bgWorker.CancelAsync();
                ButtonReset.Content = "暂停";
            }
        }
        private void SetSelectedHashAlgorithm()
        {
            var sAlgorithm = Properties.Settings.Default.SelectedHashAlgorithm;
            sAlgorithm.Clear();
            foreach (var i in MainInput.CheckBoxItems)
            {
                if (i.IsChecked == true)
                    sAlgorithm.Add(i.Content);
            }
            Properties.Settings.Default.Save();
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
            BackgroundWorker? worker = sender as BackgroundWorker;
            HashInputModel? input = e.Argument as HashInputModel;
            if (worker != null && input != null)
            {
                switch (input.Mode)
                {
                    case "文本":
                        e.Result = ComputeHashHelper.HashString(input, worker, progressBarSingle.Maximum);
                        break;
                    case "文件":
                        if (File.Exists(input.Input.Trim()))
                        {
                            input.Input = input.Input.Trim();
                            taskbarProgress.Maximum = 1;
                            e.Result = ComputeHashHelper.HashFile(resetEvent, worker, e, input, progressBarSingle.Maximum);
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
                                taskbarProgress.Maximum = length;
                                e.Result = ComputeHashHelper.HashFolder(resetEvent, worker, e, input, progressBarSingle.Maximum);
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
                progressBarSingle.Value = progressBarSingle.Minimum;
                progressBarMulti.Value = progressBarMulti.Minimum;
                taskbarProgress.Value = taskbarProgress.Minimum;
                HandyControl.Controls.Growl.SuccessGlobal("已取消！");
            }
            else
            {
                if (e.Result is HashResultModel result)
                {
                    hashResults.Add(result);
                    hashResultHistory.Add(result);
                }
                else if (e.Result is List<HashResultModel> results)
                {
                    hashResults.AddRange(results);
                    hashResultHistory.AddRange(results);
                }
            }
            ButtonStart.IsEnabled = true;
            ButtonReset.IsEnabled = false;
            ButtonCancel.IsEnabled = false;
        }
        private void bgWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressBarSingle.Value = (e.ProgressPercentage - 1) % 1000 + 1;
            progressBarMulti.Value = Math.Floor(e.ProgressPercentage / 1000.0);
            taskbarProgress.Value = e.ProgressPercentage / 1000.0 / taskbarProgress.Maximum;
        }

        #endregion BackgroundWorker

        #endregion
    }
}
