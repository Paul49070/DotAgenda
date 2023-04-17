using CommunityToolkit.Mvvm.Input;
using DotAgenda.Core;
using DotAgenda.MethodClass;
using DotAgenda.Models;
using DotAgenda.View.Popups;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static DotAgenda.Models.EventDay;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.ViewModels
{
    class MoreEventPageUsual_ViewModel : FileCommands
    {
        public ICommand DropFile { get; set; }

        private MoreEventPageViewModel _MoreEventViewModel;
        public MoreEventPageViewModel MoreEventViewModel
        {
            get { return _MoreEventViewModel; }
            set
            {
                _MoreEventViewModel = value;
            }
        }

        private EventDay _newEvent;

        public EventDay newEvent
        {
            get { return _newEvent; }
            set
            {
                if (_newEvent != value)
                    _newEvent = value;
            }
        }

        public List<string> HourList { get; }

        private string _SelectedStartTime;
        public string SelectedStartTime
        {
            get { return _SelectedStartTime; }
            set
            {
                if (_SelectedStartTime != value)
                    _SelectedStartTime = value;
            }
        }

        private string _SelectedEndTime;
        public string SelectedEndTime
        {
            get { return _SelectedEndTime; }
            set
            {
                if (_SelectedEndTime != value)
                    _SelectedEndTime = value;
            }
        }

        public List<ClasseEvent_item> EventTypes { get; }
        public ClasseEvent_item CurrentEventType { get; set; }



        public List<string> OrderByList { get; } = new List<string>
        {
            "Nom",
            "Récent",
            "Type",
        };

        private string _SelectedSort;
        public string SelectedSort
        {
            get { return _SelectedSort; }
            set
            {
                if (_SelectedSort != value)
                {
                    _SelectedSort = value;
                    SetOrderOfFile();
                }
            }
        }

        public MoreEventPageUsual_ViewModel()
        {
            DropFile = new RelayCommand<string[]>(Drop);

            this.EventTypes = GlobalDict._dict.DictClasse.Values.ToList();
            HourList = InitHourList();
        }

        private void SetOrderOfFile()
        {
            List<Fichier> Fichiers_temp = newEvent.Fichiers.ToList();

            switch (SelectedSort)
            {

                case "Nom":
                    Fichiers_temp = Fichiers_temp.OrderBy(e => e.Nom).ToList();
                    break;
                case "Récent":
                    Fichiers_temp = Fichiers_temp.OrderBy(e => e.DateAjout).ToList();
                    break;
                case "Type":
                    Fichiers_temp = Fichiers_temp.OrderBy(e => e.Type).ToList();
                    break;
                default: break;
            }

            ObservableCollection<Fichier> Obs_temp = new ObservableCollection<Fichier>();
            foreach(Fichier fic in Fichiers_temp)
            {
                Obs_temp.Add(fic);
            }

            newEvent.Fichiers = Obs_temp;
        }

        private List<string> InitHourList()
        {
            List<string> HourListTemp = new List<string>();

            for (int i = 0; i < 24; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    HourListTemp.Add(String.Format("{0:00}", i) + ":" + String.Format("{0:00}", j * 15));
                }
            }

            return HourListTemp;
        }

        public void Init(MoreEventPageViewModel MoreEventViewModel)
        {
            this.MoreEventViewModel = MoreEventViewModel;
            newEvent = MoreEventViewModel.newEvent;
            SelectedStartTime = newEvent.DateDebut.ToString("HH:mm");
            SelectedEndTime = newEvent.DateFin.ToString("HH:mm");

            try
            {
                CurrentEventType = GlobalDict._dict.DictClasse[newEvent.Classe];
            }

            catch { }

            SelectedSort = OrderByList[1];
        }

        public void Drop(string[] files) //J'ai pas encore trouvé de moyen de le mettre dans le FileCommands pcq  c'est galère de récupérer le DragEventArgs (puis apres de donnée le newEvent en parametre
        {
            foreach (string fic in files)
            {
                Primitives._prim.AddFileToEvent(fic, newEvent);
            }
        }
    }
}
