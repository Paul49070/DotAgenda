using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Forms;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;
using Window = System.Windows.Window;
using DotAgenda.Models;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using DotAgenda.View.Popups;
using Button = System.Windows.Controls.Button;
using System.Linq;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Windows.Markup;
using static DotAgenda.Models.EventDay;
using System.Text.RegularExpressions;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.ViewModels;

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour MoreEventPage.xaml
    /// </summary>
    public partial class MoreEventPage : Window
    {

        Dictionary<string, RepeatType> DictRepeat = new Dictionary<string, RepeatType>
            {
                { "jours", RepeatType.Daily },
                { "semaines", RepeatType.Weekly },
                { "mois", RepeatType.Monthly },
                { "ans", RepeatType.Yearly }
            };

        private MoreEventPageViewModel _ViewModel;

        public MoreEventPage(EventDay DetailOne)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            _mainView.ShowPopup();

            _ViewModel = new MoreEventPageViewModel(DetailOne);
            DataContext = _ViewModel;

            InitializeComponent();
        }

       
        /*
        private bool CheckFrequencyRegex()
        {
            if (ReccurenceCheck.IsChecked == true)
            {
                if (Regex.IsMatch(Frequency.Text.ToString(), "^[0-9]+$"))
                {
                    try
                    {
                        return int.Parse(Frequency.Text.ToString()) > 0;
                    }
                    catch { }
                }

                return false;
            }

            return true;
        }

        public RepeatType GetRepeatType()
        {
            try
            {
                string selectedValue = ReccurenceSelection.SelectedItem.ToString();
                return DictRepeat[selectedValue];
            }

            catch { }

            return RepeatType.None;
        }

        */

        private void Submit(object sender, RoutedEventArgs e)
        {
            /*
            if (CheckFrequencyRegex())
            {

                Reccurences newReccurences = new Reccurences();

                if (ReccurenceCheck.IsChecked == true)
                {
                    newReccurences.Repeat = GetRepeatType();
                    newReccurences.EveryXtime = int.Parse(Frequency.Text.ToString());
                }

                else newReccurences.Repeat = RepeatType.None;

               
                var _mainView = (MainWindow)Application.Current.MainWindow;

                _newEvent.Titre = TitreEvent.Text;
                _newEvent.Fichiers = _currentEvent.Fichiers;

                DateTime SelectDate = (DateTime)DatePickerEventStart.SelectedDate;

                int HeureDebut = int.Parse(Heure.SelectedItem.ToString().Substring(0, 2));
                int MinuteDebut = int.Parse(Heure.SelectedItem.ToString().Substring(3, 2));

                int HFin = int.Parse(HeureFin.SelectedItem.ToString().Substring(0, 2));
                int MinuteFin = int.Parse(HeureFin.SelectedItem.ToString().Substring(3, 2));


                _newEvent.DateDebut = new DateTime(SelectDate.Year, SelectDate.Month, SelectDate.Day, HeureDebut, MinuteDebut, 0);
                _newEvent.DateFin = new DateTime(SelectDate.Year, SelectDate.Month, SelectDate.Day, HFin, MinuteFin, 0);

                _newEvent.Description = DescriptionTXT.Text;
                _newEvent.Reccurence = newReccurences;


                if (_currentEvent != _newEvent)
                {

                    _prim.ModifEvent(_newEvent, _currentEvent);

                    _mainView.HidePopup();
                    DialogResult = true;
                }
            }*/
        }


        private void CancelPage(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            _mainView.HidePopup();

            DialogResult = false;
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
