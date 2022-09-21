// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using HashTool.Models;

namespace HashTool.ViewModels
{
    internal class AboutPageViewModel : ObservableObject
    {
        public AboutPageViewModel()
        {
            CheckUpdateCommand = new RelayCommand(CheckUpdate);
            UpdateCommand = new RelayCommand(OpenUpdater);
            OpenLinkCommand = new RelayCommand<string>(OpenLink);
            OpenFileCommand = new RelayCommand<string>(OpenFile);
        }

        #region Fields

        private static readonly HttpClient s_httpClient = new();
        private static LatestVersion? s_latestVersionInfo;

        private bool? _buttonCheckUpdateIsEnabled;
        private string? _assemblyVersion;
        private string? _updateStatusText;

        private UpdateModel? _update;

        #endregion

        #region Public Properties/Commands

        public bool ButtonCheckUpdateIsEnabled
        {
            get => _buttonCheckUpdateIsEnabled ??= true;
            set => SetProperty(ref _buttonCheckUpdateIsEnabled, value);
        }
        public string AssemblyVersion
        {
            get => _assemblyVersion ??= GetAssemblyVersion().ToString(3);
        }
        public string UpdateStatusText
        {
            get => _updateStatusText ??= string.Empty;
            set => SetProperty(ref _updateStatusText, value);
        }

        public UpdateModel Update
        {
            get => _update ??= GetInstance<UpdateModel>();
        }

        public ICommand CheckUpdateCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand OpenLinkCommand { get; }
        public ICommand OpenFileCommand { get; }

        #endregion

        #region Helper

        private async void CheckUpdate()
        {
            ButtonCheckUpdateIsEnabled = false;
            Update.HasUpdate = false;
            UpdateStatusText = "检查更新中……";

            if (await GetLatestVersion() && s_latestVersionInfo != null)
            {
                Version assemblyVer = GetAssemblyVersion();
                var ver = new Version(s_latestVersionInfo.Version);
                if (ver > assemblyVer)
                {
                    Update.HasUpdate = true;
                    Update.Version = s_latestVersionInfo.Tag;
                    Update.DownloadUrl = s_latestVersionInfo.GiteeDownloadUrl;
                    Update.GithubUrl = $"https://github.com/KiyanYang/HashTool/releases/tag/{Update.Version}";
                    Update.GiteeUrl = $"https://gitee.com/KiyanYang/HashTool/releases/{Update.Version}";
                }
                else
                {
                    UpdateStatusText = "当前为最新版本！";
                }
            }
            else
            {
                Update.HasUpdate = false;
                UpdateStatusText = "检查更新失败！";
            }
            ButtonCheckUpdateIsEnabled = true;
        }

        private static async Task<bool> GetLatestVersion()
        {
            try
            {
                string latestVersionUrl = Properties.Settings.Default.LatestVersionUrl;
                HttpResponseMessage result = await s_httpClient.GetAsync(latestVersionUrl);
                using Stream reader = result.Content.ReadAsStream();

                var serializer = new XmlSerializer(typeof(LatestVersion));
                s_latestVersionInfo = (LatestVersion?)serializer.Deserialize(reader);
                return true;
            }
            catch (Exception)
            {
                // 之后的版本写入日志
                return false;
            }
        }

        private static Version GetAssemblyVersion()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            AssemblyName assemName = assem.GetName();
            return assemName.Version ?? new Version(1, 0, 0, 0);
        }

        private void OpenUpdater()
        {
            MessageBoxResult res = HandyControl.Controls.MessageBox.Show("是否关闭程序并开始更新", "是否关闭并更新", MessageBoxButton.OKCancel);
            // 这里不要判断是否点击的是取消按钮，以防止未点击按钮（直接右上关闭消息框）的情况
            if (res != MessageBoxResult.OK)
            {
                return;
            }
            if (s_latestVersionInfo != null)
            {
                var process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                string updaterPs1 = Path.GetFullPath(@".\updater.ps1");
                string targetPath = Path.GetFullPath(@".\");
                string str = $"-ExecutionPolicy Bypass -File {updaterPs1} -Url {Update.DownloadUrl} -SHA256 {s_latestVersionInfo.SHA256_zip} -TargetPath {targetPath} -Force";
                process.StartInfo.Arguments = str;
                process.Start();
                Environment.Exit(-1);
            }
            else
            {
                HandyControl.Controls.MessageBox.Show("更新出现错误，请手动更新");
            }
        }

        private void OpenLink(string? link)
        {
            if (link == null)
            {
                return;
            }

            Process.Start("explorer.exe", link);
        }

        private void OpenFile(string? path)
        {
            if (path == null)
            {
                return;
            }

            string fullPath = Path.GetFullPath(path);
            Process.Start("explorer.exe", fullPath);
        }

        #endregion
    }

    /// <summary>
    /// LatestVersionXml 模型
    /// </summary>
    public class LatestVersion
    {
        public string Tag = string.Empty;
        public string Version = string.Empty;
        public string SHA256_zip = string.Empty;
        public string SHA256_7z = string.Empty;
        public string GiteeDownloadUrl = string.Empty;
        public string GithubDownloadUrl = string.Empty;
    }
}
