using DotAgenda.Core;
using DotAgenda.MethodClass;
using DotAgenda.Models;
namespace DotAgenda.ViewModels
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand SecondViewCommand { get; set; }
        public RelayCommand ThirdViewCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public SecondViewModel SecondVM { get; set; }
        public ThirdViewModel ThirdVM { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value; 
                OnPropertyChanged();
            }
        }

        public Profil currentUser { get; } = new Profil(-1,false,false,false,"","","");

        public MainViewModel()
        {
            //currentUser = App.User;

            HomeVM = new HomeViewModel();
            SecondVM = new SecondViewModel();
            ThirdVM = new ThirdViewModel();
            CurrentView = null;

            HomeViewCommand = new RelayCommand(o => { CurrentView = HomeVM;});

            SecondViewCommand = new RelayCommand(o => { CurrentView = SecondVM;});

            ThirdViewCommand= new RelayCommand(o => { CurrentView = ThirdVM; });


            GestionnaireEvent _global = GestionnaireEvent._global;


        }
    }
}
