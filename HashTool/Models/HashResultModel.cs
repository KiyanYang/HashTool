using System.Collections.Generic;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class HashResultModel : ObservableObject
    {
        // 输入模式
        private string inputMode = string.Empty;
        // 计算模式：文件或文本
        private string mode = string.Empty;
        // 计算内容：文件路径或文本
        private string content = string.Empty;
        // 以文件计算时
        private string fileSize = string.Empty;
        private string lastWriteTime = string.Empty;

        private string computeTime = string.Empty;
        private string computeCost = string.Empty;
        private List<HashResultItemModel>? items;

        public string InputMode
        {
            get => inputMode;
            set => SetProperty(ref inputMode, value);
        }
        public string Mode
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }
        public string Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }
        public string FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value);
        }
        public string LastWriteTime
        {
            get => lastWriteTime;
            set => SetProperty(ref lastWriteTime, value);
        }
        public string ComputeTime
        {
            get => computeTime;
            set => SetProperty(ref computeTime, value);
        }
        public string ComputeCost
        {
            get => computeCost;
            set => SetProperty(ref computeCost, value);
        }
        public List<HashResultItemModel> Items
        {
            get => items ??= new();
            set => SetProperty(ref items, value);
        }
    }

    public class HashResultItemModel : ObservableObject
    {
        public HashResultItemModel(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        private string name;
        private string value;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }
    }
}
