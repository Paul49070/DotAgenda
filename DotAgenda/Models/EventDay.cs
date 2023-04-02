using DotAgenda.View;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

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


        private int _GroupID;
        public int GroupID
        {
            get { return _GroupID; }
            set { _GroupID = value; }
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



        public EventDay(string id, string titre, DateTime startTime , DateTime endTime, string location, string description, string classe, bool visible = true) 
            : base(id, titre, startTime, endTime, description, classe)
        {        
            Lieu = location;        
            IsVisible = visible;
            Reccurence = new Reccurences();
            Fichiers = new ObservableCollection<Fichier>();


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
            public Frequency Frequency;
            public int ForXtime;
            public int EveryXtime; //Tout les 3 jours pour 5 fois : ForXtime = 5, EveryXtime = 3 avec frequency en daily

            public Reccurences()
            {
                Frequency = Frequency.JustOneTime;
                ForXtime = -1; // -> infini
                EveryXtime = 1;
            }
        }


        public enum Frequency
        {   
            Daily = 0,
            Weekly = 1,
            Monthly = 2,
            Yearly = 3,
            JustOneTime = -1,
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
