using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
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


        public Fichier(string Nom, string ID = "null", DateTime DateAjout = default)
        {
            
            _global = GestionnaireEvent._global;

            this.Nom = Nom;

            if (ID == "null")
                this.ID = Primitives._prim.GenerateID();
            else this.ID = ID;

            if (DateAjout == default)
                this.DateAjout = DateTime.Today;
            else this.DateAjout = DateAjout;


            try
            {
                FileInfo fi = new FileInfo(Nom);
                Type = GlobalDict._dict.ExtensionDict[fi.Extension];
            }

            catch
            {
                Type = GlobalDict._dict.ExtensionDict["null"];
            }


            AttachedEvent = new List<EventDay>();


            AddFileToDict();
            AddToFileList();
            AddToFolderType();
        }

        private void AddFileToDict()
        {
            //Cette vérif est normalement inutile mais on sait jamais

            if (GlobalDict._dict.DictFile.Keys.ToList().IndexOf(this.Nom) == -1)
                GlobalDict._dict.DictFile.Add(this.Nom, this);
        }

        private void AddToFileList()
        {
            if(_global.ListeFichiers.IndexOf(this)==-1)
            {
                _global.ListeFichiers.Add(this);
            }
        }

        private void AddToFolderType()
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
