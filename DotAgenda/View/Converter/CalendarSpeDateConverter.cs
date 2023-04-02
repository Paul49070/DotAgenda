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
    public class CalendarSpeDateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            var date = (DateTime)values[0];
            ObservableCollection<DateTime> dates = values[1] as ObservableCollection<DateTime>;
            if (dates == null) return 0;

            return dates.Contains(date.Date);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    