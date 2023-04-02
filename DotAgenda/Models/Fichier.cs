using DotAgenda.MethodClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DotAgenda.Models
{
    public class Fichier
    {
        private GestionnaireEvent _global;

        private string _Nom;
        public string Nom
        {
            get { return _Nom; }
            set { _Nom = value; }
        }


        private string _Contenu;
        public string Contenu
        {
            get { return _Contenu; }
            set { _Contenu = value; }
        }


        private DateTime _DateAjout;
        public DateTime DateAjout
        {
            get { return _DateAjout; }
            set { _DateAjout = value; }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }


        private string _ID;
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private List<EventDay> _AttachedEvent;
        public List<EventDay> AttachedEvent
        {
            get { return _AttachedEvent; }
            set { _AttachedEvent = value; }
        }


        public Fichier(string ID, string Nom, DateTime DateAjout = default(DateTime))
        {
            _global = GestionnaireEvent._global;

            this.ID = ID;
            this.Nom = Nom;

            try
            {
                FileInfo fi = new FileInfo(Nom);
                Type = _global._dict.ExtensionDict[fi.Extension];
            }

            catch
            {
                Type = _global._dict.ExtensionDict["null"];
            }

            if(DateAjout != default(DateTime))
                this.DateAjout = DateTime.Today;

            AttachedEvent = new List<EventDay>();

            AddToFolderType();
            _global.ListeFichiers.Add(this);
        }

        public void AddToFolderType()
        {            
            foreach (Dossier folder in _global.ListeDossiersType)
            {
                if(folder.Type)
                {
                    if(folder.Titre == Type)
                    {
                        if (folder.ListeFichiers.IndexOf(this) == -1)
                            folder.ListeFichiers.Add(this);
                    }
                }
            }
        }

        public void RemoveFromDossierType()
        {
            foreach (Dossier folder in _global.ListeDossiersType)
            {
                if (folder.Type)
                {
                    if (folder.Titre == Type)
                        folder.ListeFichiers.Remove(this);
                }
            }
        }

        public void AddToFolderCategorie()
        {

        }

        public void AttachToEvent(EventDay evenement)
        {
            AttachedEvent.Add(evenement);
        }

        public void DetachAll()
        {
            foreach(EventDay item in this.AttachedEvent)
            {
                item.RemoveFile(this);
            }
        }
    }
}
