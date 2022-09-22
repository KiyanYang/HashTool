// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

using CommunityToolkit.Mvvm.Input;

using HashTool.Helpers;
using HashTool.Helpers.Hashs;
using HashTool.Models;
using HashTool.Models.Controls;
using HashTool.Views;

using Microsoft.Win32;

namespace HashTool.ViewModels;

public sealed partial class HomePageViewModel : ObservableObject
{
    public HomePageViewModel()
    {
        _hashInput = new();
        // 初始化哈希算法 CheckBox
        StringCollection selectedHashAlgorithms = Properties.Settings.Default.SelectedHashAlgorithms;
        foreach (Hash hash in Hash.GetHashs())
        {
            _hashInput.CheckBoxItems.Add(new()
            {
                Content = hash.Name,
                IsChecked = selectedHashAlgorithms.Contains(hash.Name)
            });
        }

        InitializeBackgroundWorker();
    }

    #region Fields

    private string? _hashValueVerify1;
    private string? _hashValueVerify2;
    private List<HashResultModel>? _hashResults;
    private List<HashResultModel>? _hashResultHistory;

    private readonly BackgroundWorker _bgWorker = new();
    private readonly ManualResetEventSlim _mres = new(true);

    #endregion

    #region Public Properties/Commands

    /// <summary>
    /// 是否是文本模式。
    /// </summary>
    [ObservableProperty]
    private bool _isTextMode = false;

    /// <summary>
    /// 左侧校验框的值。（自动去除前后空白字符）
    /// </summary>
    public string HashValueVerify1
    {
        get => _hashValueVerify1 ??= string.Empty;
        set
        {
            SetProperty(ref _hashValueVerify1, value.Trim());
            VerifyHashValue();
        }
    }

    /// <summary>
    /// 右侧校验框的值。（自动去除前后空白字符）
    /// </summary>
    public string HashValueVerify2
    {
        get => _hashValueVerify2 ??= string.Empty;
        set
        {
            SetProperty(ref _hashValueVerify2, value.Trim());
            VerifyHashValue();
        }
    }

    /// <summary>
    /// 哈希校验的结果。
    /// </summary>
    [ObservableProperty]
    private BadgeModel _badgeVerify = new() { ShowBadge = false };

    /// <summary>
    /// 计算开始按钮。
    /// </summary>
    [ObservableProperty]
    private ButtonModel _buttonStart = new() { Content = "开始", IsEnabled = true };

    /// <summary>
    /// 计算暂停按钮。
    /// </summary>
    [ObservableProperty]
    private ButtonModel _buttonReset = new() { Content = "暂停", IsEnabled = true };

    /// <summary>
    /// 计算取消按钮。
    /// </summary>
    [ObservableProperty]
    private ButtonModel _buttonCancel = new() { Content = "取消", IsEnabled = true };

    /// <summary>
    /// 哈希计算所需的输入参数。
    /// </summary>
    [ObservableProperty]
    private HashInputModel _hashInput;

    /// <summary>
    /// 哈希计算的输入模式。
    /// </summary>
    public string HashInputMode
    {
        get => HashInput.Mode;
        set
        {
            HashInput.Mode = value;
            IsTextMode = value == "文本";
        }
    }

    /// <summary>
    /// 右侧当前对象的计算进度。
    /// </summary>
    [ObservableProperty]
    private ProgressBarModel _progressBarSingle = new(minimum: 0, maximum: 1000);

    /// <summary>
    /// 左侧总体对象的计算进度。
    /// </summary>
    [ObservableProperty]
    private ProgressBarModel _progressBarMulti = new(minimum: 0, maximum: 1);

    /// <summary>
    /// 任务栏显示的进度。
    /// </summary>
    [ObservableProperty]
    private ProgressBarModel _taskbarProgress = new(minimum: 0, maximum: 1);

    /// <summary>
    /// 当前结果。
    /// </summary>
    public List<HashResultModel> HashResults
    {
        get => _hashResults ??= new();
        set => SetProperty(ref _hashResults, value);
    }

    /// <summary>
    /// 历史结果。
    /// </summary>
    public List<HashResultModel> HashResultHistory
    {
        get => _hashResultHistory ??= new();
    }

    #endregion

    #region Command

    [RelayCommand]
    private void BrowserDialog()
    {
        if (HashInput.Mode == HashInputModel.ModeItems[0])
        {
            OpenFileDialog dialog = new()
            {
                Multiselect = false,  //是否支持多个文件的打开？
                Title = "请选择文件",  //标题
                Filter = "文件(*.*)|*.*"  //文件类型
            };  //选择文件文件对话框
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
                // 获取文件夹路径
                HashInput.Input = dialog.SelectedPath;
            }
        }
    }

    [RelayCommand]
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

    [RelayCommand]
    private void SaveResult(List<HashResultModel>? results)
    {
        if (results == null || results.Count <= 0)
        {
            HandyControl.Controls.MessageBox.Warning("暂无计算结果！");
            return;
        }

        SaveFileDialog saveFileDialog = new()
        {
            FileName = $"HashTool 结果_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}",
            Filter = "YAML (*.yaml)|*.yaml|JSON (*.json)|*.json|纯文本 (*.txt)|*.txt|XML (*.xml)|*.xml",
            RestoreDirectory = true
        };

        if (saveFileDialog.ShowDialog() != true)
        {
            return;
        }

        List<Dictionary<string, string>> res = SerializerHelper.BuildHashResult(results);
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

    [RelayCommand]
    private void Start()
    {
        if (!_bgWorker.IsBusy)
        {
            _mres.Set();
            HashResults.Clear();
            _bgWorker.RunWorkerAsync(HashInput);
            ButtonStart.IsEnabled = false;
            ButtonReset.IsEnabled = true;
            ButtonCancel.IsEnabled = true;
        }
    }

    [RelayCommand]
    private void Reset()
    {
        if (_bgWorker.IsBusy)
        {
            switch (ButtonReset.Content)
            {
                case "暂停":
                    ButtonReset.Content = "继续";
                    _mres.Reset();
                    break;
                case "继续":
                    ButtonReset.Content = "暂停";
                    _mres.Set();
                    break;
            }
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_bgWorker.IsBusy)
        {
            _mres.Set();
            _bgWorker.CancelAsync();
            ButtonReset.Content = "暂停";
        }
    }

    /// <summary>
    /// 设置 Properties.Settings.Default 中的 SelectedHashAlgorithm。
    /// </summary>
    [RelayCommand]
    private void SetSelectedHashAlgorithms()
    {
        Properties.Settings setting = Properties.Settings.Default;
        setting.SelectedHashAlgorithms.Clear();
        HashInput.CheckBoxItems.Where(i => i.IsChecked == true)
            .Select(i => i.Content)
            .ToList()
            .ForEach(i => setting.SelectedHashAlgorithms.Add(i));
        setting.Save();
    }

    #endregion Command

    #region Helper

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

    #region BackgroundWorker

    private void InitializeBackgroundWorker()
    {
        _bgWorker.WorkerReportsProgress = true;
        _bgWorker.WorkerSupportsCancellation = true;
        _bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
        _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
        _bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
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
                        e.Result = ComputeHashHelper.HashFile(_mres, worker, e, input, ProgressBarSingle.Maximum);
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
                            e.Result = ComputeHashHelper.HashFolder(_mres, worker, e, input, ProgressBarSingle.Maximum);
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
