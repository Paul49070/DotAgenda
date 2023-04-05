using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;


namespace DotAgenda.Models
{
    public class EventDay : BaseItem, INotifyPropertyChanged
    {
        private Reccurences _Reccurence;
        public Reccurences Reccurence
        {
            get { return _Reccurence; }
            set { _Reccurence = value; }
        }


        private ObservableCollection<Fichier> _Fichiers;
        public ObservableCollection<Fichier> Fichiers
        {
            get { return _Fichiers; }
            set { _Fichiers = value; }
        }


        private string _Lieu;
        public string Lieu
        {
            get { return _Lieu; }
            set { _Lieu = value; }
        }


        private string _GroupID;
        public string GroupID
        {
            get { return _GroupID; }
        }
        

        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }



        public EventDay(string titre, DateTime startTime , DateTime endTime, string classe, string ID = "null", string GroupID = "", string location = "", string desc = "", 
            Reccurences reccurence = default, ObservableCollection<Fichier> fic = default, bool visible = true) 
            : base(titre, startTime, endTime, ID, desc, classe)
        {        
            Lieu = location;        
            IsVisible = visible;

            if (reccurence == default)
                Reccurence = new Reccurences();
            else Reccurence = reccurence;

            if (fic == default)
                Fichiers = new ObservableCollection<Fichier>();
            else Fichiers = fic;

            _GroupID = GroupID;

            if(this.GroupID != "")
            {
                try 
                {
                    GlobalDict._dict.DictGroupEvent[this.GroupID].AddEventToListe(this);
                }

                catch
                {
                    new GroupEvent(this.GroupID, this);
                }
            }
        }

        public EventDay Clone(bool GenerateNewGroupID = false)
        {
            if(GenerateNewGroupID)
                return new EventDay(Titre, DateDebut, DateFin, Classe, ID, Primitives._prim.GenerateID(), Lieu, Description, Reccurence, Fichiers, IsVisible);
            else return new EventDay(Titre, DateDebut, DateFin, Classe, ID, GroupID, Lieu, Description, Reccurence, Fichiers, IsVisible);

        }

        public void DeleteEventFromGroup()
        {
            if (this.GroupID != "")
            {
                try
                {
                    GlobalDict._dict.DictGroupEvent[this.GroupID].DeleteEventFromListe(this);
                }

                catch
                {
                }
            }
        }

        public void RemoveEvent()
        {
            int numY = DateDebut.Year - DateTime.Today.Year + 1;
            int numM = DateDebut.Month - 1;
            int numJ = DateDebut.Day - 1;

            DeleteEventFromGroup();
            GestionnaireEvent._global.A[numY].M[numM].J[numJ].DeleteEventToList(this);
            DataBaseEvents._dbEvent.DeleteEventToDB(this);
        }

        public bool AddFile(Fichier file)
        {
            if (Fichiers.IndexOf(file) == -1)
            {
                file.AttachToEvent(this);
                Fichiers.Add(file);
                return true;
            }

            else
                return false;
        }


        public bool RemoveFile(Fichier file)
        {
            if (Fichiers.Remove(file))
                return true;
            else return false;
        }


        public bool RemoveFileByIndex(int index)
        {
            try
            {
                Fichiers.RemoveAt(index);
            }

            catch
            {
                return false;
            }

            return true;
        }


        public class Reccurences
        {
            public RepeatType Repeat;
            public int ForXtime;
            public int EveryXtime; //Tout les 3 jours pour 5 fois : ForXtime = 5, EveryXtime = 3 avec frequency en daily

            public Reccurences()
            {
                Repeat = RepeatType.None;
                ForXtime = -1; // -> infini
                EveryXtime = 1;
            }
        }


        public enum RepeatType
        {   
            Daily = 0,
            Weekly = 1,
            Monthly = 2,
            Yearly = 3,
            None = -1,
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
