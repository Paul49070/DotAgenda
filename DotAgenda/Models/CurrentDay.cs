using DotAgenda.MethodClass;
using DotAgenda.View;
using DotAgenda.ViewModels;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

namespace DotAgenda.Models
{
    public class CurrentDay : INotifyPropertyChanged
    {
        private DateTime _currentDate;
        public DateTime Date
        {
            get { return _currentDate; }

            set { 

                if(_currentDate != value)
                {
                    _currentDate = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public event EventHandler<ListChangedEventArgs> ListChanged;


        private ObservableCollection<EventDay> _ListeEvent;
        public ObservableCollection<EventDay> ListeEvent 
        {
            get { return this._ListeEvent; }
            set 
            {
                if (_ListeEvent != value)
                {
                    _ListeEvent = value;
                    OnPropertyChanged(nameof(ListeEvent));
                }

                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }


        private ObservableCollection<TodoItem> _Todo;
        public ObservableCollection<TodoItem> Todo
        {
            get { return this._Todo; }
            set
            {
                if (_Todo != value)
                {
                    _Todo = value;
                    OnPropertyChanged(nameof(Todo));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
