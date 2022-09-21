// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

namespace HashTool.Models.Controls;

public sealed partial class ButtonModel : ObservableObject
{
    [ObservableProperty]
    private string? _content = string.Empty;

    [ObservableProperty]
    private bool _isEnabled;
}
