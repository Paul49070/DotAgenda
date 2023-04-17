using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.Models
{
    public class AlertProtocolJSON
    {
        [JsonProperty("id")]
        private string _ID;

        public string ID
        {
            get { return _ID; }
        }

        [JsonProperty("eventID")]
        private string _EventID;

        public string EventID
        {
            get { return _EventID; }
        }

        [JsonProperty("dateDebut")]

        private DateTime _Start;

        public DateTime Start
        {
            get { return _Start; }
            set
            {
                _Start = value;
            }
        }

        [JsonProperty("dateFin")]

        private DateTime _End;

        public DateTime End
        {
            get { return _End; }
            set
            {
                _End = value;
            }
        }

        [JsonProperty("launchingDate")]

        private DateTime _Launch;
        public DateTime Launch
        {
            get { return _Launch; }
            set
            {
                _Launch = value;
            }
        }

        [JsonProperty("mail")]

        private bool _Mail;
        public bool Mail
        {
            get { return _Mail; }
            set
            {
                if (_Mail != value)
                    _Mail = value;
            }
        }


        [JsonProperty("notif")]

        private bool _Notif;
        public bool Notif
        {
            get { return _Notif; }
            set
            {
                if (_Notif != value)
                    _Notif = value;
            }
        }

        [JsonProperty("titre")]

        private string _Titre;
        public string Titre
        {
            get { return _Titre; }
            set
            {
                if (_Titre != value)
                    _Titre = value;
            }
        }

        [JsonProperty("classe")]

        private string _Classe;
        public string Classe
        {
            get { return _Classe; }
            set
            {
                if (_Classe != value)
                    _Classe = value;
            }
        }

        [JsonProperty("type")]

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set
            {
                if (value != _Type)
                    _Type = value;
            }
        }

        [JsonProperty("email")]

        private string _Email;
        public string Email
        {
            get { return _Email; }
            set
            {
                if (value != _Email)
                    _Email = value;
            }
        }


        [JsonProperty("prenom")]

        private string _Prenom;
        public string Prenom
        {
            get { return _Prenom; }
            set
            {
                if (value != _Prenom)
                    _Prenom = value;
            }
        }


        [JsonProperty("nom")]

        private string _Nom;
        public string Nom
        {
            get { return _Nom; }
            set
            {
                if (value != _Nom)
                    _Nom = value;
            }
        }


        public AlertProtocolJSON(Alerte a, string Type)
        {
            this._ID = a.ID;
            this._EventID = a.Evenement.ID;
            this.Start = a.Evenement.DateDebut;
            this.End = a.Evenement.DateFin;
            this.Launch = a.HeureEnvoi;

            this.Titre = a.Evenement.Titre;
            this.Classe = a.Evenement.Classe;
            this.Type = Type;

            if (a.MoyenEnvoi == Alerte.TypeMoyenEnvoie.Both)
            {
                this.Mail = true; this.Notif = true;
            }
            else if (a.MoyenEnvoi == Alerte.TypeMoyenEnvoie.Notification)
            {
                this.Mail = false; this.Notif = true;
            }

            else
            {
                this.Mail = true; this.Notif = false;
            }

            this.Email = App.User.Mail;
            this.Prenom = App.User.Prenom;
            this.Nom = App.User.Nom;
        }
    }
}
