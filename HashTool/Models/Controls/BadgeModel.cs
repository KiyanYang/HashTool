// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows;

namespace HashTool.Models.Controls;

public sealed partial class BadgeModel : ObservableObject
{
    [ObservableProperty]
    private string? _text = string.Empty;

    [ObservableProperty]
    private bool _showBadge;

    [ObservableProperty]
    private Style? _style;

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
