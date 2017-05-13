using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MoreLinq;

namespace Majblommor
{
    public class SchoolClass : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { PropertyChanged.SetField(this, ref _name, value); }
        }
        public string ID { get; set; }
        public long Changed { get; set; }
        public ObservableCollection<Student> Students { get; private set; }

        public SchoolClass(string name)
        {
            Name = name;
            Students = new ObservableCollection<Student>();
            ID = Guid.NewGuid().ToString("N");
            Changed = DateTime.UtcNow.Ticks;
        }

        public SchoolClass(string name, string id, long changed)
        {
            Name = name;
            Students = new ObservableCollection<Student>();
            ID = id;
            Changed = changed;
        }

        public static IList<SchoolClass> Merge(IList<SchoolClass> l1, IList<SchoolClass> l2)
        {
            var merged = new List<SchoolClass>();
            merged.AddRange(l1.ExceptBy(l2, schoolClass => schoolClass.ID));
            
            var both = l1.Intersect(l2, new SchoolClassComparer());
            foreach(var k in both)
            {
                var k1 = l1.First(schoolClass => schoolClass.ID == k.ID);
                var k2 = l2.First(schoolClass => schoolClass.ID == k.ID);

                var newSchoolClass = new SchoolClass(k1.Changed > k2.Changed ? k1.Name : k2.Name, k.ID, k1.Changed > k2.Changed ? k1.Changed : k2.Changed);
                newSchoolClass.Students.AddRange(Student.Merge(k1.Students, k2.Students));
                merged.Add(newSchoolClass);
            }

            merged.AddRange(l2.ExceptBy(l1, schoolClass => schoolClass.ID));
            return merged;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private class SchoolClassComparer : IEqualityComparer<SchoolClass>
        {
            public bool Equals(SchoolClass x, SchoolClass y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(SchoolClass obj)
            {
                return obj.ID.GetHashCode();
            }
        }
    }
}
