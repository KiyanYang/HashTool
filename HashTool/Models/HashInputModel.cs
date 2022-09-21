// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text;
using System.Linq;
using HashTool.Models.Controls;

namespace HashTool.Models;

public sealed partial class HashInputModel : ObservableObject
{
    public static IReadOnlyList<string> ModeItems { get; } = new[]
    {
        "文件",
        "文件夹",
        "文本",
    };
    public static IReadOnlyList<string> EncodingNameItems { get; } = Encoding.GetEncodings().Select(ei => ei.Name).ToList();

    /// <summary>
    /// 输入模式。（文件，文件夹，文本）
    /// </summary>
    [ObservableProperty]
    private string _mode = ModeItems[0];

    /// <summary>
    /// 输入内容。（路径或字符串）
    /// </summary>
    [ObservableProperty]
    private string _input = string.Empty;

    /// <summary>
    /// 文本编码。（文本模式下）
    /// </summary>
    [ObservableProperty]
    private string _encodingName = "utf-8";

    /// <summary>
    /// 算法复选框。
    /// </summary>
    [ObservableProperty]
    private List<CheckBoxModel> _checkBoxItems = new();
}
