// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using HashTool.Models.Controls;
using HashTool.Models.Enums;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class HashInputModel : ObservableObject
    {
        private string? mode;
        private string? input;
        private List<CheckBoxEnumModel<AlgorithmEnum>>? checkBoxItems;

        public static IReadOnlyList<string> ModeItem { get; } = new[]
        {
            "文件",
            "文件夹",
            "文本",
        };
        public string Mode
        {
            get => mode ??= ModeItem[0];
            set => SetProperty(ref mode, value);
        }
        public string Input
        {
            get => input ??= string.Empty;
            set => SetProperty(ref input, value);
        }
        public List<CheckBoxEnumModel<AlgorithmEnum>> CheckBoxItems
        {
            get => checkBoxItems ??= new();
            set => SetProperty(ref checkBoxItems, value);
        }
    }
}
