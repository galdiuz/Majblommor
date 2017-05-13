using System;
using System.Windows;
using System.Windows.Data;

namespace Majblommor
{
    class AddConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;
            for (int i = 0; i < values.Length; i++)
            {
                result += System.Convert.ToDouble(values[i]);
            }
            return new GridLength(result);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
