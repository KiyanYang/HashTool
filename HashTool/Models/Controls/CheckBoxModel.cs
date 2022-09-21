// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class CheckBoxModel : ObservableObject
    {
        public CheckBoxModel() { }
        public CheckBoxModel(string? content, bool isChecked = true)
        {
            _content = content;
            _isChecked = isChecked;
        }

        private string? _content;
        private bool? _isChecked;

        public string Content
        {
            get => _content ??= string.Empty;
            set => SetProperty(ref _content, value);
        }
        public bool? IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }
}
