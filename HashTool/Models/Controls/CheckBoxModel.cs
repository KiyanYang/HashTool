using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class CheckBoxModel : ObservableObject
    {
        public CheckBoxModel(string content, bool isEnabled = true)
        {
            this.content = content;
            this.isChecked = isEnabled;
        }

        private string content;
        private bool? isChecked;

        public string Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }
        public bool? IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }
    }
}
