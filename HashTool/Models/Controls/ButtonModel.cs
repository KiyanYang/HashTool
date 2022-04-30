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
            _content = content;
            _isEnabled = isEnabled;
        }

        private string? _content;
        private bool _isEnabled;

        public string Content
        {
            get => _content ??= string.Empty;
            set => SetProperty(ref _content, value);
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
    }
}
