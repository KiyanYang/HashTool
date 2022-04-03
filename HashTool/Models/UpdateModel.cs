// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    internal class UpdateModel : ObservableObject
    {
        private bool? hasUpdate;
        private string? version;
        private string? downloadUrl;
        private string? githubUrl;
        private string? giteeUrl;

        public bool? HasUpdate
        {
            get => hasUpdate;
            set => SetProperty(ref hasUpdate, value);
        }
        public string? Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }
        public string? DownloadUrl
        {
            get => downloadUrl;
            set => SetProperty(ref downloadUrl, value);
        }
        public string? GithubUrl
        {
            get => githubUrl;
            set => SetProperty(ref githubUrl, value);
        }
        public string? GiteeUrl
        {
            get => giteeUrl;
            set => SetProperty(ref giteeUrl, value);
        }
    }
}
