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
    public class IconConverter : IValueConverter
    {

        GlobalDict _dict;

        object IValueConverter.Convert(object classe, Type targetType, object parameter, CultureInfo culture)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            string icon = "";

            try
            {
                _dict = GlobalDict._dict;
                icon = _dict.DictClasse[classe.ToString()].Icon;
            }

            catch
            {
                Console.WriteLine("Mauvaise conversion de classe en icon");
            }

            return icon;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
