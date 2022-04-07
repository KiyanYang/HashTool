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

using HashTool.Models;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

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

        private static readonly HttpClient httpClient = new();
        private static LatestVersion? latestVersionInfo;

        private bool? buttonCheckUpdateIsEnabled;
        private string? assemblyVersion;
        private string? updateStatusText;

        private UpdateModel? update;

        #endregion

        #region Public Properties/Commands

        public bool ButtonCheckUpdateIsEnabled
        {
            get => buttonCheckUpdateIsEnabled ??= true;
            set => SetProperty(ref buttonCheckUpdateIsEnabled, value);
        }
        public string AssemblyVersion
        {
            get => assemblyVersion ??= GetAssemblyVersion().ToString(3);
        }
        public string UpdateStatusText
        {
            get => updateStatusText ??= string.Empty;
            set => SetProperty(ref updateStatusText, value);
        }

        public UpdateModel Update
        {
            get => update ??= GetInstance<UpdateModel>();
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

            if (await GetLatestVersion() && latestVersionInfo != null)
            {
                var assemblyVer = GetAssemblyVersion();
                var ver = new Version(latestVersionInfo.Version);
                if (ver > assemblyVer)
                {
                    Update.HasUpdate = true;
                    Update.Version = latestVersionInfo.Tag;
                    Update.DownloadUrl = latestVersionInfo.GiteeDownloadUrl;
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
                var result = await httpClient.GetAsync(latestVersionUrl);
                using var reader = result.Content.ReadAsStream();

                var serializer = new XmlSerializer(typeof(LatestVersion));
                latestVersionInfo = (LatestVersion?)serializer.Deserialize(reader);
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
            return assemName.Version ?? new Version(1, 0, 0);
        }

        private void OpenUpdater()
        {
            var res = HandyControl.Controls.MessageBox.Show("是否关闭程序并开始更新", "是否关闭并更新", MessageBoxButton.OKCancel);
            // 这里不要使用相等判断，以防止未点击按钮（直接右上关闭消息框）的情况
            if (res != MessageBoxResult.OK)
                return;

            if (latestVersionInfo != null)
            {
                var process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                var updaterPs1 = Path.GetFullPath(@".\updater.ps1");
                var targetPath = Path.GetFullPath(@".\");
                string str = $"-ExecutionPolicy Bypass -File {updaterPs1} -Url {Update.DownloadUrl} -SHA256 {latestVersionInfo.SHA256_zip} -TargetPath {targetPath} -Force";
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
                return;

            Process.Start("explorer.exe", link);
        }

        private void OpenFile(string? path)
        {
            if (path == null)
                return;
            var fullPath = Path.GetFullPath(path);
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
