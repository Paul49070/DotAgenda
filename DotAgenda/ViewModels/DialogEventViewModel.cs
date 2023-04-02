using DotAgenda.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.ViewModels
{
    class DialogEventViewModel : ObservableObject
    {
        public static List<DateTime> Dates { get; } = new List<DateTime>();

        public RelayCommand DialogEventViewModel_Evenement { get; set; }

        public RelayCommand DialogEventViewModel_Tache { get; set; }

        public DialogEventViewModel_Evenement DialogVM_E { get; set; }
        public DialogEventViewModel_Tache DialogVM_T { get; set; }

        private object _currentView_sub;
        public object CurrentView_Sub
        {
            get { return _currentView_sub; }
            set
            {
                _currentView_sub = value;
                OnPropertyChanged();
            }
        }

        public DialogEventViewModel()
        {
            DialogVM_E = new DialogEventViewModel_Evenement();
            DialogVM_T = new DialogEventViewModel_Tache();

            CurrentView_Sub = DialogVM_E;

            DialogEventViewModel_Evenement = new RelayCommand(o =>
            {
                CurrentView_Sub = DialogVM_E;
            });

            DialogEventViewModel_Tache = new RelayCommand(o =>
            {
                CurrentView_Sub = DialogVM_T;
            });
        }
    }
}
