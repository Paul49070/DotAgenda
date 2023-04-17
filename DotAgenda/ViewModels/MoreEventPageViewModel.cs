using DotAgenda.Core;
using DotAgenda.Models;
using System;
using System.Windows;

namespace DotAgenda.ViewModels
{
    class MoreEventPageViewModel : ObservableObject
    {
        public RelayCommand ChangeViewCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        public MoreEventPageUsual_ViewModel MoreVM { get; set; }
        public MoreEventPageAdvancedViewModel MoreAdvancedVM { get; set; }

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

        private EventDay _newEvent;
        public EventDay newEvent
        {
            get { return _newEvent; }
            set 
            { 
                if(_newEvent != value)
                {
                    _newEvent = value;
                }
            }
        }

        private EventDay _oldEvent;
        public EventDay oldEvent
        {
            get { return _oldEvent; }
            set
            {
                if (_oldEvent != value)
                {
                    _oldEvent = value;
                }
            }
        }

        public MoreEventPageViewModel(EventDay DetailOne)
        {
            oldEvent = DetailOne;
            newEvent = oldEvent.Clone();

            MoreVM = new MoreEventPageUsual_ViewModel();
            MoreVM.Init(this);

            MoreAdvancedVM = new MoreEventPageAdvancedViewModel();
            MoreAdvancedVM.Init(this);

            CurrentView = MoreVM;

            ChangeViewCommand = new RelayCommand(o =>
            {
                if (CurrentView == MoreVM)
                    CurrentView = MoreAdvancedVM;

                else CurrentView = MoreVM;
            });

            SubmitCommand = new RelayCommand(OnSubmit);
        }

        private void OnSubmit(object obj)
        {
        }
    }
}
