using System.Collections.Generic;

using HashTool.Models.Enums;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class HashResultModel : ObservableObject
    {
        private string? inputMode;
        private string? mode;
        private string? content;
        private string? fileSize;
        private string? lastWriteTime;
        private string? computeTime;
        private string? computeCost;
        private List<HashResultItemModel>? items;

        /// <summary>
        /// 输入模式：文件/文件夹/文本
        /// </summary>
        public string InputMode
        {
            get => inputMode ??= string.Empty;
            set => SetProperty(ref inputMode, value);
        }

        /// <summary>
        /// 计算模式：文件流或字符串
        /// </summary>
        public string Mode
        {
            get => mode ??= string.Empty;
            set => SetProperty(ref mode, value);
        }

        /// <summary>
        /// 计算内容：文件路径或文本
        /// </summary>
        public string Content
        {
            get => content ??= string.Empty;
            set => SetProperty(ref content, value);
        }

        /// <summary>
        /// 文件流计算模式下，格式化后的文件大小
        /// </summary>
        public string FileSize
        {
            get => fileSize ??= string.Empty;
            set => SetProperty(ref fileSize, value);
        }

        /// <summary>
        /// 文件流计算模式下，文件最后修改时间
        /// </summary>
        public string LastWriteTime
        {
            get => lastWriteTime ??= string.Empty;
            set => SetProperty(ref lastWriteTime, value);
        }

        /// <summary>
        /// 计算开始时间
        /// </summary>
        public string ComputeTime
        {
            get => computeTime ??= string.Empty;
            set => SetProperty(ref computeTime, value);
        }

        /// <summary>
        /// 计算用时
        /// </summary>
        public string ComputeCost
        {
            get => computeCost ??= string.Empty;
            set => SetProperty(ref computeCost, value);
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        public List<HashResultItemModel> Items
        {
            get => items ??= new();
            set => SetProperty(ref items, value);
        }
    }

    public class HashResultItemModel : ObservableObject
    {
        public HashResultItemModel(AlgorithmEnum id, string value)
        {
            this.id = id;
            this.value = value;
        }

        private AlgorithmEnum id;
        private string value;

        public AlgorithmEnum Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        public string Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
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
