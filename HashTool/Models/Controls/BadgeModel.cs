using System.Windows;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class BadgeModel : ObservableObject
    {
        public BadgeModel(string text, bool showBadge = true)
        {
            this.text = text;
            this.showBadge = showBadge;
        }

        private string text;
        private bool showBadge;
        private Style? style;

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
        public bool ShowBadge
        {
            get => showBadge;
            set => SetProperty(ref showBadge, value);
        }
        public Style? Style
        {
            get => style;
            set => SetProperty(ref style, value);
        }

        public bool SetStyle(string name)
        {
            if (Application.Current.TryFindResource(name) is Style s)
            {
                Style = s;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
