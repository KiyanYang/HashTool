// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

namespace HashTool.Models.Controls;

public sealed partial class ProgressBarModel : ObservableObject
{
    public ProgressBarModel() { }
    public ProgressBarModel(double value = 0, double minimum = 0, double maximum = 100)
    {
        _value = value;
        _minimum = minimum;
        _maximum = maximum;
    }

    [ObservableProperty]
    private double _value;

    [ObservableProperty]
    private double _minimum;

    [ObservableProperty]
    private double _maximum;
}
