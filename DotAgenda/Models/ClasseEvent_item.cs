using DotAgenda.MethodClass;
using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DotAgenda.Models
{
    public class ClasseEvent_item : VisualItem
    {
        private GestionnaireEvent _global;


        private int _Pourcentage;
        public int Pourcentage
        {
            get { return _Pourcentage; }
            set { _Pourcentage = value; }
        }


        public ClasseEvent_item(string titre ="", string couleur = "", string icon = "") : base(couleur, icon, titre)
        {
            _global = GestionnaireEvent._global;
            Pourcentage = 0;
        }

        public void Add(EventDay Event)
        {
            _global.A[Event.DateDebut.Year - DateTime.Today.Year + 1].M[Event.DateDebut.Month - 1].NbParClasse[Event.Classe] += 1;
        }

        public void Remove(EventDay Event)
        {
            _global.A[Event.DateDebut.Year - DateTime.Today.Year + 1].M[Event.DateDebut.Month - 1].NbParClasse[Event.Classe] -= 1;
        }
    }
}
