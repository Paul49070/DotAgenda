using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.View.Popups;
using Microsoft.SqlServer.Management.Smo.Agent;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO.Pipes;
using System.IO;
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
            set 
            { 
                _Fichiers = value;
                OnPropertyChanged(nameof(Fichiers));
            }
        }

        private ObservableCollection<Alerte> _Alertes;
        public ObservableCollection<Alerte> Alertes
        {
            get { return _Alertes; }
            set
            {
                _Alertes = value;
                OnPropertyChanged(nameof(_Alertes));
            }
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

            Alertes = new ObservableCollection<Alerte>();
            
            if(ID == "null")
            {

            }

            /*AjouterAlerte(new Alerte(this, 4, Alerte.TypeMoyenEnvoie.Notification, Alerte.DelaiType.Day));
            AjouterAlerte(new Alerte(this, 8, Alerte.TypeMoyenEnvoie.Notification, Alerte.DelaiType.Hour));
            AjouterAlerte(new Alerte(this, 1, Alerte.TypeMoyenEnvoie.Notification, Alerte.DelaiType.Week));*/
        }

        public void AjouterAlerte(Alerte a)
        {
            MessageBox.Show("Alert to add");
            if (!Alertes.Contains(a))
                Alertes.Add(a);

            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand commandAddAlerte = new SQLiteCommand("INSERT INTO Alertes (UserID, EventID, ID, DateLancement, Mail, Notification) VALUES(@userID, @eventID, @ID, @launch, @mail, @notif)", connection))
                {
                    int mail, notif;
                    if (a.MoyenEnvoi == Alerte.TypeMoyenEnvoie.Both)
                    {
                        mail = 1;
                        notif = 1;
                    }

                    else if (a.MoyenEnvoi == Alerte.TypeMoyenEnvoie.Notification)
                    {
                        notif = 1;
                        mail = 0;
                    }

                    else
                    {
                        notif = 0;
                        mail = 0;
                    }


                    commandAddAlerte.Parameters.AddWithValue("userID", App.ID);
                    commandAddAlerte.Parameters.AddWithValue("eventID", this.ID);
                    commandAddAlerte.Parameters.AddWithValue("ID",a.ID);
                    commandAddAlerte.Parameters.AddWithValue("launch", a.HeureEnvoi.ToString("s"));
                    commandAddAlerte.Parameters.AddWithValue("mail", mail);
                    commandAddAlerte.Parameters.AddWithValue("notif", notif);

                    commandAddAlerte.ExecuteNonQuery();
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
            DeleteAllAlerts();
        }

        private void DeleteAllAlerts()
        {
            foreach(Alerte a in Alertes)
            {
                a.SendAlertPipe("Remove");
            }

            Alertes.Clear();
        }

        public bool AttachFile(Fichier file)
        {
            ObservableCollection<Fichier> FichiersTemp = Fichiers;

            if (FichiersTemp.IndexOf(file) == -1)
            {
                file.AttachToEvent(this);

                FichiersTemp.Insert(0,file);
                Fichiers = FichiersTemp;

                return true;
            }

            else
                return false;
        }


        public bool RemoveFile(Fichier file)
        {
            ObservableCollection<Fichier> FichiersTemp = Fichiers;
            if (FichiersTemp.Remove(file))
            {
                Fichiers = FichiersTemp;
                return true;
            }

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
