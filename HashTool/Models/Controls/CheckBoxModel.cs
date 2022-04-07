// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class CheckBoxModel : ObservableObject
    {
        public CheckBoxModel() { }
        public CheckBoxModel(string? content, bool isEnabled = true)
        {
            this.content = content;
            isChecked = isEnabled;
        }

        private string? content;
        private bool? isChecked;

        public string Content
        {
            get => content ??= string.Empty;
            set => SetProperty(ref content, value);
        }
        public bool? IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }
    }
}
