// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class BadgeModel : ObservableObject
    {
        public BadgeModel() { }
        public BadgeModel(string text, bool showBadge = true)
        {
            _text = text;
            _showBadge = showBadge;
        }

        private string? _text;
        private bool _showBadge;
        private Style? _style;

        public string Text
        {
            get => _text ??= string.Empty;
            set => SetProperty(ref _text, value);
        }
        public bool ShowBadge
        {
            get => _showBadge;
            set => SetProperty(ref _showBadge, value);
        }
        public Style? Style
        {
            get => _style;
            set => SetProperty(ref _style, value);
        }

        public bool SetStyle(string name)
        {
            if (Application.Current.TryFindResource(name) is Style s)
            {
                Style = s;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
