using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.Models
{
    public class Dossier : VisualItem
    {
        private ObservableCollection<Fichier> _ListeFichiers;
        public ObservableCollection<Fichier> ListeFichiers
        {
            get { return _ListeFichiers; }
            set { _ListeFichiers = value; }
        }


        private bool _Type;
        public bool Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public Dossier(string nom = "", bool type = true, string couleur = "", string icon = "") : base(couleur, icon, nom)
        {
            ListeFichiers = new ObservableCollection<Fichier>();
            Type = type;
        }
    }
}
