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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Flowers Box { get; set; }
        public Flowers Price { get; set; }
        public int Bonus { get; set; }

        public Settings(Flowers box, Flowers price, int bonus)
        {
            Box = new Flowers(box);
            Price = new Flowers(price);
            Bonus = bonus;
            InitializeComponent();
        }

        private void OK_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
