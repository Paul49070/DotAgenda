using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DotAgenda.Models
{
    public class Mois
    {

        private Jours[] _J;
        public Jours[] J
        {
            get { return _J; }
            set { _J = value; }
        }

        private Dictionary<string, int> _NbParClasse;
        public Dictionary<string, int> NbParClasse
        {
            get { return _NbParClasse; }
            set { _NbParClasse = value; }
        }


        public Mois(int nbJours)
        {
            J = new Jours[nbJours];
            NbParClasse = new Dictionary<string, int>();
        }


        public void TriClasseMois()
        {
            var ListTemp = NbParClasse.ToList();

            ListTemp.Sort((i1, i2) => -i1.Value.CompareTo(i2.Value));

            NbParClasse.Clear();

            foreach (var item in ListTemp)
            {
                NbParClasse.Add(item.Key, item.Value);
            }
        }
    }
}
