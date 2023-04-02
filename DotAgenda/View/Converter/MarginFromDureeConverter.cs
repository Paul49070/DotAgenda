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
    public class MarginFromDureeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            var currentEvent = values[0] as EventDay;
            var ListeColumn = values[1] as ObservableCollection<EventDay>;

            int indexOfCurrentEvent = ListeColumn.IndexOf(currentEvent); //indice dans les colonnes de l'event actuel

            var ListeLigne_temp = values[2] as ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>>;

            int SearchCurrentLine()
            {
                for(int i = 0; i<ListeLigne_temp.Count; ++i)
                {
                    if (ListeLigne_temp[i].IndexOf(ListeColumn) != -1)
                    {
                        return i;
                    }
                }

                return -1;
            }

            int indexCurrentLine = SearchCurrentLine();

            var ListeLigne = ListeLigne_temp[indexCurrentLine];

            double duree = 0;

            if (ListeLigne != null)
            {
                if (ListeColumn != null)
                {
                    if (ListeColumn.Count > 0)
                    {
                        if(ListeLigne[0].IndexOf(currentEvent) != 0)
                        {
                            DateTime PlusTot;

                            if (indexOfCurrentEvent > 0)
                            {
                                PlusTot = ListeColumn[indexOfCurrentEvent - 1].DateFin;
                            }

                            else PlusTot = ListeLigne[0][0].DateDebut;

                            duree = currentEvent.DateDebut.Hour - PlusTot.Hour;
                            duree -= (double)PlusTot.Minute / 60;
                            duree += (double)currentEvent.DateDebut.Minute / 60;


                        }
                    }
                }
            }

            return new Thickness(2, duree * 70, 2, 0);

            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    