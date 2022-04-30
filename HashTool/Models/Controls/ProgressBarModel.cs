// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class ProgressBarModel : ObservableObject
    {
        public ProgressBarModel() { }
        public ProgressBarModel(double value = 0, double minimum = 0, double maximum = 100)
        {
            _value = value;
            _minimum = minimum;
            _maximum = maximum;
        }

        private double _value;
        private double _minimum;
        private double _maximum;

        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        public double Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
        }
        public double Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }
    }
}
