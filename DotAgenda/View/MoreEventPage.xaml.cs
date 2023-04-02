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

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour MoreEventPage.xaml
    /// </summary>
    public partial class MoreEventPage : Window
    {
        EventDay _currentEvent, _newEvent;

        DataBase _db;
        GlobalDict _dict;
        GestionnaireEvent _global;
        Primitives _prim;


        public MoreEventPage(EventDay DetailOne)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            _mainView.ShowPopup();

            _global = GestionnaireEvent._global;

            _dict = _global._dict;
            _db = _global._db;
            _prim = _global._prim;


            InitializeComponent();

            _currentEvent = DetailOne;


            _newEvent = new EventDay(DetailOne.ID, DetailOne.Titre, DetailOne.DateDebut, DetailOne.DateFin, DetailOne.Lieu, DetailOne.Description, DetailOne.Classe);

            ListeView_File.ItemsSource = _currentEvent.Fichiers;

            SetHasNoFile();

            DatePickerEvent.SelectedDate = _currentEvent.DateDebut;
            TitreEvent.Text = _currentEvent.Titre;
            DescriptionTXT.Text = _currentEvent.Description;

            AdvancedOptionCheck.IsChecked = false;

            SH_all(false);
            SetSideBorder();
            SetReccurence();
            SetAlertes();

            SetHour();
            SetComboBox(_currentEvent.Classe);
        }

        public void SetReccurence()
        {
            List<string> frequencyToString = new List<string>
            {
                "jours",
                "semaines",
                "mois",
                "ans"
            };

            int x  = (int)Enum.Parse(typeof(EventDay.Frequency), Enum.GetName(typeof(EventDay.Frequency), _currentEvent.Reccurence.Frequency));

            ReccurenceSelection.ItemsSource = frequencyToString;

            if (x != -1)
            {
                ReccurenceCheck.IsChecked = true;
                ReccurenceSelection.SelectedIndex = x;
            }

            else
            {
                ReccurenceCheck.IsChecked = false;
                ReccurenceSelection.SelectedIndex = 0;
            }

            SH_Reccurence(x != -1);
        }

        public void SetAlertes()
        {
            AlertCheck.IsChecked = false;
            SH_Alertes(false);
        }

        public void SetSideBorder()
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            try
            {
                SideBorder.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_dict.DictClasse[_newEvent.Classe].Couleur);
            }
            catch
            {
                Console.WriteLine("ColorException");
            }
        }

        public void SetHasNoFile()
        {
            if (_currentEvent.Fichiers.Count == 0)
            {
                NoFile.Visibility = Visibility.Visible;
                HasFile.Visibility = Visibility.Hidden;
            }
            else
            {
                NoFile.Visibility = Visibility.Hidden;
                HasFile.Visibility = Visibility.Visible;
            }
        }

        private void SetHour()
        {
            List<string> HeureList = new List<string>();

            for (int i = 0; i < 24; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    HeureList.Add(String.Format("{0:00}", i) + ":" + String.Format("{0:00}", j * 15));
                }
            }

            Heure.ItemsSource = HeureList;
            Heure.SelectedItem = String.Format("{0:00}", _currentEvent.DateDebut.Hour) + ":" + String.Format("{0:00}", _currentEvent.DateDebut.Minute);

            HeureFin.ItemsSource = HeureList;
            HeureFin.SelectedItem = String.Format("{0:00}", _currentEvent.DateFin.Hour) + ":" + String.Format("{0:00}", _currentEvent.DateFin.Minute);
        }
        private void Titre_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        void ChangeClass(object sender, RoutedEventArgs e)
        {
            
            
        }

        private bool CheckFrequency()
        {
            if(Regex.IsMatch(Frequency.Text.ToString(), "^[0-9]+$"))
            {
                try
                {
                    return int.Parse(Frequency.Text.ToString())>0;
                }
                catch { }
            }

            return false;
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            if (CheckFrequency())
            {
                if (_currentEvent != _newEvent)
                {

                    var _mainView = (MainWindow)Application.Current.MainWindow;

                    _newEvent.Titre = TitreEvent.Text;
                    _newEvent.Fichiers = _currentEvent.Fichiers;

                    DateTime SelectDate = (DateTime)DatePickerEvent.SelectedDate;

                    int HeureDebut = int.Parse(Heure.SelectedItem.ToString().Substring(0, 2));
                    int MinuteDebut = int.Parse(Heure.SelectedItem.ToString().Substring(3, 2));

                    int HFin = int.Parse(HeureFin.SelectedItem.ToString().Substring(0, 2));
                    int MinuteFin = int.Parse(HeureFin.SelectedItem.ToString().Substring(3, 2));


                    _newEvent.DateDebut = new DateTime(SelectDate.Year, SelectDate.Month, SelectDate.Day, HeureDebut, MinuteDebut, 0);
                    _newEvent.DateFin = new DateTime(SelectDate.Year, SelectDate.Month, SelectDate.Day, HFin, MinuteFin, 0);

                    _newEvent.Description = DescriptionTXT.Text;

                    _prim.ModifEvent(_newEvent, _currentEvent, _global.A);

                    _mainView.HidePopup();
                    DialogResult = true;
                }
            }
        }

        //Entrer TextBox

        private void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
            }
        }

        private void CancelPage(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            _mainView.HidePopup();

            DialogResult = false;
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta / 3);
        }

        private void SetComboBox(string classe)
        {

            List<ClasseEvent_item> Classe =  _dict.DictClasse.Values.ToList();

            ClasseSelection.ItemsSource = Classe;

            try
            {
                ClasseSelection.SelectedItem = _dict.DictClasse[classe];
            }

            catch { }
        }

        private void DropFile(object sender, System.Windows.DragEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            BorderEvent.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00FFFFFF");

            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                foreach (string fic in files)
                {
                    _prim.AddFileToEvent(fic, _currentEvent);
                }
            }

            SetHasNoFile();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;

            int index = ListeView_File.Items.IndexOf(item);

            Process launchingFile = new Process();

            try
            {
                launchingFile.StartInfo.FileName = _currentEvent.Fichiers[index].Nom;
                launchingFile.Start();
            }

            catch
            {
                ShowOpenWithDialog(_currentEvent.Fichiers[index].Nom);
            }
        }

        private void GetFile(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            string CheminFichier;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CheminFichier = openFileDialog.FileName;
                _prim.AddFileToEvent(CheminFichier, _currentEvent);
            }

            SetHasNoFile();
        }

        public static void ShowOpenWithDialog(string path)
        {
            var args = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + path;
            Process.Start("rundll32.exe", args);
        }

        private void DetachFile(object sender, RoutedEventArgs e)
        {
            var dialog = new AreYouSureDelete("Vous allez détâcher ce fichier et cet évènement", "Détacher");
            bool? result = dialog.ShowDialog();

            if(result == true)
            {
                Button btn = sender as Button;
                var item = btn.DataContext;

                int pos = ListeView_File.Items.IndexOf(item);

                var _mainView = (MainWindow)Application.Current.MainWindow;
                _prim.DetachFileToEvent(_currentEvent.Fichiers[pos], _currentEvent);

                SetHasNoFile();
            }


            e.Handled = true;
        }

        private void ShowHideAlertes(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            bool etat = bool.Parse(checkbox.IsChecked.ToString());

            SH_all(etat);
        }

        public void SH_all(bool etat)
        {
            if (etat)
            {
                ReccurencesEtAlertes.Visibility = Visibility.Visible;
                ReccurencesEtAlertes.Height = Double.NaN;
            }

            else
            {
                ReccurencesEtAlertes.Visibility = Visibility.Hidden;
                ReccurencesEtAlertes.Height = 0;
            }
        }

        public void SH_Reccurence(bool etat)
        {
            if (etat)
            {
                ReccurenceGrid.Height = Double.NaN;
            }

            else ReccurenceGrid.Height = 0;
        }

        public void SH_Alertes(bool etat)
        {
            if (etat)
            {
                AlertGrid.Height = Double.NaN;
            }

            else AlertGrid.Height = 0;
        }


        private void AddRemoveReccurence(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;

            bool etat = bool.Parse(checkbox.IsChecked.ToString());
            SH_Reccurence(etat);
        }

        private void AddRemoveAlert(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            bool etat = bool.Parse(checkbox.IsChecked.ToString());
            SH_Alertes(etat);
        }

        private void ClasseSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            try
            {
                _newEvent.Classe = _dict.DictClasse.Keys.ElementAt(ClasseSelection.SelectedIndex);
            }

            catch 
            { 
            }

            SetSideBorder();
        }
    }
}
