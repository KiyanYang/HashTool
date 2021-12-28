using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    internal class OpenSourceLicenseModel : ObservableObject
    {
        public const string GPLv3 = "GNU General Public License v3.0";
        public const string MIT = "MIT License";
        public const string Apache = "Apache License 2.0";

        private string? name;
        private string? link;
        private string? license;

        public string? Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string? Link
        {
            get => link;
            set => SetProperty(ref link, value);
        }
        public string? License
        {
            get => license;
            set => SetProperty(ref license, value);
        }
    }
}
