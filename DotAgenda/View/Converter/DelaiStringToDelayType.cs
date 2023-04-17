using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using DotAgenda.Models;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DotAgenda.View.Converter
{
    public class DelaiStringToDelayType : IValueConverter
    { 

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<Alerte.DelaiType, string> Dict = new Dictionary<Alerte.DelaiType, string>
            {
                { Alerte.DelaiType.Minute, "minutes avant" },
                { Alerte.DelaiType.Hour, "heures avant" },
                { Alerte.DelaiType.Day, "jours avant" },
                { Alerte.DelaiType.Week, "semaines avant" },
            };

            try
            {
                return Dict[(Alerte.DelaiType)value];
            }
            catch
            {

            }

            return "minutes avant";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, Alerte.DelaiType> Dict = new Dictionary<string, Alerte.DelaiType>
            {
                { "minutes avant", Alerte.DelaiType.Minute },
                { "heures avant", Alerte.DelaiType.Hour },
                { "jours avant", Alerte.DelaiType.Day },
                { "semaines avant", Alerte.DelaiType.Week },
            };

            try
            {
                return Dict[(string)value];
            }
            catch
            {

            }

            return Alerte.DelaiType.Minute;
        }
    }
}
    