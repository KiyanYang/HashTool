using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    internal class OpenSourceLicenseModel : ObservableObject
    {
        public const string GPLv3 = "GNU General Public License v3.0";
        public const string MIT = "MIT License";
        public const string Apache = "Apache License 2.0";

        public OpenSourceLicenseModel() { }
        public OpenSourceLicenseModel(string name, string link, string license)
        {
            this.name = name;
            this.link = link;
            this.license = license;
        }

        private string? name;
        private string? link;
        private string? license;

        public string Name
        {
            get => name ??= string.Empty;
            set => SetProperty(ref name, value);
        }
        public string Link
        {
            get => link ??= string.Empty;
            set => SetProperty(ref link, value);
        }
        public string License
        {
            get => license ??= string.Empty;
            set => SetProperty(ref license, value);
        }
    }
}
