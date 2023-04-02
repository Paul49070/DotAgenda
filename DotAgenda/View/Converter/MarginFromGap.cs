using DotAgenda.Models;
using LiveChartsCore.Measure;
using Microsoft.Identity.Client;
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
    public class MarginFromGap : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var currentRow = values[0] as ObservableCollection<ObservableCollection<EventDay>>;

            var ListeLigne_temp = values[1] as ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>>;


            int indexOfCurrentList = ListeLigne_temp.IndexOf(currentRow); //indice dans les colonnes de l'event actuel


            double CalculMargin(DateTime last, DateTime first)
            {
                double duree;
                duree = first.Hour - last.Hour;
                duree -= (double)last.Minute / 60;
                duree += (double)first.Minute / 60;

                return duree * 70 - 1; //le 1 est la marge par défaut entre les lignes
            }



            if (indexOfCurrentList > 0)
            {
                DateTime lastEvent = new DateTime(2000,01,01);

                foreach(ObservableCollection<EventDay> col in ListeLigne_temp[indexOfCurrentList-1])
                {
                    foreach(EventDay e in col)
                    {
                        if (DateTime.Compare(e.DateFin, lastEvent) >= 0) 
                            lastEvent = e.DateFin;
                    }
                }
                DateTime PlusTot = currentRow[0][0].DateDebut;

                double duree;


                if (PlusTot.Hour == 0 && PlusTot.Minute == 0)
                    duree = 0;

                else duree = CalculMargin(lastEvent, PlusTot);

                return new Thickness(2, duree, 2, 0);

            }

            else if (indexOfCurrentList == 0)
            {
                DateTime PlusTot = currentRow[0][0].DateDebut;

                DateTime lastEvent = new DateTime(2000, 01, 01, 0, 0, 0);

                double duree = CalculMargin(lastEvent, PlusTot);

                Console.WriteLine(duree);

                return new Thickness(2, duree, 2, 0);
            }

            else return new Thickness(2, 0, 2, 0);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    