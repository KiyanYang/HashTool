using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

            CheckUpdateCommand = new RelayCommand(CheckUpdate);
            OpenLinkCommand = new RelayCommand<string>(OpenLink);
        }
        #region Fields

        private UpdateModel update;

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
        public ICommand CheckUpdateCommand { get; }
        public ICommand OpenLinkCommand { get; }

        #endregion

        #region Helper

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
