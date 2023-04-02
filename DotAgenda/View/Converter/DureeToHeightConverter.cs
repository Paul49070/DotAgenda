using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DotAgenda.View.Converter
{
    public class DureeToHeightConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime start = (DateTime)values[0];
            DateTime end = (DateTime)values[1];

            double dureeH = end.Hour - start.Hour;
            dureeH -= (double)start.Minute / 60;
            dureeH += (double)end.Minute / 60;

            if (dureeH < 1)
            {
                dureeH = 1;
            }

            return (double) dureeH * 70 ; // le 4 correspond aux espaces entre les lignes qui ne sont ici pas représentés 
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    