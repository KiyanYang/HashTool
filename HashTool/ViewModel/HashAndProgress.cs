using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

using HashTool.Model;

namespace HashTool.ViewModel
{
    public class HashAndProgress
    {
        // 辅助计算
        private BackgroundWorker _bgWorkerMain = new();
        private CancellationTokenSource _cancellationTokenSource = new();
        private ManualResetEvent _resetEvent = new(true);
        // 输入相关
        private Dispatcher _dispatcher;
        private InputValue _inputValue;
        private ProgressBar _progressBarFile;
        private ProgressBar _progressBarFiles;
        // 计算内部结果
        private Dictionary<string, Dictionary<string, string>> _hashResults = new();
        private FileInfo[]? _files;
        // 计算拓展结果
        public string mode;  // 计算模式; 构造函数赋值
        public bool isValid = false;  // 是否有效, 即是否可以完成计算; HashCompute()内赋值
        public bool isComplete = false;  // 是否完成计算; RunWorkerCompleted() 内赋值
        // 实际显示结果
        public HashResultCore hashResult = new();  // 显示结果; 动态显示
        public string folderPath = string.Empty;  // 文件夹路径 仅文件夹模式下有效; HashCompute()内赋值
        public ObservableCollection<HashResultFileName> fileNames = new(); // List<Name> 仅文件夹模式下有效; BulidHashResult()内赋值

        public HashAndProgress(Dispatcher dp, InputValue inputVal, ProgressBar pBFile, ProgressBar pBFiles)
        {
            _dispatcher = dp;
            _inputValue = inputVal;
            _progressBarFile = pBFile;
            _progressBarFiles = pBFiles;
            mode = inputVal.inputMode;
            HashCompute();
        }

        public void HashCompute()
        {
            _bgWorkerMain.WorkerReportsProgress = true;
            switch (_inputValue.inputMode)
            {
                case "文件":
                    if (File.Exists(_inputValue.input))
                    {
                        isValid = true;
                        _bgWorkerMain.DoWork += new DoWorkEventHandler(bgWorkerMain_DoWork_File);
                        break;
                    }
                    else
                    {
                        isValid = false;
                        HandyControl.Controls.MessageBox.Error("文件读取错误！\n请检查路径是否错误或文件是否存在。");
                        return;
                    }
                case "文件夹":
                    if (Directory.Exists(_inputValue.input))
                    {
                        _files = new DirectoryInfo(_inputValue.input).GetFiles();
                        if (_files.Length > 0)
                        {
                            isValid = true;
                            _bgWorkerMain.DoWork += new DoWorkEventHandler(bgWorkerMain_DoWork_Folder);
                            folderPath = _inputValue.input;
                            break;
                        }
                        else
                        {
                            isValid = false;
                            HandyControl.Controls.MessageBox.Error("文件夹内无文件。");
                            return;
                        }
                    }
                    else
                    {
                        isValid = false;
                        HandyControl.Controls.MessageBox.Error("文件夹读取错误！\n请检查路径是否错误或文件夹是否存在。");
                        return;
                    }
                case "文本":
                    isValid = true;
                    _bgWorkerMain.DoWork += new DoWorkEventHandler(bgWorkerMain_DoWork_String);
                    break;
            }
            _bgWorkerMain.ProgressChanged += new ProgressChangedEventHandler(bgWorkerMain_ProgressChanged);
            _bgWorkerMain.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorkerMain_RunWorkerCompleted);
            _bgWorkerMain.RunWorkerAsync();
        }
        public void HashReset()
        {
            _resetEvent.Reset();
        }
        public void HashSet()
        {
            _resetEvent.Set();
        }
        public void HashCancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public void SetResult(string fileName)
        {
            if (_hashResults.TryGetValue(fileName, out _))
            {
                Common.SetProperties(hashResult, _hashResults[fileName]);
            }
        }
        public List<Dictionary<string, string>> GetFormatterHashResult()
        {
            return FormatHashResult.BuildHashResult(_hashResults);
        }

        private void bgWorkerMain_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                _dispatcher.Invoke(new Action(() =>
                {
                    _progressBarFile.Value = 0;
                    _progressBarFiles.Value = 0;
                }));
            }
            else
            {
                isComplete = true;

                if (mode == "文件夹")
                {
                    Common.SetProperties(hashResult, _hashResults[fileNames[0].FullName]);
                }
                else
                {
                    Common.SetProperties(hashResult, _hashResults[_inputValue.input]);
                }

                foreach (Dictionary<string, string> val in _hashResults.Values)
                {
                    HashResultCore hashResultCore = new();
                    Common.SetProperties(hashResultCore, val);
                }

                HashResultAllHistory.allHistory.AddRange(FormatHashResult.BuildHashResult(_hashResults));
            }
        }
        private void bgWorkerMain_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(() =>
            {
                _progressBarFile.Value = (e.ProgressPercentage - 1) % 1000 + 1;
                _progressBarFiles.Value = Math.Floor(e.ProgressPercentage / 1000.0);
            }));
        }
        private void bgWorkerMain_DoWork_String(object? sender, DoWorkEventArgs e)
        {
            using Task<Dictionary<string, string>> hashTask = Task.Run(
                () => Compute.HashString(_inputValue));
            BulidHashResult(hashTask.Result);
        }
        private void bgWorkerMain_DoWork_File(object? sender, DoWorkEventArgs e)
        {
            _dispatcher.Invoke(new Action(() =>
            {
                _progressBarFile.Maximum = 1000.0;
            }));
            using Task<Dictionary<string, string>> hashTask = Task.Run(
                () => Compute.HashFile(_resetEvent, _cancellationTokenSource.Token, _inputValue, _bgWorkerMain, 1000.0));
            BulidHashResult(hashTask.Result);
        }
        private void bgWorkerMain_DoWork_Folder(object? sender, DoWorkEventArgs e)
        {
            _dispatcher.Invoke(new Action(() =>
            {
                _progressBarFile.Maximum = 1000.0;
                if (_files != null)
                {
                    _progressBarFiles.Maximum = _files.Length;
                }
            }));
            using Task<Dictionary<string, Dictionary<string, string>>> hashTask = Task.Run(
                () => Compute.HashFolder(_resetEvent, _cancellationTokenSource.Token, _inputValue, _bgWorkerMain, 1000.0));
            BulidHashResult(hashTask.Result);
        }

        private void BulidHashResult(Dictionary<string, string> hashResult)
        {
            hashResult.Add("InputMode", _inputValue.inputMode);
            hashResult.Add("Input", _inputValue.input);

            if (_inputValue.inputMode == "文件")
            {
                FileInfo fileInfo = new(_inputValue.input);
                hashResult.Add("FileSize", fileInfo.Length.ToString());
                hashResult.Add("LastWriteTime", fileInfo.LastWriteTime.ToString());
            }
            _hashResults.Add(hashResult["Input"], hashResult);
        }
        private void BulidHashResult(Dictionary<string, Dictionary<string, string>> hashResults)
        {
            foreach (var kvp in hashResults)
            {
                FileInfo fileInfo = new(kvp.Key);
                kvp.Value.Add("InputMode", "文件");
                kvp.Value.Add("Input", kvp.Key);
                kvp.Value.Add("FileSize", fileInfo.Length.ToString());
                kvp.Value.Add("LastWriteTime", fileInfo.LastWriteTime.ToString());
                _hashResults.Add(kvp.Key, kvp.Value);
                HashResultFileName hashResultFileName = new();
                hashResultFileName.FileName = fileInfo.Name;
                hashResultFileName.FullName = fileInfo.FullName;
                _dispatcher.Invoke(new Action(() =>
                {
                    fileNames.Add(hashResultFileName);
                }));
            }
        }
    }
}
