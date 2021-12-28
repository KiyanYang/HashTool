using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

using HashTool.Helpers;
using HashTool.Models;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace HashTool.ViewModels
{
    internal class AboutViewModel : ObservableObject
    {
        public AboutViewModel()
        {
            update = new();
            openSourceLicenses = new OpenSourceLicenseModel[]
            {
                GetOpenSourceLicense("HandyOrg - HandyControl", "https://github.com/HandyOrg/HandyControl", OpenSourceLicenseModel.MIT),
                GetOpenSourceLicense("aaubry - YamlDotNet", "https://github.com/aaubry/YamlDotNet", OpenSourceLicenseModel.MIT)
            };

            CheckUpdateCommand = new RelayCommand(CheckUpdate);
            OpenLinkCommand = new RelayCommand<string>(OpenLink);
        }
        #region Fields

        private UpdateModel update;
        private OpenSourceLicenseModel[] openSourceLicenses;

        #endregion

        #region Public Properties/Commands

        public string? AssemblyVersion
        {
            get
            {
                var ver = UpdateHelper.GetAssemblyVersion();
                return $"{ver.Major}.{ver.Minor}.{ver.Build}";
            }
        }
        public UpdateModel Update
        {
            get => update;
            set => SetProperty(ref update, value);
        }
        public OpenSourceLicenseModel[] OpenSourceLicenses
        {
            get => openSourceLicenses;
            set => SetProperty(ref openSourceLicenses, value);
        }

        public ICommand CheckUpdateCommand { get; }
        public ICommand OpenLinkCommand { get; }

        #endregion

        #region Helper

        private OpenSourceLicenseModel GetOpenSourceLicense(string name, string link, string license)
        {
            return new OpenSourceLicenseModel()
            {
                Name = name,
                Link = link,
                License = license
            };
        }

        private async void CheckUpdate()
        {
            await UpdateHelper.SetUpdate(Update);
        }

        private void OpenLink(string? link)
        {
            if (link == null)
                return;

            Process.Start("explorer.exe", link);
        }

        #endregion
    }
}
