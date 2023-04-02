using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Windows.UI.Xaml;

namespace DotAgenda.Models
{
    public class TodoItem : BaseItem
    {
        private Priorite _Priority;
        public Priorite Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }


        private States _Status;
        public States Status
        {
            get { return _Status; }
            set { _Status = value; }
        }



        private DateTime _ModifiedDate;
        public DateTime ModifiedDate 
        {
            get { return _ModifiedDate; }
            set { _ModifiedDate = value; }
        }


        private List<Label> _Labels;
        public List<Label> Labels
        {
            get { return _Labels; }
            set { _Labels = value; }
        }


        public TodoItem(string titre, DateTime debut, string classe, bool fini, int ID = -1, string description = "", 
            Priorite priorite = Priorite.Medium, States status = States.NotStarted, DateTime fin = default)

            :base(ID,titre,debut,fin,description,classe,fini)
        {
            Priority = priorite;
            Status = status;

            Labels = new List<Label>();
            ModifiedDate = DateTime.Now;
        }


        public enum Priorite
        {
            High,
            Medium,
            Low
        }


        public enum States
        {
            NotStarted,
            InProgress,
            Completed
        }


        public static Dictionary<string, string> ListeEtiquettes { get; set; } = new Dictionary<string, string>()
        {
                { "Travail", "#ff5733"},
                { "Perso", "#ffa33b"},
                { "Urgent", "#ff3333"},
                { "Santé", "#33ff57"},
                { "Voyage", "#33aaff"},
                { "Shopping", "#b033ff"},
                { "Divers", "#bfbfbf"},
        };

        public bool AddLabelToList(string name)
        {
            bool _succes = true;
            Label L1 = new Label(name);

            if(L1.Couleur == "")
                return !_succes;

            else
            {
                if (Labels.IndexOf(L1) == -1)
                {
                    Labels.Add(L1);
                    return _succes;
                }
                else return !_succes;
            }
        }

        public bool RemoveLabelFromList(string name)
        {
            bool _succes = true;
            Label L1 = new Label(name);

            if (Labels.IndexOf(L1) == -1)
                return !_succes;

            else
            {
                Labels.Remove(L1);
                return _succes;
            }
        }

        public class Label
        {
            public string Nom { get; }
            public string Couleur { get; }
            public Label(string name)
            {
                Nom = name;
                this.Couleur = AddLabel(name);
            }

            public string AddLabel(string name)
            {
                try
                {
                    return TodoItem.ListeEtiquettes[name];
                }

                catch
                {
                    Console.WriteLine("Pas dans le dico des etiquettes");
                    return "";
                }
            }

            
        }
    }
}
