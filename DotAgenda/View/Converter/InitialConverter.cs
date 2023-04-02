using DotAgenda.Models;
using LiveChartsCore.Measure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Windows.ApplicationModel.Payments;

namespace DotAgenda.View.Converter
{
    public class InitialConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            string prenom = values[0] as string;
            string nom = values[1] as string;

            return prenom.Substring(0, 1) + nom.Substring(0, 1);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    