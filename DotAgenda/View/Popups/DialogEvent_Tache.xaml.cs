using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.View.Popups
{
    /// <summary>
    /// Logique d'interaction pour DialogEvent_Tache.xaml
    /// </summary>
    public partial class DialogEvent_Tache : UserControl
    {
        GestionnaireEvent _global;
        DataBase _db;
        Primitives _prim;
        GlobalDict _dict;

        public DialogEvent_Tache()
        {
            InitializeComponent();

            _global = GestionnaireEvent._global;
            _dict = GlobalDict._dict;
            _db = DataBase._db;
            _prim = Primitives._prim;

            TitreEvent.LostFocus += TitreEvent_LostFocus;
            TitreEvent.GotFocus += TitreEvent_GotFocus;
            SetDay();
        }

        public void SetDay()
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.LongDatePattern = "dd MMMM yyyy";
            ci.DateTimeFormat.ShortDatePattern = "dd.MMM.yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            DatePickerTodo.SelectedDate = _global._currentDay.Date;
        }

        private void SeeMore(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta / 3);

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

        private void Submit(object sender, RoutedEventArgs e)
        {
            bool valide = true;

            var myObject = (MainWindow)Application.Current.MainWindow;

            //var myObject = this.Owner as MainWindow;

            DateTime Date = _global._currentDay.Date;

            string Titre = TitreEvent.Text;

            //Ajouter un DateTIme Deadline

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

                TodoItem TodoAdd = new TodoItem(Titre, DateTime.Parse(DatePickerTodo.SelectedDate.Value.ToString()), "", false);
                TodoAdd.AjouterTodoToDB();

                Window parentWindow = Window.GetWindow(this);
                parentWindow.Close();
            }
        }
    }
}
