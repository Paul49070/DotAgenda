using DotAgenda.Models;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour HomeView_Week.xaml
    /// </summary>
    public partial class HomeView_Week : UserControl
    {
        public ObservableCollection<Jours> DayList = new ObservableCollection<Jours>();
        public DateTime _currentDate;

        GestionnaireEvent _global;
        DataBase _db;
        GlobalDict _dict;
        Primitives _prim;
        public HomeView_Week()
        {
            InitializeComponent();

            _global = GestionnaireEvent._global;

            _dict = _global._dict;
            _db = _global._db;
            _prim = _global._prim; ;

            var _mainView = (MainWindow)Application.Current.MainWindow;

            ActList(_global._currentDay.Date);

            ((INotifyCollectionChanged)ListViewDay.Items).CollectionChanged += ListView_CollectionChanged;
            ListViewDay.ItemsSource = _global.ListeEventWeek;
        }


        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ActList(_global._currentDay.Date);
            }

            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                ActList(_currentDate);
            }
        }

        private void TodayUpdate(object sender, RoutedEventArgs e)
        {
            HomeView.ActCalendrier(DateTime.Today);
            ActList(DateTime.Today);

        }

        private void MoinsJours(object sender, RoutedEventArgs e)
        {
            ActList(_currentDate.AddDays(-7));
        }

        private void PlusJours(object sender, RoutedEventArgs e)
        {
            ActList(_currentDate.AddDays(7));

        }

        public void ActList(DateTime debut)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;


            while(_mainView.ListeEventWeek.Count()>0)
            {
                _mainView.ListeEventWeek.RemoveAt(0);
            }
                        
            _currentDate = debut;
            DateTime temp = _currentDate;
            for (int i = 0; i < 7; ++i)
            {
                _mainView.ListeEventWeek.Add(_global.A[temp.Year - DateTime.Today.Year + 1].M[temp.Month - 1].J[temp.Day - 1]);
                temp = temp.AddDays(1);
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }

        private void MoreEventClick(object sender, RoutedEventArgs e)
        {
            ListView lst = sender as ListView;
            Button btn = sender as Button;
            var _mainView = (MainWindow)Application.Current.MainWindow;

            int pos = ListViewDay.Items.IndexOf(lst);

            //var lst_temp = ListViewDay.Items[pos];

            Console.WriteLine(btn);

            /*var event_window = new MoreEventPage(_mainView.semaine[0].ListeEvent[pos], pos);
            event_window.Owner = Window.GetWindow(this);
            event_window.Show();

            e.Handled = true;*/
        }
    }
}
