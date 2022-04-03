// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class ButtonModel : ObservableObject
    {
        public ButtonModel() { }
        public ButtonModel(string content, bool isEnabled = true)
        {
            this.content = content;
            this.isEnabled = isEnabled;
        }

        private string? content;
        private bool isEnabled;

        public string Content
        {
            get => content ??= string.Empty;
            set => SetProperty(ref content, value);
        }
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }
    }
}
