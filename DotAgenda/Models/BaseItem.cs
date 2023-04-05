using System;
using DotAgenda.MethodClass;

namespace DotAgenda.Models
{
    public abstract class BaseItem
    {
        private string _Titre;
        public string Titre
        {
            get { return _Titre; }
            set { _Titre = value; }
        }


        private bool _Fini;
        public bool Fini
        {
            get { return _Fini; }
            set { _Fini = value; }
        }


        private string _Classe;
        public string Classe
        {
            get { return _Classe; }
            set { _Classe = value; }
        }


        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }


        private DateTime _DateDebut;

        public DateTime DateDebut
        {
            get { return _DateDebut; }
            set { _DateDebut = value; }
        }


        private DateTime _DateFin;
        public DateTime DateFin
        {
            get { return _DateFin; }
            set { _DateFin = value; }
        }


        private string _ID;
        public string ID
        {
            get { return _ID; }
        }


        public BaseItem(string titre, DateTime debut, DateTime fin, string ID = "null", string description = "", string classe = "", bool fini = false)
        {
            if (ID == "null")
                this._ID = Primitives._prim.GenerateID();
            else this._ID = ID;

            DateDebut = debut;
            DateFin = fin;
            Description = description;
            Titre = titre;
            Classe = classe;
            Fini = fini;

        }
    }
}
