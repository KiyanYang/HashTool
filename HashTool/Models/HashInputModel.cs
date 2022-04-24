// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text;
using System.Linq;
using HashTool.Models.Controls;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class HashInputModel : ObservableObject
    {
        private string? _mode;
        private string? _input;
        private string? _encodingName;
        private List<CheckBoxModel>? _checkBoxItems;

        public static IReadOnlyList<string> ModeItems { get; } = new[]
        {
            "文件",
            "文件夹",
            "文本",
        };
        public static IReadOnlyList<string> EncodingNameItems { get; } = Encoding.GetEncodings().Select(ei => ei.Name).ToList();

        public string Mode
        {
            get => _mode ??= ModeItems[0];
            set => SetProperty(ref _mode, value);
        }
        public string Input
        {
            get => _input ??= string.Empty;
            set => SetProperty(ref _input, value);
        }
        public string EncodingName
        {
            get => _encodingName ??= "utf-8";
            set => SetProperty(ref _encodingName, value);
        }
        public List<CheckBoxModel> CheckBoxItems
        {
            get => _checkBoxItems ??= new();
            set => SetProperty(ref _checkBoxItems, value);
        }
    }
}
