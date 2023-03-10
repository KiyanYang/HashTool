// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.Input;

using HashTool.Helpers;
using HashTool.Models;

namespace HashTool.ViewModels;

internal sealed partial class AboutPageViewModel : ObservableObject
{
    public AboutPageViewModel()
    {
    }

    private static readonly HttpClient s_httpClient = new();
    private static LatestVersion? s_latestVersionInfo;

    #region Public Properties

    public string AppVersion => RuntimeHelper.AppVersion.ToString();

    [ObservableProperty]
    private string _updateStatusText = string.Empty;

    public UpdateModel Update { get; } = GetInstance<UpdateModel>();

    #endregion

    #region Command

    [RelayCommand]
    private async Task CheckUpdateAsync()
    {
        Update.HasUpdate = false;
        UpdateStatusText = "检查更新中……";

        bool _hasLatestVersionInfo = false;
        try
        {
            string latestVersionUrl = Properties.Settings.Default.LatestVersionUrl;
            HttpResponseMessage result = await s_httpClient.GetAsync(latestVersionUrl);
            using Stream reader = result.Content.ReadAsStream();
            var serializer = new XmlSerializer(typeof(LatestVersion));
            s_latestVersionInfo = serializer.Deserialize(reader) as LatestVersion;
            _hasLatestVersionInfo = true;
        }
        catch (Exception)
        {
        }

        if (_hasLatestVersionInfo && s_latestVersionInfo != null)
        {
            Version appVersion = RuntimeHelper.AppVersion;
            var ver = new Version(s_latestVersionInfo.Version);
            if (ver > appVersion)
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
    }

    [RelayCommand]
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

    [RelayCommand]
    private void OpenLink(string? link)
    {
        if (link == null)
        {
            return;
        }

        Process.Start("explorer.exe", link);
    }

    [RelayCommand]
    private void OpenFile(string? path)
    {
        if (path == null)
        {
            return;
        }

        string fullPath = Path.GetFullPath(path);
        Process.Start("explorer.exe", fullPath);
    }

    #endregion Command
}

/// <summary>
/// LatestVersionXml 模型
/// </summary>
public sealed class LatestVersion
{
    public string Tag = string.Empty;
    public string Version = string.Empty;
    public string SHA256_zip = string.Empty;
    public string SHA256_7z = string.Empty;
    public string GiteeDownloadUrl = string.Empty;
    public string GithubDownloadUrl = string.Empty;
}
