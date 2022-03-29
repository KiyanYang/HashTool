using System;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class CheckBoxModel : ObservableObject
    {
        public CheckBoxModel() { }
        public CheckBoxModel(string content, bool isEnabled = true)
        {
            this.content = content;
            this.isChecked = isEnabled;
        }

        private string? content;
        private bool? isChecked;

        public string Content
        {
            get => content ??= string.Empty;
            set => SetProperty(ref content, value);
        }
        public bool? IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }
    }

    /// <summary>
    /// 添加枚举属性 EnumContent 的 CheckBoxModel，以便从 Content 访问枚举名称。
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    public class CheckBoxEnumModel<T> : CheckBoxModel where T : Enum
    {
        public CheckBoxEnumModel(T enumObj, bool isEnabled = true) : base(enumObj.ToString(), isEnabled)
        {
            enumContent = enumObj;
        }

        private T enumContent;

        public T EnumContent
        {
            get => enumContent;
            set
            {
                SetProperty(ref enumContent, value);
                Content = value.ToString();
            }
        }
    }
}
