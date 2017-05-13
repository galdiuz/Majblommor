using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Majblommor
{
    class UICommands
    {
        static UICommands()
        {
            New.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            Open.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            Save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            Exit.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));
            Undo.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            Redo.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
        }
        
        public static RoutedUICommand New = new RoutedUICommand("Ny", "New", typeof(UICommands));

        public static RoutedUICommand Open = new RoutedUICommand("Öppna", "Open", typeof(UICommands));

        public static RoutedUICommand Save = new RoutedUICommand("Spara", "Save", typeof(UICommands));

        public static RoutedUICommand SaveAs = new RoutedUICommand("Spara Som...", "SaveAs", typeof(UICommands));

        public static RoutedUICommand Merge = new RoutedUICommand("Sammanfoga Med...", "Merge", typeof(UICommands));

        public static RoutedUICommand Exit = new RoutedUICommand("Avsluta", "Exit", typeof(UICommands));

        public static RoutedUICommand EditSchools = new RoutedUICommand("Redigera Skolor & Klasser", "EditSchools", typeof(UICommands));

        public static RoutedUICommand Settings = new RoutedUICommand("Inställningar", "Settings", typeof(UICommands));

        public static RoutedUICommand NewStudent = new RoutedUICommand("Lägg Till Elev", "NewStudent", typeof(UICommands));

        public static RoutedUICommand DeleteStudent = new RoutedUICommand("Ta Bort Elev", "DeleteStudent", typeof(UICommands));
        
        public static RoutedUICommand Undo = new RoutedUICommand("Ångra", "Undo", typeof(UICommands));

        public static RoutedUICommand Redo = new RoutedUICommand("Gör Om", "Redo", typeof(UICommands));

        public static RoutedUICommand NewSchool = new RoutedUICommand("Lägg Till Skola", "NewSchool", typeof(UICommands));

        public static RoutedUICommand NewClass = new RoutedUICommand("Lägg Till Klass", "NewClass", typeof(UICommands));

        public static RoutedUICommand Edit = new RoutedUICommand("Edit", "Edit", typeof(UICommands));

        public static RoutedUICommand Delete = new RoutedUICommand("Ta Bort", "Delete", typeof(UICommands));

        public static RoutedUICommand Close = new RoutedUICommand("Stäng", "Close", typeof(UICommands));

        public static RoutedUICommand OK = new RoutedUICommand("OK", "OK", typeof(UICommands));

        public static RoutedUICommand Cancel = new RoutedUICommand("Avbryt", "Cancel", typeof(UICommands));

        public static RoutedUICommand Stats = new RoutedUICommand("Stats", "Stats", typeof(UICommands));

        public static RoutedUICommand About = new RoutedUICommand("Om Programmet", "About", typeof(UICommands));
    }
}
