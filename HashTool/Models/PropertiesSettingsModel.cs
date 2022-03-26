using System.Runtime.CompilerServices;

using HashTool.Models.Enums;
using HashTool.Properties;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class PropertiesSettingsModel : ObservableObject
    {
        private bool? isLowerCase;
        private AlgorithmEnum? selectedHashAlgorithm;

        public bool IsLowerCase
        {
            get => isLowerCase ??= Settings.Default.IsLowerCase;
            set
            {
                SetProperty(ref isLowerCase, value);
                SaveSettings(value);
            }
        }
        public AlgorithmEnum SelectedHashAlgorithm
        {
            get => selectedHashAlgorithm ??= (AlgorithmEnum)Settings.Default.SelectedHashAlgorithm;
            set
            {
                SetProperty(ref selectedHashAlgorithm, value);
                SaveSettings((int)value);
            }
        }

        private void SaveSettings<T>(T value, [CallerMemberName] string? propertyName = null)
        {
            Settings.Default[propertyName] = value;
            Settings.Default.Save();
        }
    }
}
