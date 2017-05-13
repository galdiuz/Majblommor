using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using MoreLinq;

namespace Majblommor
{
    public class Student : INotifyPropertyChanged
    {
        private string _firstname = "";
        private string _surname = "";
        private string _telephone = "";
        private string _givSign = "";
        private int _received = 0;
        private int _sms = 0;
        private string _sign = "";
        private string _note = "";
        private Flowers _given;
        private Flowers _returned;

        public string ID { get; set; }
        public long Changed { get; set; }
        public string Firstname
        {
            get { return _firstname; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _firstname, value));
                PropertyChanged.SetField(this, ref _firstname, value);
            }
        }
        public string Surname
        {
            get { return _surname; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _surname, value));
                PropertyChanged.SetField(this, ref _surname, value);
            }
        }
        public string Telephone
        {
            get { return _telephone; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _telephone, value));
                PropertyChanged.SetField(this, ref _telephone, value);
            }
        }
        public string GivSign
        {
            get { return _givSign; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _givSign, value));
                PropertyChanged.SetField(this, ref _givSign, value);
            }
        }
        public int Received
        {
            get { return _received; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _received, value));
                PropertyChanged.SetField(this, ref _received, value);
            }
        }
        public string Sign
        {
            get { return _sign; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _sign, value));
                PropertyChanged.SetField(this, ref _sign, value);
            }
        }
        public string Note
        {
            get { return _note; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _note, value));
                PropertyChanged.SetField(this, ref _note, value);
            }
        }
        public Flowers Given
        {
            get { return _given; }
            set
            {
                if (_given != null) _given.PropertyChanged -= OnBlommorPropertyChanged;
                PropertyChanged.SetField(this, ref _given, value);
                OnBlommorPropertyChanged(this, null);
                _given.PropertyChanged += OnBlommorPropertyChanged;
            }
        }
        public Flowers Returned
        {
            get { return _returned; }
            set
            {
                if (_returned != null) _returned.PropertyChanged -= OnBlommorPropertyChanged;
                PropertyChanged.SetField(this, ref _returned, value);
                OnBlommorPropertyChanged(this, null);
                _returned.PropertyChanged += OnBlommorPropertyChanged;
            }
        }
        public Flowers Total => Given - Returned;
        public int Sold => Total.Small * MainWindow.Price.Small + Total.Wreaths * MainWindow.Price.Wreaths + Total.Large * MainWindow.Price.Large + Total.Pins * MainWindow.Price.Pins;
        public int Sms
        {
            get { return _sms; }
            set
            {
                History.AddToHistory(new Commands.ChangeValue(this, _sms, value));
                PropertyChanged.SetField(this, ref _sms, value);
                OnBlommorPropertyChanged(this, null);
            }
        }
        public int Provision => (int)(Sold * 0.1);
        public int Bonus => CalculateBonus();
        public int Pay => Sold - Sms - (Provision + Bonus);

        public Student()
        {
            Given = new Flowers(MainWindow.Box);
            Returned = new Flowers(MainWindow.Box);
            ID = Guid.NewGuid().ToString("N");
            Changed = DateTime.UtcNow.Ticks;
        }

        public Student(string id, long changed)
        {
            Given = new Flowers();
            Returned = new Flowers();
            ID = id;
            Changed = changed;
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnBlommorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(Returned != null && Given != null)
            {
                Returned.Small = Math.Min(Given.Small, Returned.Small);
                Returned.Wreaths = Math.Min(Given.Wreaths, Returned.Wreaths);
                Returned.Large = Math.Min(Given.Large, Returned.Large);
                Returned.Pins = Math.Min(Given.Pins, Returned.Pins);
            }
            Extensions.OnPropertyChanged(PropertyChanged, this, "Total");
            Extensions.OnPropertyChanged(PropertyChanged, this, "Sold");
            Extensions.OnPropertyChanged(PropertyChanged, this, "Sms");
            Extensions.OnPropertyChanged(PropertyChanged, this, "Provision");
            Extensions.OnPropertyChanged(PropertyChanged, this, "Bonus");
            Extensions.OnPropertyChanged(PropertyChanged, this, "Pay");
        }

        private int CalculateBonus()
        {
            int bonus = 0;
            Flowers t = Total;

            while (t.Small >= MainWindow.Box.Small && t.Wreaths >= MainWindow.Box.Wreaths && t.Large >= MainWindow.Box.Large && t.Pins >= MainWindow.Box.Pins)
            {
                t -= MainWindow.Box;
                bonus += MainWindow.Bonus;
            }

            return bonus;
        }

        public static IList<Student> Merge(IList<Student> l1, IList<Student> l2)
        {
            var merged = new List<Student>();
            merged.AddRange(l1.ExceptBy(l2, student => student.ID));
            
            var both = l1.Intersect(l2, new StudentComparer());
            foreach (var e in both)
            {
                var e1 = l1.First(student => student.ID == e.ID);
                var e2 = l2.First(student => student.ID == e.ID);
                merged.Add(e1.Changed > e2.Changed ? e1 : e2);
            }

            merged.AddRange(l2.ExceptBy(l1, student => student.ID));

            return merged;
        }

        private class StudentComparer : IEqualityComparer<Student>
        {
            public bool Equals(Student x, Student y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(Student obj)
            {
                return obj.ID.GetHashCode();
            }
        }

    }
}
