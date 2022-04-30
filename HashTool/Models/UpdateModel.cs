// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    internal class UpdateModel : ObservableObject
    {
        private bool? _hasUpdate;
        private string? _version;
        private string? _downloadUrl;
        private string? _githubUrl;
        private string? _giteeUrl;

        public bool? HasUpdate
        {
            get => _hasUpdate;
            set => SetProperty(ref _hasUpdate, value);
        }
        public string? Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }
        public string? DownloadUrl
        {
            get => _downloadUrl;
            set => SetProperty(ref _downloadUrl, value);
        }
        public string? GithubUrl
        {
            get => _githubUrl;
            set => SetProperty(ref _githubUrl, value);
        }
        public string? GiteeUrl
        {
            get => _giteeUrl;
            set => SetProperty(ref _giteeUrl, value);
        }
    }
}
