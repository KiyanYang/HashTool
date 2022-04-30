// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class HashResultModel : ObservableObject
    {
        private string? _inputMode;
        private string? _mode;
        private string? _content;
        private string? _encodingName;
        private string? _fileSize;
        private string? _lastWriteTime;
        private string? _computeTime;
        private string? _computeCost;
        private List<HashResultItemModel>? _items;

        /// <summary>
        /// 输入模式：文件/文件夹/文本
        /// </summary>
        public string InputMode
        {
            get => _inputMode ??= string.Empty;
            set => SetProperty(ref _inputMode, value);
        }

        /// <summary>
        /// 计算模式：文件流或字符串
        /// </summary>
        public string Mode
        {
            get => _mode ??= string.Empty;
            set => SetProperty(ref _mode, value);
        }

        /// <summary>
        /// 计算内容：文件路径或文本
        /// </summary>
        public string Content
        {
            get => _content ??= string.Empty;
            set => SetProperty(ref _content, value);
        }

        /// <summary>
        /// 计算内容：字符编码名称
        /// </summary>
        public string EncodingName
        {
            get => _encodingName ??= string.Empty;
            set => SetProperty(ref _encodingName, value);
        }

        /// <summary>
        /// 文件流计算模式下，格式化后的文件大小
        /// </summary>
        public string FileSize
        {
            get => _fileSize ??= string.Empty;
            set => SetProperty(ref _fileSize, value);
        }

        /// <summary>
        /// 文件流计算模式下，文件最后修改时间
        /// </summary>
        public string LastWriteTime
        {
            get => _lastWriteTime ??= string.Empty;
            set => SetProperty(ref _lastWriteTime, value);
        }

        /// <summary>
        /// 计算开始时间
        /// </summary>
        public string ComputeTime
        {
            get => _computeTime ??= string.Empty;
            set => SetProperty(ref _computeTime, value);
        }

        /// <summary>
        /// 计算用时
        /// </summary>
        public string ComputeCost
        {
            get => _computeCost ??= string.Empty;
            set => SetProperty(ref _computeCost, value);
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        public List<HashResultItemModel> Items
        {
            get => _items ??= new();
            set => SetProperty(ref _items, value);
        }
    }

    public class HashResultItemModel : ObservableObject
    {
        public HashResultItemModel() { }
        public HashResultItemModel(string name, string value)
        {
            _name = name;
            _value = value;
        }

        private string? _name;
        private string? _value;

        public string Name
        {
            get => _name ??= string.Empty;
            set => SetProperty(ref _name, value);
        }
        public string Value
        {
            get => _value ??= string.Empty;
            set => SetProperty(ref _value, value);
        }

        /// <summary>
        /// 在不更新值的情况下, 刷新内容。
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public void ChangeProperty(string? propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
            base.OnPropertyChanged(propertyName);
        }
    }
}
