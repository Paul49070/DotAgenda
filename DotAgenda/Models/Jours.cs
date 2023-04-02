using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.View;
using DotAgenda.ViewModels;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Web.UI.Design;
using System.Windows;

namespace DotAgenda.Models
{
    public class Jours
    {
        public static event EventHandler<EventDay> EventAdded;

        GestionnaireEvent _global;
        DataBase _db;
        Primitives _prim;
        GlobalDict _dict;


        private DateTime _Date;
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        private ObservableCollection<EventDay> _ListeEvent;
        public ObservableCollection<EventDay> ListeEvent
        {
            get { return _ListeEvent; }
            set { _ListeEvent = value; }
        }


        private ObservableCollection<TodoItem> _Todo;
        public ObservableCollection<TodoItem> Todo
        {
            get { return _Todo; }
            set { _Todo = value; }
        }

        public Jours()
        {
            _global = GestionnaireEvent._global;

            _dict = _global._dict;
            _db = _global._db;
            _prim = _global._prim;

            ListeEvent = new ObservableCollection<EventDay>();
            Todo = new ObservableCollection<TodoItem>();
        }



        public void AjouterEvent(EventDay EventAdd)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            EventAdd.Fini = _prim.Event_Fini(EventAdd.DateDebut);

            EventAdd.ID = _db.Event.AddEventToDB(EventAdd);

            if (EventAdd.ID != -1)
            {
                if (AddEventToList(EventAdd))
                {
                    if (_mainView.ListeEventWeek.Count() > 0)
                        _mainView.ListeEventWeek.Move(0, 1);

                    EventAdded?.Invoke(this, EventAdd);
                }

                else Console.WriteLine("Evenement déjà présent dans les listes.");
            }

            else Console.WriteLine("Evenement déjà présent dans la base de donnée.");

        }

        public void DeleteEvent(EventDay EventRemove)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            if (DeleteEventToList(EventRemove))
            {
                _db.Event.DeleteEventToDB(EventRemove);

                if (_mainView.ListeEventWeek.Count() > 0)
                {
                    _mainView.ListeEventWeek.Move(0, 1);
                }
            }
        }


        public bool AddEventToList(EventDay EventAdd)
        {
            int index = _prim.Tri(EventAdd.DateDebut);

            if (ListeEvent.IndexOf(EventAdd) == -1)
            {
                ListeEvent.Insert(index, EventAdd);

                if (!_global.JoursAvecUnEvenement.Contains(EventAdd.DateDebut.Date))
                    _global.JoursAvecUnEvenement.Add(EventAdd.DateDebut.Date);

                if (_global._currentDay.Date.Date == EventAdd.DateDebut.Date)
                    _prim.SetCurrentDate();
                

                if (_global.NextEvent.Count > 0)
                {
                    if (DateTime.Now < EventAdd.DateDebut.Date && EventAdd.DateDebut.Date < _global.NextEvent[0].DateDebut.Date)
                    {
                        _global.NextEvent.Clear();
                        _global.NextEvent.Add(EventAdd);
                    }
                }

                return true;
            }

            else
            {
                Console.WriteLine("Evenement déjà ajouté");
                return false;
            }
        }


        public bool DeleteEventToList(EventDay EventRemove)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            if (ListeEvent.IndexOf(EventRemove) != -1)
            {
                ListeEvent.Remove(EventRemove);

                if (ListeEvent.Count == 0)
                    _global.JoursAvecUnEvenement.Remove(EventRemove.DateDebut.Date);

                if (_global._currentDay.Date.Date == EventRemove.DateDebut.Date)
                    _prim.SetCurrentDate();

                if (_global.NextEvent.Count > 0)
                {
                    if (EventRemove == _global.NextEvent[0])
                    {
                        _global.NextEvent.Clear();
                        _global.NextEvent.Add(_prim.SearchNextEvent(_mainView.delai_nextEvent, DateTime.Today));
                    }
                }

                //Remove from class

                int i = _dict.DictClasse.Keys.ToList().IndexOf(EventRemove.Classe);
                //_mainView.Classe[i].Remove(EventRemove);

                return true;
            }
            else
            {
                Console.WriteLine("Evenement déjà retiré");
                return false;
            }
        }


        public bool AjouterTodoToList(int index, TodoItem TodoAdd)
        {
            if(Todo.IndexOf(TodoAdd) == -1)
            {
                var _mainView = (MainWindow)Application.Current.MainWindow;

                Todo.Insert(index, TodoAdd);

                if (_global._currentDay.Date.Date == TodoAdd.DateDebut.Date)
                    _prim.SetCurrentDate();

                return true;
            }

            return false;
        }


        public bool DeleteTodo(TodoItem Tache)
        {
            if (Todo.IndexOf(Tache) != -1)
            {
                Todo.Remove(Tache);
                _db.Todo.DeleteTodoDB(Tache);
                _prim.SetCurrentDate();

                return true;
            }

            else return false;
        }

    }
}
