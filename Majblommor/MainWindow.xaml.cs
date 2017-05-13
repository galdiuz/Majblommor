using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Input;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using MoreLinq;
using System;

namespace Majblommor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static Flowers Box { get; set; }
        public static Flowers Price { get; set; }
        public static int Bonus { get; set; }
        public ObservableCollection<School> Schools { get; set; }

        private ICollectionView _view;
        public ICollectionView View
        {
            get { return _view; }
            set { PropertyChanged.SetField(this, ref _view, value); }
        }
        
        private bool _hideDone = false;
        public bool HideDone
        {
            get { return _hideDone; }
            set
            {
                _hideDone = value;
                if(View != null) View.Filter = Filter;
            }
        }

        private bool _hideInactive = true;
        public bool HideInactive
        {
            get { return _hideInactive; }
            set
            {
                _hideInactive = value;
                if (View != null) View.Filter = Filter;
            }
        }

        private School _currentSchool;
        public School CurrentSchool
        {
            get { return _currentSchool; }
            set
            {
                PropertyChanged.SetField(this, ref _currentSchool, value);
                if (CurrentSchool?.Classes.Count > 0) CurrentClass = CurrentSchool.Classes[0];
            }
        }

        private SchoolClass _currentClass;
        public SchoolClass CurrentClass
        {
            get { return _currentClass; }
            set
            {
                if (CurrentClass != null) CurrentClass.Students.CollectionChanged -= Students_CollectionChanged;
                PropertyChanged.SetField(this, ref _currentClass, value);
                if(_currentClass != null)
                {
                    View = CollectionViewSource.GetDefaultView(_currentClass.Students);
                    View.Filter = Filter;
                }
                if (CurrentClass != null) CurrentClass.Students.CollectionChanged += Students_CollectionChanged;
            }
        }

        private bool userAddingRow = false;
        private string filename;
        private bool isManualEditCommit = false;
        private bool pasting = false;

        public MainWindow()
        {
            Schools = new ObservableCollection<School>();
            Box = new Flowers(20, 10, 6, 8);
            Price = new Flowers(20, 40, 20, 60);
            Bonus = 28;
            
            InitializeComponent();

            CommandManager.AddPreviewExecutedHandler(dataGrid, PreviewPaste);
            dataGrid.PasteFinished += new CustomDataGrid.PasteFinishedEventHandler(DataGrid_PasteFinished);
        }

        private bool Filter(object o)
        {
            Student e = (Student)o;
            return (!HideDone || e.Sign == "")
                && (!HideInactive || (e.Given.Small > 0 || e.Given.Wreaths > 0 || e.Given.Large > 0 || e.Given.Pins > 0));
        }

        private void PreviewPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                pasting = true;
                History.NewStep();
            }
        }
        
        private void DataGrid_PasteFinished(object sender, PasteFinishedEventArgs e)
        {
            pasting = false;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!isManualEditCommit)
            {
                var val = ((TextBox)e.EditingElement).Text;
                var student = e.Row.Item as Student;
                var propertyPath = ((e.Column as DataGridBoundColumn)?.Binding as Binding).Path.Path;
                
                if (!pasting)
                {
                    if (propertyPath.Contains("."))
                    {
                        int newVal;
                        if (int.TryParse(val, out newVal))
                        {
                            var split = propertyPath.Split('.');
                            var flowers = (Flowers)student.GetType().GetProperty(split[0]).GetValue(student);
                            var property = flowers.GetType().GetProperty(split[1]);
                            int oldVal = (int)property.GetValue(flowers);
                            if (oldVal == newVal) return;
                            History.NewStep();
                        }
                    }
                    else
                    {
                        var property = student.GetType().GetProperty(propertyPath);
                        if (property.PropertyType == typeof(int))
                        {
                            int newVal;
                            if (int.TryParse(val, out newVal))
                            {
                                var oldVal = (int)property.GetValue(student);
                                if (oldVal == newVal) return;
                                History.NewStep();
                            }
                        }
                        else
                        {
                            var oldVal = (string)property.GetValue(student);
                            if (oldVal == val) return;
                            History.NewStep();
                        }
                    }
                }

                student.Changed = DateTime.UtcNow.Ticks;
                isManualEditCommit = true;
                var grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEditCommit = false;
            }
            
        }
        
        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            userAddingRow = true;
        }

        private void Students_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(userAddingRow)
            {
                userAddingRow = false;
                if (!pasting) History.NewStep();
                History.AddToHistory(new Commands.AddStudent(CurrentClass, (Student)e.NewItems[0]));
            }
        }
        
        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                Header.Margin = new Thickness(-e.HorizontalOffset + 1, 48, 0, 0);
            }
        }
        
        #region File Menu
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Är du säker att du vill skapa ett blankt dokument?", "Nytt dokument", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                filename = null;
                Schools.Clear();
                History.ClearHistory();
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "XML Filer (*.xml)|*.xml|Alla Filer (*.*)|*.*" };
            if (dialog.ShowDialog() != true) return;

            filename = dialog.FileName;

            Schools.Clear();
            School school = null;
            SchoolClass schoolClass = null;
            Student student = null;

            using (var reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement()) continue;
                    switch (reader.Name)
                    {
                        case "Box":
                            Box = new Flowers(int.Parse(reader["Small"]), int.Parse(reader["Wreaths"]), int.Parse(reader["Large"]), int.Parse(reader["Pins"]));
                            break;
                        case "Price":
                            Price = new Flowers(int.Parse(reader["Small"]), int.Parse(reader["Wreaths"]), int.Parse(reader["Large"]), int.Parse(reader["Pins"]));
                            break;
                        case "Bonus":
                            Bonus = reader.ReadElementContentAsInt();
                            break;
                        case "School":
                            school = new School(reader["Name"], reader["ID"], long.Parse(reader["Changed"]));
                            Schools.Add(school);
                            break;
                        case "Class":
                            schoolClass = new SchoolClass(reader["Name"], reader["ID"], long.Parse(reader["Changed"]));
                            school?.Classes.Add(schoolClass);
                            break;
                        case "Student":
                            student = new Student(reader["ID"], long.Parse(reader["Changed"]));
                            schoolClass?.Students.Add(student);
                            break;
                        case "Firstname":
                            student.Firstname = reader.ReadElementContentAsString();
                            break;
                        case "Surname":
                            student.Surname = reader.ReadElementContentAsString();
                            break;
                        case "Telephone":
                            student.Telephone = reader.ReadElementContentAsString();
                            break;
                        case "GivSmall":
                            student.Given.Small = reader.ReadElementContentAsInt();
                            break;
                        case "GivWreaths":
                            student.Given.Wreaths = reader.ReadElementContentAsInt();
                            break;
                        case "GivLarge":
                            student.Given.Large = reader.ReadElementContentAsInt();
                            break;
                        case "GivPins":
                            student.Given.Pins = reader.ReadElementContentAsInt();
                            break;
                        case "GivSign":
                            student.GivSign = reader.ReadElementContentAsString();
                            break;
                        case "RetSmall":
                            student.Returned.Small = reader.ReadElementContentAsInt();
                            break;
                        case "RetWreaths":
                            student.Returned.Wreaths = reader.ReadElementContentAsInt();
                            break;
                        case "RetLarge":
                            student.Returned.Large = reader.ReadElementContentAsInt();
                            break;
                        case "RetPins":
                            student.Returned.Pins = reader.ReadElementContentAsInt();
                            break;
                        case "Received":
                            student.Received = reader.ReadElementContentAsInt();
                            break;
                        case "Sign":
                            student.Sign = reader.ReadElementContentAsString();
                            break;
                        case "Note":
                            student.Note = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }

            if (Schools.Count > 0)
            {
                CurrentSchool = Schools[0];
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (filename == null)
            {
                SaveAs_Executed(sender, e);
            }
            else
            {
                Save();
            }
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog() { Filter = "XML Filer (*.xml)|*.xml|Alla Filer (*.*)|*.*" };
            if (dialog.ShowDialog() != true) return;
            filename = dialog.FileName;
            Save();
        }

        private void Save()
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true
            };

            using (var writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Data");

                writer.WriteStartElement("Box");
                writer.WriteAttributeString("Small", Box.Small.ToString());
                writer.WriteAttributeString("Wreaths", Box.Wreaths.ToString());
                writer.WriteAttributeString("Large", Box.Large.ToString());
                writer.WriteAttributeString("Pins", Box.Pins.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Price");
                writer.WriteAttributeString("Small", Price.Small.ToString());
                writer.WriteAttributeString("Wreaths", Price.Wreaths.ToString());
                writer.WriteAttributeString("Large", Price.Large.ToString());
                writer.WriteAttributeString("Pins", Price.Pins.ToString());
                writer.WriteEndElement();

                writer.WriteElementString("Bonus", Bonus.ToString());

                foreach (var school in Schools)
                {
                    writer.WriteStartElement("School");
                    writer.WriteAttributeString("Name", school.Name);
                    writer.WriteAttributeString("ID", school.ID);
                    writer.WriteAttributeString("Changed", school.Changed.ToString());
                    foreach (var schoolClass in school.Classes)
                    {
                        writer.WriteStartElement("SchoolClass");
                        writer.WriteAttributeString("Name", schoolClass.Name);
                        writer.WriteAttributeString("ID", schoolClass.ID);
                        writer.WriteAttributeString("Changed", schoolClass.Changed.ToString());
                        foreach (var student in schoolClass.Students)
                        {
                            writer.WriteStartElement("Student");
                            writer.WriteAttributeString("ID", student.ID);
                            writer.WriteAttributeString("Changed", student.Changed.ToString());
                            writer.WriteElementString("Förname", student.Firstname);
                            writer.WriteElementString("Surnamename", student.Surname);
                            writer.WriteElementString("Telephone", student.Telephone);
                            writer.WriteElementString("GivSmall", student.Given.Small.ToString());
                            writer.WriteElementString("GivWreaths", student.Given.Wreaths.ToString());
                            writer.WriteElementString("GivLarge", student.Given.Large.ToString());
                            writer.WriteElementString("GivPins", student.Given.Pins.ToString());
                            writer.WriteElementString("GivSign", student.GivSign);
                            writer.WriteElementString("RetSmall", student.Returned.Small.ToString());
                            writer.WriteElementString("RetWreaths", student.Returned.Wreaths.ToString());
                            writer.WriteElementString("RetLarge", student.Returned.Large.ToString());
                            writer.WriteElementString("RetPins", student.Returned.Pins.ToString());
                            writer.WriteElementString("Received", student.Received.ToString());
                            writer.WriteElementString("Sign", student.Sign);
                            writer.WriteElementString("Note", student.Note);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void Merge_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "XML Filer (*.xml)|*.xml|Alla Filer (*.*)|*.*" };
            if (dialog.ShowDialog() != true) return;

            var file = dialog.FileName;

            var newschools = new List<School>();
            School school = null;
            SchoolClass schoolClass = null;
            Student student = null;

            using (var reader = XmlReader.Create(file))
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement()) continue;
                    switch (reader.Name)
                    {
                        case "School":
                            school = new School(reader["Name"], reader["ID"], long.Parse(reader["Changed"]));
                            newschools.Add(school);
                            break;
                        case "SchoolClass":
                            schoolClass = new SchoolClass(reader["Name"], reader["ID"], long.Parse(reader["Changed"]));
                            school?.Classes.Add(schoolClass);
                            break;
                        case "Student":
                            student = new Student(reader["ID"], long.Parse(reader["Changed"]));
                            schoolClass?.Students.Add(student);
                            break;
                        case "Firstname":
                            student.Firstname = reader.ReadElementContentAsString();
                            break;
                        case "Surname":
                            student.Surname = reader.ReadElementContentAsString();
                            break;
                        case "Telephone":
                            student.Telephone = reader.ReadElementContentAsString();
                            break;
                        case "GivSmall":
                            student.Given.Small = reader.ReadElementContentAsInt();
                            break;
                        case "GivWreaths":
                            student.Given.Wreaths = reader.ReadElementContentAsInt();
                            break;
                        case "GivLarge":
                            student.Given.Large = reader.ReadElementContentAsInt();
                            break;
                        case "GivPins":
                            student.Given.Pins = reader.ReadElementContentAsInt();
                            break;
                        case "GivSign":
                            student.GivSign = reader.ReadElementContentAsString();
                            break;
                        case "RetSmall":
                            student.Returned.Small = reader.ReadElementContentAsInt();
                            break;
                        case "RetWreaths":
                            student.Returned.Wreaths = reader.ReadElementContentAsInt();
                            break;
                        case "RetLarge":
                            student.Returned.Large = reader.ReadElementContentAsInt();
                            break;
                        case "RetPins":
                            student.Returned.Pins = reader.ReadElementContentAsInt();
                            break;
                        case "Received":
                            student.Received = reader.ReadElementContentAsInt();
                            break;
                        case "Sign":
                            student.Sign = reader.ReadElementContentAsString();
                            break;
                        case "Note":
                            student.Note = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }

            var mergedschools = School.Merge(Schools, newschools);
            Schools.Clear();
            Schools.AddRange(mergedschools);

            if (Schools.Count > 0)
            {
                CurrentSchool = Schools[0];
            }
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Edit Menu
        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            History.Undo();
            View.Filter = Filter;
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            History.Redo();
            View.Filter = Filter;
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = History.CanUndo;
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = History.CanRedo;
        }

        private void NewStudentCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentClass != null;
        }

        private void NewStudentCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var student = new Student();
            CurrentClass.Students.Add(new Student());
            History.AddToHistory(new Commands.AddStudent(CurrentClass, student));
        }

        private void DeleteStudentCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dataGrid.SelectedCells.Select(x => x.Item as Student).Any(x => x != null);
        }

        private void DeleteStudentCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxManager.OK = "Ta bort";
            MessageBoxManager.Cancel = "Avbryt";
            MessageBoxManager.Register();
            var items = dataGrid.SelectedCells.Select(x => x.Item as Student).Where(x => x != null).ToHashSet();
            var text = items.Count > 1 ? "Är du säker att du vill ta bort markerade elever?" : "Är du säker att du vill ta bort markerad elev?";
            var caption = items.Count > 1 ? "Ta bort elever" : "Ta bort elev";
            
            History.NewStep();
            if (MessageBox.Show(text, caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                foreach (var student in items)
                {
                    CurrentClass.Students.Remove(student);
                    History.AddToHistory(new Commands.RemoveStudent(CurrentClass, student));
                }
            }
            MessageBoxManager.Unregister();
        }

        private void EditSchools_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new SchoolsClasses(Schools).ShowDialog();
        }

        private void Settings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Settings(Box, Price, Bonus);
            if (dialog.ShowDialog() != true) return;
            Box = dialog.Box;
            Price = dialog.Price;
            Bonus = dialog.Bonus;

            if (CurrentClass != null)
            {
                foreach (var student in CurrentClass.Students)
                {
                    student.OnBlommorPropertyChanged(this, null);
                }
            }
        }

        #endregion

        private void Stats_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new Stats(Schools).ShowDialog();
        }

        private void windowElement_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxManager.Yes = "Spara";
            MessageBoxManager.No = "Stäng";
            MessageBoxManager.Cancel = "Avbryt";
            MessageBoxManager.Register();
            var result = MessageBox.Show("Vill du spara innan du stänger programmet?", "Spara", MessageBoxButton.YesNoCancel);
            if(result == MessageBoxResult.Yes)
            {
                Save_Executed(sender, null);
            }
            else if(result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            MessageBoxManager.Unregister();
        }

        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Ett program för att underlätta redovisning av majblommor.\n\nSkapat av Andreas Lundkvist.", "Om Programmet", MessageBoxButton.OK);
        }
    }
}
