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
        public ObservableCollection<Fichier> ListeFichiers { get; set; }
        public ObservableCollection<EventDay> ListeEventWeek;

        public ObservableCollection<EventDay> NextEvent { get; set; }
        public ObservableCollection<DateTime> JoursAvecUnEvenement { get; set; } = new ObservableCollection<DateTime>();

        public Annee[] A;

        private GestionnaireEvent() { }

        public void Init()
        {
            A = new Annee[App.LastDate.Year - App.StartDate.Year + 1];

            NextEvent = new ObservableCollection<EventDay>();
            ListeFichiers = new ObservableCollection<Fichier>();
            ListeDossiersType = InitFolderType();
            DataBaseFile._dbFile.SetListeFichiers();

            InitA();

            DataBaseEvents._dbEvent.InitEvent();
            DataBaseTodo._dbTodo.InitTodo();
            DataBaseEvents._dbEvent.InitFileEvent();
        }
        

        public void InitA()
        {
            GlobalDict _dict = GlobalDict._dict;

            int TodayYear = DateTime.Now.Year - 1;

            for (int k = 0; k < A.Count(); ++k)
            {
                A[k] = new Annee();
                A[k].Num = App.StartDate.Year + k;


                for (int i = 0; i < 12; ++i)
                {
                    if (i == 1 && A[k].Bisextile)
                        _dict.DictMois["Fevrier"] = 29;
                    else _dict.DictMois["Fevrier"] = 28;

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

        public ObservableCollection<Dossier> InitFolderType()
        {
            return Primitives._prim.InitFolderType();
        }
    }


}
