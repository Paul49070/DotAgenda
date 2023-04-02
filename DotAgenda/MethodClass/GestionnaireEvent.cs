using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.MethodClass
{
    public class GestionnaireEvent
    {
        private static readonly GestionnaireEvent events = new GestionnaireEvent();

        public static GestionnaireEvent _global => events;

        public CurrentDay _currentDay = new CurrentDay();
        public ObservableCollection<Dossier> ListeDossiersType;
        public ObservableCollection<Fichier> ListeFichiers;
        public ObservableCollection<EventDay> ListeEventWeek;

        public ObservableCollection<EventDay> NextEvent { get; set; }
        public ObservableCollection<DateTime> JoursAvecUnEvenement { get; set; } = new ObservableCollection<DateTime>();

        public Annee[] A;

        public DataBase _db;
        public GlobalDict _dict;
        public Primitives _prim;

        private GestionnaireEvent() { }

        public Annee[] InitArray()
        {
            NextEvent = new ObservableCollection<EventDay>();

            A = new Annee[4];
            InitA();
            _db.Event.InitEvent();
            _db.Todo.InitTodo();
            _db.Event.InitFileEvent();


            return A;
        }


        public void InitA()
        {

            int TodayYear = DateTime.Now.Year - 1;
            for (int k = 0; k < 4; ++k)
            {
                A[k] = new Annee();
                A[k].Num = TodayYear + k;
                A[k].Bisextile = false;

                for (int i = 0; i < 12; ++i)
                {
                    //Init du mois

                    A[k].M[i] = new Mois(_dict.DictMois.ElementAt(i).Value);

                    for (int p = 0; p < _dict.DictClasse.Count; ++p)
                    {
                        A[k].M[i].NbParClasse.Add(_dict.DictClasse.Keys.ElementAt(p), 0);
                    }

                    for (int j = 0; j < A[k].M[i].J.Count(); ++j)
                    {
                        //Init du jour
                        A[k].M[i].J[j] = new Jours();

                        DateTime temp = new DateTime(TodayYear + k, i + 1, j + 1);

                        A[k].M[i].J[j].Date = temp;
                        A[k].M[i].J[j].ListeEvent = new ObservableCollection<EventDay>();
                        A[k].M[i].J[j].Todo = new ObservableCollection<TodoItem>();
                    }
                }
            }
        }

        public ObservableCollection<Fichier> InitListeFichiers()
        {
            return _db.File.SetListeFichiers();
        }

        public ObservableCollection<Dossier> InitFolderType()
        {
            return _prim.InitFolderType();
        }
    }


}
