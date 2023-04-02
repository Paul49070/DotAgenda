using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.Models
{
    public class Profil
    {
        DataBase _db = GestionnaireEvent._global._db;

        private int _id;
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }


        private bool _lightMode;
        public bool lightMode
        {
            get { return _lightMode; }
            set 
            { 
                _lightMode = value;
                App.SwitchTheme(_lightMode);
            }
        }


        private bool _AllowMail;
        public bool AllowMail
        {
            get { return _AllowMail; }
            set { _AllowMail = value; }
        }


        private bool _AllowNotification;
        public bool AllowNotification
        {
            get { return _AllowNotification; }
            set { _AllowNotification = value; }
        }


        private string _Mail;
        public string Mail
        {
            get { return _Mail; }
            set { _Mail = value; }
        }


        private string _Prenom;
        public string Prenom
        {
            get { return _Prenom; }
            set { _Prenom = value; }
        }


        private string _Nom;
        public string Nom
        {
            get { return _Nom; }
            set { _Nom = value; }
        }

        public Profil(int ID, bool light, bool bmail, bool notif, string mail, string prenom, string nom)
        {
            id = ID;
            lightMode = light;
            AllowMail = bmail;
            AllowNotification = notif;

            Mail = mail;

            Prenom = prenom;
            Nom = nom;
        }
    }
}
