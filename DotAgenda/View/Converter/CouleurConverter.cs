using DotAgenda.MethodClass;
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
    public class CouleurConverter : IValueConverter
    {
        GlobalDict _dict;

        object IValueConverter.Convert(object classe, Type targetType, object parameter, CultureInfo culture)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            string couleur = "";

            try
            {
                _dict = GlobalDict._dict;
                couleur = _dict.DictClasse[classe.ToString()].Couleur;
            }

            catch
            {
                Console.WriteLine("Mauvaise conversion de classe en couleur");
            }

            return couleur;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
