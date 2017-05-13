using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Majblommor
{
    /// <summary>
    /// Interaction logic for NameDialog.xaml
    /// </summary>
    public partial class NameDialog : Window
    {
        public string NewName { get; set; }
        
        public NameDialog(string title)
        {
            NewName = "";
            InitializeComponent();
            Title = title;
            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public NameDialog(string title, string name)
        {
            NewName = name;
            InitializeComponent();
            Title = title;
        }

        private void OK_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (NewName != "")
                DialogResult = true;
        }

        private void OK_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (NewName != "")
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
