using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Majblommor
{
    /// <summary>
    /// Interaction logic for SchoolsClasses.xaml
    /// </summary>
    public partial class SchoolsClasses : Window
    {
        public ObservableCollection<School> Schools { get; set; }

        public SchoolsClasses(ObservableCollection<School> schools)
        {
            Schools = schools;
            
            InitializeComponent();
        }

        private void NewSchool_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new NameDialog("Ny Skola");
            if (dialog.ShowDialog() == true)
            {
                Schools.Add(new School(dialog.NewName));
            }
            
        }

        private void NewClass_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            School s;
            if(tv.SelectedItem is School)
            {
                s = tv.SelectedItem as School;
            }
            else if(tv.SelectedItem is SchoolClass)
            {
                s = Schools.First(f => f.Classes.Contains(tv.SelectedItem as SchoolClass));
            }
            else
            {
                s = tv.Items[0] as School;
            }
            var dialog = new NameDialog("Ny Klass");
            if (dialog.ShowDialog() == true)
            {
                s.Classes.Add(new SchoolClass(dialog.NewName));
            }
        }

        private void NewClass_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tv.Items.Count > 0;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (tv.SelectedItem is School)
            {
                var s = tv.SelectedItem as School;
                var dialog = new NameDialog("Redigera Skola", s.Name);
                if (dialog.ShowDialog() == true)
                {
                    s.Name = dialog.NewName;
                    s.Changed = DateTime.UtcNow.Ticks;
                }
            }
            else if (tv.SelectedItem is SchoolClass)
            {
                SchoolClass k = tv.SelectedItem as SchoolClass;
                NameDialog dialog = new NameDialog("Redigera Klass", k.Name);
                if (dialog.ShowDialog() == true)
                {
                    k.Name = dialog.NewName;
                    k.Changed = DateTime.UtcNow.Ticks;
                }
            }
        }

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tv.SelectedItem != null;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxManager.OK = "Ta bort";
            MessageBoxManager.Cancel = "Avbryt";
            MessageBoxManager.Register();
            if (tv.SelectedItem is School)
            {
                School s = (School)tv.SelectedItem;
                if (MessageBox.Show("Är du säker att du vill ta bort markerad skola? Alla klasser och elever i skolan kommer försvinna.", "Ta bort skola", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Schools.Remove(s);
                }
            }
            else
            {
                SchoolClass k = (SchoolClass)tv.SelectedItem;
                if (MessageBox.Show("Är du säker att du vill ta bort markerad klass? Alla elever i klassen kommer försvinna.", "Ta bort klass", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Schools.First(f => f.Classes.Contains(k)).Classes.Remove(k);
                }
            }
            
            MessageBoxManager.Unregister();
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tv.SelectedItem != null;
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
