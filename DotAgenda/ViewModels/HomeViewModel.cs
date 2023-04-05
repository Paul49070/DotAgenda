using DotAgenda.Core;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using DotAgenda.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DotAgenda.ViewModels
{
    class HomeViewModel : ObservableObject, INotifyPropertyChanged
    {
        GestionnaireEvent _global;
        DataBase _db;
        GlobalDict _dict;
        Primitives _prim;

        public ObservableCollection<DateTime> Dates { get; set; }

        public RelayCommand HomeViewCommand_Day { get; set; }

        public RelayCommand HomeViewCommand_Week { get; set; }

        public RelayCommand HomeViewCommand_Month { get; set; }
        public HomeViewModel_Day HomeVM_D { get; set; }
        public HomeViewModel_Week HomeVM_W { get; set; }

        public HomeViewModel_Month HomeVM_M { get; set; }

        private object _currentView_sub;
        public object CurrentView_Sub
        {
            get { return _currentView_sub; }
            set
            {
                _currentView_sub = value;
                OnPropertyChanged();
            }
        }

        public HomeViewModel()
        {
            _global = GestionnaireEvent._global;

            _dict = GlobalDict._dict;
            _db = DataBase._db;
            _prim = Primitives._prim;

            Jours.EventAdded += OnEventAdded;

            Dates = _global.JoursAvecUnEvenement;
            Dates.CollectionChanged += Dates_CollectionChanged;

            HomeVM_D = new HomeViewModel_Day();
            HomeVM_W = new HomeViewModel_Week();
            HomeVM_M = new HomeViewModel_Month();

            CurrentView_Sub = HomeVM_D;

            HomeViewCommand_Day = new RelayCommand(o =>
            {
                CurrentView_Sub = HomeVM_D;
            });

            HomeViewCommand_Week = new RelayCommand(o =>
            {
                CurrentView_Sub = HomeVM_W;
            });

            HomeViewCommand_Month = new RelayCommand(o =>
            {
                CurrentView_Sub = HomeVM_M;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private new void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Dates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Dates));
        }

        public void AddSpeDay(DateTime date)
        {
            DateTime temp = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            Dates.Add(temp);
        }

        private void OnEventAdded(object sender, EventDay newEvent)
        {
            ObservableCollection<DateTime> temp = Dates;
            temp.Add(newEvent.DateDebut);
            Dates = temp;

            OnPropertyChanged(nameof(Dates));
        }
    }
}
