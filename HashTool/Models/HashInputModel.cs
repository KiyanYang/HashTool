using System.Collections.Generic;

using HashTool.Models.Controls;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class HashInputModel : ObservableObject
    {
        private string? mode;
        private string input = string.Empty;
        private List<CheckBoxModel>? checkBoxItems;

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
            get => input;
            set => SetProperty(ref input, value);
        }
        public List<CheckBoxModel> CheckBoxItems
        {
            get => checkBoxItems ??= new();
            set => SetProperty(ref checkBoxItems, value);
        }
    }
}
