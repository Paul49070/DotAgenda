using DotAgenda.Models;
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
    public class StartTimeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<ObservableCollection<EventDay>> list = value as ObservableCollection<ObservableCollection<EventDay>>;

            DateTime PlusTot = list[0][0].DateDebut;

            return PlusTot.Hour.ToString("00") + "h" + PlusTot.Minute.ToString("00");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    