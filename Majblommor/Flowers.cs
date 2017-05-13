using System.ComponentModel;

namespace Majblommor
{
    public class Flowers : INotifyPropertyChanged
    {
        private int _small;
        private int _wreaths;
        private int _large;
        private int _pins;

        public int Small
        {
            get { return _small; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _small, value));
                PropertyChanged.SetField(this, ref _small, value);
            }
        }
        public int Wreaths
        {
            get { return _wreaths; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _wreaths, value));
                PropertyChanged.SetField(this, ref _wreaths, value);
            }
        }
        public int Large
        {
            get { return _large; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _large, value));
                PropertyChanged.SetField(this, ref _large, value);
            }
        }
        public int Pins
        {
            get { return _pins; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _pins, value));
                PropertyChanged.SetField(this, ref _pins, value);
            }
        }

        public Flowers()
        {
            Small = 0;
            Wreaths = 0;
            Large = 0;
            Pins = 0;
        }

        public Flowers(int small, int wreath, int large, int pins)
        {
            Small = small;
            Wreaths = wreath;
            Large = large;
            Pins = pins;
        }

        public Flowers(Flowers b)
        {
            Small = b.Small;
            Wreaths = b.Wreaths;
            Large = b.Large;
            Pins = b.Pins;
        }

        public static Flowers operator +(Flowers b1, Flowers b2)
        {
            return new Flowers(b1.Small + b2.Small, b1.Wreaths + b2.Wreaths, b1.Large + b2.Large, b1.Pins + b2.Pins);
        }

        public static Flowers operator -(Flowers b1, Flowers b2)
        {
            return new Flowers(b1.Small - b2.Small, b1.Wreaths - b2.Wreaths, b1.Large - b2.Large, b1.Pins - b2.Pins);
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
