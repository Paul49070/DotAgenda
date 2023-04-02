using DotAgenda.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using RadioButton = System.Windows.Controls.RadioButton;
using UserControl = System.Windows.Controls.UserControl;
using Application = System.Windows.Application;
using System.Globalization;
using System.Threading;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;

namespace DotAgenda.View.Popups
{
    public partial class DialogEvent_Evenement : UserControl
    {
        public string classe = "";
        private ObservableCollection<EventDay> eventday;

        GestionnaireEvent _global;
        DataBase _db;
        GlobalDict _dict;
        Primitives _prim;


        public DialogEvent_Evenement()
        {
            InitializeComponent();

            _global = GestionnaireEvent._global;

            _dict = _global._dict;
            _db = _global._db;
            _prim = _global._prim;

            TitreEvent.LostFocus += TitreEvent_LostFocus;
            TitreEvent.GotFocus += TitreEvent_GotFocus;

            int index = (DateTime.Now.Hour+1) * 2;

            if (DateTime.Now.Minute >= 30) ++index;

            Heure.SelectedIndex = index;
            HeureFin.SelectedIndex = index + 2;

            SetDay();
            CreateRadio();
            eventday = new ObservableCollection<EventDay>();
        }

        private void CreateRadio()
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            for (int j = 0; j < 2; ++j)
            {
                for (int i = 0; i < _dict.DictClasse.Count(); i++)
                {
                    RadioButton r1 = new RadioButton();
                    if (i == 0) r1.IsChecked = true;
                    ClasseSel.Children.Add(r1);
                    r1.Name = _dict.DictClasse.Values.ElementAt(i).Titre;
                    r1.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_dict.DictClasse.Values.ElementAt(i).Couleur);
                    r1.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#60" + _dict.DictClasse.Values.ElementAt(i).Couleur.Substring(1, 6));
                    r1.Click += ChangeClass;
                    r1.Style = (Style)Application.Current.FindResource("ColorPicker");
                }
            }
        }

        public void SetDay()
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.LongDatePattern = "dd MMMM yyyy";
            ci.DateTimeFormat.ShortDatePattern = "dd.MMM.yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            DatePickerEvent.SelectedDate = _global._currentDay.Date;
        }

        private void TitreEvent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TitreEvent.Text == "Titre")
            {
                TitreEvent.Text = "";
            }
        }

        private void TitreEvent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TitreEvent.Text == "")
            {
                TitreEvent.Text = "Titre";
            }
        }

        public string Titre { get; set; }
        //public string Vheure { get; set; }

        private void Submit(object sender, RoutedEventArgs e)
        {
            bool valide = true;

            var myObject = (MainWindow)Application.Current.MainWindow;

            //var myObject = this.Owner as MainWindow;
            if (classe == "") classe = _dict.DictClasse.Values.ElementAt(0).Titre;

            Titre = TitreEvent.Text;

            DateTime debut, fin, temp = DatePickerEvent.SelectedDate.Value;

            debut = new DateTime(temp.Year, temp.Month, temp.Day, int.Parse(Heure.Text.Substring(0, 2)), int.Parse(Heure.Text.Substring(3, 2)), 0);
            fin = new DateTime(temp.Year, temp.Month, temp.Day, int.Parse(HeureFin.Text.Substring(0, 2)), int.Parse(HeureFin.Text.Substring(3, 2)), 0);

            string VheureF = HeureFin.Text.Substring(0, 2);

            if (Int32.Parse(VheureF) == 0)
                VheureF = "24";

            int duree = fin.Hour - debut.Hour;

            if (duree <= 0)
            {
                valide = false;
                HeureFin.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#f25b65");
            }

            else HeureFin.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(Application.Current.Resources["Font"].ToString());


            if (Titre == "Titre" | Titre == "")
            {
                valide = false;
                TitreEvent.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#f25b65");
                TitreEvent.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#f25b65");
            }

            else
            {
                TitreEvent.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(Application.Current.Resources["Font"].ToString());
                TitreEvent.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(Application.Current.Resources["Tertiary"].ToString());
            }


            if (valide)
            {
                string couleur = _dict.DictClasse[classe].Couleur;

                EventDay EventAdd = new EventDay(_prim.GenerateID(), Titre, debut, fin, "", "", classe);
                
                _global.A[debut.Year - DateTime.Today.Year +1].M[debut.Month - 1].J[debut.Day-1].AjouterEvent(EventAdd);


                Window parentWindow = Window.GetWindow(this);
                parentWindow.Close();
            }

        }

        private void ChangeClass(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            classe = button.Name;
        }

        bool flipflop = true;

        private void SeeMore(object sender, RoutedEventArgs e)
        {
            if (flipflop)
            {
                ClasseSel.MaxHeight = 400;
                ClasseSel.MaxWidth = 180;
            }
            else
            {
                ClasseSel.MaxHeight = 85;
                ClasseSel.MaxWidth = 200;
            }

            flipflop = !flipflop;
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta / 3);
        }
    }
}
