using DotAgenda.MethodClass;
using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.View.Converter
{
    public class RepeatTypeToBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EventDay.Reccurences reccurence = (EventDay.Reccurences)value;
            EventDay.RepeatType repeat = reccurence.Repeat;

            Console.WriteLine("Repeat type : " + repeat);

            return repeat != EventDay.RepeatType.None;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //a implémenter

            throw new NotImplementedException();
        }
    }
}
