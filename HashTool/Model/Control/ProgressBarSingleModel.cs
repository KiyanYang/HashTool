using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Model.Control
{
    public class ProgressBarSingleModel : ObservableObject
    {
        private double value_;
        private double minimum = 0;
        private double maximun = 1000;

        public double Value
        {
            get => value_;
            set => SetProperty(ref value_, value);
        }
        public double Minimum
        {
            get => minimum;
            set => SetProperty(ref minimum, value);
        }
        public double Maximum
        {
            get => maximun;
            set => SetProperty(ref maximun, value);
        }
    }
}
