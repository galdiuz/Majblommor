using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MoreLinq;

namespace Majblommor
{
    public class School : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { PropertyChanged.SetField(this, ref _name, value); }
        }
        public string ID { get; set; }
        public long Changed { get; set; }
        public ObservableCollection<SchoolClass> Classes { get; private set; }

        public School(string name)
        {
            Name = name;
            Classes = new ObservableCollection<SchoolClass>();
            ID = Guid.NewGuid().ToString("N");
            Changed = DateTime.UtcNow.Ticks;
        }

        public School(string name, string id, long changed)
        {
            Name = name;
            Classes = new ObservableCollection<SchoolClass>();
            ID = id;
            Changed = changed;
        }

        public static IList<School> Merge(IList<School> l1, IList<School> l2)
        {
            var merged = new List<School>();
            merged.AddRange(l1.ExceptBy(l2, school => school.ID));

            var both = l1.Intersect(l2, new SchoolComparer());
            foreach (var s in both)
            {
                var s1 = l1.First(school => school.ID == s.ID);
                var s2 = l2.First(school => school.ID == s.ID);

                var newSchool = new School(s1.Changed > s2.Changed ? s1.Name : s2.Name, s.ID, s1.Changed > s2.Changed ? s1.Changed : s2.Changed);
                newSchool.Classes.AddRange(SchoolClass.Merge(s1.Classes, s2.Classes));
                merged.Add(newSchool);
            }

            merged.AddRange(l2.ExceptBy(l1, school => school.ID));
            return merged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private class SchoolComparer : IEqualityComparer<School>
        {
            public bool Equals(School x, School y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(School obj)
            {
                return obj.ID.GetHashCode();
            }
        }
    }
}
