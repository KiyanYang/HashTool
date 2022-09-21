// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace HashTool.Models;

public sealed partial class HashResultModel : ObservableObject
{
    /// <summary>
    /// 输入模式：文件/文件夹/文本。
    /// </summary>
    [ObservableProperty]
    private string _inputMode = string.Empty;

    /// <summary>
    /// 计算模式：文件流或字符串。
    /// </summary>
    [ObservableProperty]
    private string _mode = string.Empty;

    /// <summary>
    /// 计算内容：文件路径或文本。
    /// </summary>
    [ObservableProperty]
    private string _content = string.Empty;

    /// <summary>
    /// 计算内容：字符编码名称。
    /// </summary>
    [ObservableProperty]
    private string _encodingName = string.Empty;

    /// <summary>
    /// 文件流计算模式下，格式化后的文件大小。
    /// </summary>
    [ObservableProperty]
    private string _fileSize = string.Empty;

    /// <summary>
    /// 文件流计算模式下，文件最后修改时间。
    /// </summary>
    [ObservableProperty]
    private string _lastWriteTime = string.Empty;

    /// <summary>
    /// 计算开始时间。
    /// </summary>
    [ObservableProperty]
    private string _computeTime = string.Empty;

    /// <summary>
    /// 计算用时。
    /// </summary>
    [ObservableProperty]
    private string _computeCost = string.Empty;

    /// <summary>
    /// 计算结果。
    /// </summary>
    [ObservableProperty]
    private List<HashResultItemModel> _items = new();
}

public sealed partial class HashResultItemModel : ObservableObject
{
    /// <summary>
    /// 算法名称。
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// 哈希结果。
    /// </summary>
    [ObservableProperty]
    private string _value = string.Empty;

    /// <summary>
    /// 在不更新值的情况下, 刷新内容。
    /// </summary>
    /// <param name="propertyName">属性名称。</param>
    public void ChangeProperty(string? propertyName = null)
    {
        base.OnPropertyChanging(propertyName);
        base.OnPropertyChanged(propertyName);
    }
}
