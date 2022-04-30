// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Runtime.CompilerServices;

using HashTool.Properties;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class PropertiesSettingsModel : ObservableObject
    {
        private bool? _isLowerCase;
        private StringCollection? _selectedHashAlgorithm;

        public bool IsLowerCase
        {
            get => _isLowerCase ??= Settings.Default.IsLowerCase;
            set
            {
                SetProperty(ref _isLowerCase, value);
                SaveSettings(value);
            }
        }
        public StringCollection SelectedHashAlgorithm
        {
            get => _selectedHashAlgorithm ??= Settings.Default.SelectedHashAlgorithm;
        }

        private static void SaveSettings<T>(T value, [CallerMemberName] string? propertyName = null)
        {
            Settings.Default[propertyName] = value;
            Settings.Default.Save();
        }
    }
}
