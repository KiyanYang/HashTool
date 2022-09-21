// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows.Input;

using HashTool.Models;
using HashTool.Models.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HashTool.ViewModels
{
    internal class SettingPageViewModel : ObservableObject
    {
        public SettingPageViewModel()
        {
            SaveSettingCommand = new RelayCommand(SaveSetting);
            CancelSettingCommand = new RelayCommand(CancelSetting);
        }

        #region Fields

        private PropertiesSettingsModel _propertiesSettings = GetInstance<PropertiesSettingsModel>();

        private CheckBoxModel? _isLowerCase;
        private CheckBoxModel? _mainWindowTopmost;
        private CheckBoxModel? _hashResultWindowTopMost;

        #endregion

        #region Public Properties/Commands

        /// <summary>
        /// 结果小写。
        /// </summary>
        public CheckBoxModel IsLowerCase
        {
            get => _isLowerCase ??= new("结果小写", _propertiesSettings.IsLowerCase);
            set => SetProperty(ref _isLowerCase, value);
        }

        /// <summary>
        /// 主界面置顶。
        /// </summary>
        public CheckBoxModel MainWindowTopmost
        {
            get => _mainWindowTopmost ??= new("主界面置顶", _propertiesSettings.MainWindowTopmost);
            set => SetProperty(ref _mainWindowTopmost, value);
        }

        /// <summary>
        /// 结果界面置顶。
        /// </summary>
        public CheckBoxModel HashResultWindowTopMost
        {
            get => _hashResultWindowTopMost ??= new("结果界面置顶", _propertiesSettings.HashResultWindowTopmost);
            set => SetProperty(ref _hashResultWindowTopMost, value);
        }

        public ICommand SaveSettingCommand { get; }
        public ICommand CancelSettingCommand { get; }

        #endregion

        #region Helpers

        private void SaveSetting()
        {
            _propertiesSettings.IsLowerCase = IsLowerCase.IsChecked ?? false;
            _propertiesSettings.MainWindowTopmost = MainWindowTopmost.IsChecked ?? false;
            _propertiesSettings.HashResultWindowTopmost = HashResultWindowTopMost.IsChecked ?? false;
        }

        private void CancelSetting()
        {
            IsLowerCase.IsChecked = _propertiesSettings.IsLowerCase;
            MainWindowTopmost.IsChecked = _propertiesSettings.MainWindowTopmost;
            HashResultWindowTopMost.IsChecked = _propertiesSettings.HashResultWindowTopmost;
        }

        #endregion
    }
}
