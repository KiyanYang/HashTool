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
        private string? mode;
        private string? input;
        private string? encodingName;
        private List<CheckBoxModel>? checkBoxItems;

        public static IReadOnlyList<string> ModeItems { get; } = new[]
        {
            "文件",
            "文件夹",
            "文本",
        };
        public static IReadOnlyList<string> EncodingNameItems { get; } = Encoding.GetEncodings().Select(ei => ei.Name).ToList();

        public string Mode
        {
            get => mode ??= ModeItems[0];
            set => SetProperty(ref mode, value);
        }
        public string Input
        {
            get => input ??= string.Empty;
            set => SetProperty(ref input, value);
        }
        public string EncodingName
        {
            get => encodingName ??= "utf-8";
            set => SetProperty(ref encodingName, value);
        }
        public List<CheckBoxModel> CheckBoxItems
        {
            get => checkBoxItems ??= new();
            set => SetProperty(ref checkBoxItems, value);
        }
    }
}
