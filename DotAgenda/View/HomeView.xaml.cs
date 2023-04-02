using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static DevExpress.Xpo.Helpers.PerformanceCounters;

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        GestionnaireEvent _global;
        DataBase _db;
        GlobalDict _dict;
        Primitives _prim;


        public static TextBlock MonthStatic;
        public static Calendar tempCal;

        Dictionary<string, bool> ClasseChecked = new Dictionary<string, bool>();


        public HomeView()
        {
            _global = GestionnaireEvent._global;

            _dict = _global._dict;
            _db = _global._db;
            _prim = _global._prim;

            InitializeComponent();

            tempCal = Calendrier;

            Calendrier.SelectedDate = _global._currentDay.Date;
            BoxClasse.ItemsSource = _dict.DictClasse.Values.ToList();

            foreach(string classe in _dict.DictClasse.Keys)
            {
                ClasseChecked.Add(classe, true);
            }
        }

        void AddClassToView(object sender, RoutedEventArgs e)
        {
            CheckBox CheckBox_temp = (sender as CheckBox);
            string classe = CheckBox_temp.Content.ToString();

            int numY = _global._currentDay.Date.Year - DateTime.Today.Year + 1;
            int today_Day = _global._currentDay.Date.Day - 1;
            int today_M = _global._currentDay.Date.Month - 1;

            try
            {
                ClasseChecked[classe] = !ClasseChecked[classe];

                foreach (EventDay item in _global.A[numY].M[today_M].J[today_Day].ListeEvent)
                {
                    if (item.Classe == classe)
                    {
                        item.IsVisible = ClasseChecked[classe];
                    }
                }
            }

            catch { Console.WriteLine("Mauvais nom de classe"); }
        }

        private void MonthlyCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            _global._currentDay.Date = Calendrier.SelectedDate.Value;
            _prim.SetCurrentDate();
        }
        
        public static void ActCalendrier(DateTime Date)
        {
            tempCal.SelectedDate = Date;
            tempCal.DisplayDate = Date;
        }
        
        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }
    }
}
