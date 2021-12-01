using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Model.Control
{
    internal class ButtonModel : ObservableObject
    {
        public ButtonModel(string content, bool isEnabled = true)
        {
            this.content = content;
            this.isEnabled = isEnabled;
        }

        private string content;
        private bool isEnabled;

        public string Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }
    }
}
