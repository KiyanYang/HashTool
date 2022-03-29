using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models.Controls
{
    public class ProgressBarModel : ObservableObject
    {
        public ProgressBarModel() { }
        public ProgressBarModel(double value = 0, double minimum = 0, double maximum = 100)
        {
            this.value = value;
            this.minimum = minimum;
            this.maximum = maximum;
        }

        private double value;
        private double minimum;
        private double maximum;

        public double Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }
        public double Minimum
        {
            get => minimum;
            set => SetProperty(ref minimum, value);
        }
        public double Maximum
        {
            get => maximum;
            set => SetProperty(ref maximum, value);
        }
    }
}
