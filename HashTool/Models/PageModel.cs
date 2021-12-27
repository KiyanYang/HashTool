using System.Windows.Controls;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    internal class PageModel : ObservableObject
    {
        private string name = string.Empty;
        private Page? page;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public Page? Page
        {
            get => page;
            set => SetProperty(ref page, value);
        }
    }
}
