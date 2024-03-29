﻿using DotAgenda.MethodClass;
using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.View.Converter
{
    public class FileToColor : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GlobalDict _dict = GlobalDict._dict;

            FileInfo fi = new FileInfo((string)value);

            try
            {
                return _dict.DossierTypeIconDict[_dict.ExtensionDict[fi.Extension]][2];
            }

            catch
            {
                return _dict.DossierTypeIconDict[_dict.Autre][2];
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
