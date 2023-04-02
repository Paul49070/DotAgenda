using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.Models
{
    public abstract class VisualItem
    {
        private string _Couleur;
        public string Couleur
        {
            get { return _Couleur; }
            set { _Couleur = value; }
        }


        private string _Icon;
        public string Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }

        private string _Titre;
        public string Titre
        {
            get { return _Titre; }
            set { _Titre = value; }
        }

        public VisualItem(string couleur, string icon, string titre)
        {
            Couleur = couleur;
            Icon = icon;
            Titre = titre;
        }
    }
}
