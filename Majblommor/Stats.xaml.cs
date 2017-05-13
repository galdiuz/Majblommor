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
    /// Interaction logic for Stats.xaml
    /// </summary>
    public partial class Stats : Window
    {
        private IList<School> Schools { get; set; }

        public Stats(IList<School> schools)
        {
            Schools = schools;

            InitializeComponent();

            string text = "";
            int total = 0;
            int brutto = 0;
            int bonus = 0;

            foreach (School s in Schools)
            {
                string schoolClasstext = "";
                int skoltotal = 0;
                int skolbrutto = 0;
                foreach (SchoolClass k in s.Classes)
                {
                    schoolClasstext += "\n  " + k.Name;

                    int schoolClasstotal = 0;
                    int schoolClassbrutto = 0;

                    foreach(Student e in k.Students)
                    {
                        schoolClasstotal += e.Pay;
                        schoolClassbrutto += e.Sold;
                        bonus += e.Bonus;
                    }
                    schoolClasstext += ": " + schoolClassbrutto + " (" + schoolClasstotal + ")";
                    skoltotal += schoolClasstotal;
                    skolbrutto += schoolClassbrutto;
                }

                text += s.Name + ": " + skolbrutto + " (" + skoltotal + ")" + schoolClasstext + "\n";
                total += skoltotal;
                brutto += skolbrutto;
            }

            text += "\nTotalt: " + brutto + " (" + total + ")\nBonus: " + bonus;

            textBlock.Text = text;


        }
    }
}
