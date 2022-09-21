// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

namespace HashTool.Models;

internal sealed partial class UpdateModel : ObservableObject
{
    [ObservableProperty]
    private bool? _hasUpdate;

    [ObservableProperty]
    private string? _version;

    [ObservableProperty]
    private string? _downloadUrl;

    [ObservableProperty]
    private string? _githubUrl;

    [ObservableProperty]
    private string? _giteeUrl;
}
