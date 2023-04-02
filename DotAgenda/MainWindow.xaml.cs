using DotAgenda.Models;
using DotAgenda.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using DotAgenda.ViewModels;
using DotAgenda.View.Popups;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Security.Cryptography;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using DevExpress.Utils.About;
using Microsoft.SqlServer.Management.XEvent;
using Windows.ApplicationModel.UserDataTasks;
using System.ComponentModel.Design;
using System.Net.Mail;
using System.Net;
using DevExpress.DirectX.Common.Direct2D;
using System.Windows.Media.Animation;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using MaterialDesignColors;
using System.Threading;
using System.Drawing;
using System.IO;
using Typography.OpenFont.Tables;
using System.Windows.Media.Effects;
using System.ComponentModel;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;

namespace DotAgenda
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public int ThingToAdd = 0;      //0 = event/1 = todo/...


        //public CurrentDay _currentDay = new CurrentDay();

        public ObservableCollection<Jours> ListeEventWeek = new ObservableCollection<Jours>();

        public ObservableCollection<EventDay> FilteredEventSearch = new ObservableCollection<EventDay>();
        
        DataBase _db;
        GlobalDict _dict;
        GestionnaireEvent _global;
        Primitives _prim;

        public int delai_nextEvent = 3;

        public bool IsNavBarDevelop = false;
        public string SearchText;

        public MainWindow()
        {
            _global = GestionnaireEvent._global;


            _dict = _global._dict;
            _db = _global._db;
            _prim = _global._prim;

            InitializeComponent();

            SearchBox.LostFocus += SearchBox_LostFocus;
            SearchBox.GotFocus += SearchBox_GotFocus;
            SearchBarItems.ItemsSource= FilteredEventSearch;
        }


        public void AddEvent(object sender, RoutedEventArgs e)
        {
            Activate();            //on active la MainWindow pour prévenir le cas ou on la met en arrière plan et on revient dessus
            Popup frm1 = new Popup
            {
                Owner = this
            };

            frm1.Show();
            frm1.Activate();    //On l'active car sinon le Deactivate Event ne fonctionne pas en mode .exe
        }

        private void ChangeColor(object sender, RoutedEventArgs e) //Change couleurs nav bar icons
        {
            var button = sender as RadioButton;
            if (button.Name != "calendar") calendar.IsChecked = false;
            if (button.Name != "list") list.IsChecked = false;
            if (button.Name != "folder") folder.IsChecked = false;
        }

        private void Deconnect(object sender, RoutedEventArgs e)
        {
            ProfilPicture.ContextMenu.Visibility = Visibility.Hidden;

            _db.Deconnect();
            
            Application.Current.Shutdown();
            System.Windows.Forms.Application.Restart();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Reduce(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Minimized;
            WindowStyle = WindowStyle.None;
        }

        private void MoreNavBar(object sender, RoutedEventArgs e)
        {
            int ToWidth;
            if (IsNavBarDevelop)
            {
                ToWidth = 70;
            }
            else 
            {
                ToWidth = 170;
            }

            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = NavBar.Width,
                To = ToWidth,
                Duration = TimeSpan.FromSeconds(0.2)
            };

            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Grid.WidthProperty));
            Storyboard.SetTarget(widthAnimation, NavBar);

            Storyboard s = new Storyboard();
            s.Children.Add(widthAnimation);
            s.Begin();
            

            IsNavBarDevelop = !IsNavBarDevelop;
        }

        private void Parametre(object sender, RoutedEventArgs e)
        {
            var event_window = new Settings();
            event_window.Owner = Window.GetWindow(this);
            bool? dialogResult = event_window.ShowDialog();
        }

        private void DetailProfil(object sender, RoutedEventArgs e)
        {

        }

        //SearchBox

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            string text = box.Text;

            FilteredEventSearch.Clear();

            if (_global.A.Count() > 0 & text != "")
            {
                foreach (var itemA in _global.A)
                {
                    if (itemA != null)
                    {
                        foreach (var itemM in itemA.M)
                        {
                            if (itemM != null)
                            {
                                foreach (var itemJ in itemM.J)
                                {
                                    if (itemJ != null)
                                    {
                                        foreach (var item in itemJ.ListeEvent)
                                        {
                                            if (item != null)
                                            {
                                                if (item.Titre.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                                                    FilteredEventSearch.Add(item);

                                                else FilteredEventSearch.Remove(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                 }
             }
         }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            if (calendar.IsChecked == false)
            {
                calendar.IsChecked = true;
                folder.IsChecked = false;
                list.IsChecked = false;
            }


            var item = (sender as Button).DataContext as EventDay;
            HomeView.ActCalendrier(item.DateDebut);

            SearchBox.Text = "Recherchez...";
        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "Recherchez...";
        }

        //Focus + utilisataire


        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Recherchez...") SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchText = SearchBox.Text;
            if (SearchBox.Text == "")
            {
                FilteredEventSearch.Clear();
                SearchBox.Text = "Recherchez...";
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            base.OnMouseLeftButtonDown(e);
            _mainView.DragMove();
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }

        public void ShowPopup()
        {
            this.Opacity = .5;
            this.Effect = new BlurEffect();
        }

        public void HidePopup()
        {
            this.Opacity = 1;
            this.Effect = null;
        }

        private void DayNightClick(object sender, RoutedEventArgs e)
        {
            App.User.lightMode = !App.User.lightMode;
        }
    }
}
