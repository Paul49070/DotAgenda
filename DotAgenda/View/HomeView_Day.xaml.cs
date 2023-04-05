using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
using System.Data.SQLite;
using DotAgenda.ViewModels;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Security.Policy;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Security.Cryptography;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Reflection;
using System.Windows.Media.Animation;
using System.Drawing;
using DevExpress.Utils;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Typography.OpenFont.Tables;
using System.Globalization;
using System.Windows.Media.Effects;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using Windows.UI.Xaml.Controls.Primitives;
using System.ComponentModel;
using Windows.UI.Xaml.Input;

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour HomeView_Day.xaml
    /// </summary>
    public partial class HomeView_Day : UserControl
    {

        int startIndex = -1;

        readonly DataBase _db;
        GlobalDict _dict;
        GestionnaireEvent _global;
        Primitives _prim;

        ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>> ListeEventOverlap;

        public HomeView_Day()
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            this.InitializeComponent();

            _global = GestionnaireEvent._global;

            _dict = GlobalDict._dict;
            _db = DataBase._db;
            _prim = Primitives._prim;

            this.DataContext = _global._currentDay;
            NextEvent.ItemsSource = _global.NextEvent;

            ImgTodo.Source = new BitmapImage(new Uri("/Img/People/Rain.png", UriKind.Relative));

            ListeTodo.Loaded += ListeTodo_Load;
            ((INotifyCollectionChanged)ListeEvent.Items).CollectionChanged += ListView_CollectionChanged;
            ((INotifyCollectionChanged)ListeTodo.Items).CollectionChanged += ListViewTodo_CollectionChanged;


            List<string> Liste_hour = new List<string>();

            for(int i = 0; i<24; ++i)
            {
                Liste_hour.Add(i.ToString("00") + "h"); 
            }

            ListeHeure.ItemsSource = Liste_hour;

            scrollviewer.Loaded += Scrollviewer_Loaded;        


        //_global._currentDay.ListeEvent.CollectionChanged += ;
            _global._currentDay.ListChanged += CurrentDayCollectionChanged;

            UpdateListe();

            VerifTodoNull();
            InitDetailsEvent(_prim.SearchNextEvent(_mainView.delai_nextEvent, DateTime.Today));
        }

        private void Scrollviewer_Loaded(object sender, RoutedEventArgs e)
        {
            SetScrollBarPosition(scrollviewer.ExtentHeight);
        }

        private void SetScrollBarPosition(double ScrollViewerSize)
        {
            TimeSpan _currentTime = DateTime.Now.TimeOfDay;
            double offset = (_currentTime.TotalSeconds / (24 * 75 * 60)) * ScrollViewerSize;

            // Scroll to the calculated offset
            scrollviewer.ScrollToVerticalOffset(offset);
        }

        private void CurrentDayCollectionChanged(object sender, ListChangedEventArgs e)
        {
            UpdateListe();
        }

        private void UpdateListe()
        {
            ObservableCollection<EventDay> OverlapList = new ObservableCollection<EventDay>();

            foreach (EventDay evenement in _global._currentDay.ListeEvent)
            {
                OverlapList.Add(evenement);
            }

            ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>> ListeEventOverlap_temp = SortList(OverlapList);
            ListeEventOverlap = ListeEventOverlap_temp;

            ListeEvent.ItemsSource = ListeEventOverlap;
            ListeEvent.DataContext = ListeEventOverlap;
        }

        public ObservableCollection<ObservableCollection<EventDay>> GetOverlappingEvents(List<EventDay> events)
        {
            ObservableCollection<ObservableCollection<EventDay>> result = new ObservableCollection<ObservableCollection<EventDay>>();
            ObservableCollection<EventDay> overlap = new ObservableCollection<EventDay>();
            overlap.Add(events[0]);

            for (int i = 1; i < events.Count; i++)
            {
                bool isOverlap = false;
                for (int j = 0; j < overlap.Count; j++)
                {
                    //On check pour tout les items de la ligne actuelle si il y en a un qui overlap avec un autre. Si oui on sors direct

                    if (events[i].DateDebut < overlap[j].DateFin && events[i].DateFin > overlap[j].DateDebut)
                    {
                        isOverlap = true;
                        break;
                    }
                }

                //Si on a bien trouvé un overlap (n'importe lequel) on l'ajoute a la ligne

                if (isOverlap)
                {
                    overlap.Add(events[i]);
                }
                
                //Sinon on ajoute notre ligne a la liste des lignes et on créer une autre ligne

                else
                {
                    result.Add(overlap);
                    overlap = new ObservableCollection<EventDay>
                    {
                        events[i]
                    };
                }
            }

            //Si la dernière ligne a plus d'event que 0

            if (overlap.Count > 0)
            {
                result.Add(overlap);
            }

            return result;
        }

        public ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>> SortList(ObservableCollection<EventDay> list)
        {

            ObservableCollection<ObservableCollection<EventDay>> listOverlap = new ObservableCollection<ObservableCollection<EventDay>>();

            List<EventDay> list_temp = list.ToList();

            if (list_temp.Count() > 0)
            {
                listOverlap = GetOverlappingEvents(list_temp);

                //la on a toutes les listes d'event qui s'overlap. Maintenant le but ça va être de créer des listes de listes de listes 

                ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>> listtemp = new ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>>();

                bool first;
                int taille = -1;
                int taillel2 = -1;
                int taille3 = -1;

                foreach (ObservableCollection<EventDay> l in listOverlap) // = liste de toutes mes listes  d'events
                {
                    listtemp.Add(new ObservableCollection<ObservableCollection<EventDay>>()); //j'ajoute une nouvelle liste qui la represente
                    ++taille;

                    first = true;

                    while (l.Count != 0)
                    {
                        taillel2 = listtemp[taille].Count - 1;

                        if (first)
                        {
                            first = false;

                            listtemp[taille].Add(new ObservableCollection<EventDay>());
                            listtemp[taille][0].Add(l[0]);  //Le premier event est forcément dans sa liste
                        }

                        else
                        {
                            //Cherche slot dispo en partant de la gauche jusqu'à lui.

                            if (taillel2 >= 0)
                            {
                                taille3 = listtemp[taille][taillel2].Count - 1;

                                int SlotDispo(EventDay e, ObservableCollection<ObservableCollection<EventDay>> listHorizontal)
                                {
                                    for (int i = 0; i < listHorizontal.Count; ++i)
                                    {
                                        DateTime dateMaxListe = listHorizontal[i].Last().DateFin;

                                        if (e.DateDebut >= dateMaxListe)
                                        {
                                            listHorizontal[i].Add(e);
                                            return 0;
                                        }
                                    }
                                    return -1;
                                }

                                if (SlotDispo(l[0], listtemp[taille]) == -1)
                                {
                                    listtemp[taille].Add(new ObservableCollection<EventDay>()); //nouvelle colonne
                                    listtemp[taille][listtemp[taille].Count - 1].Add(l[0]);
                                }
                            }
                        }

                        l.RemoveAt(0);
                    }
                }
                return listtemp;
            }

            return new ObservableCollection<ObservableCollection<ObservableCollection<EventDay>>>();
        }

        public ObservableCollection<EventDay> OverlappingEvent(ObservableCollection<EventDay> list)
        {
            //La liste "list" est déjà triée.

            ObservableCollection<EventDay> OverlappingEvent = new ObservableCollection<EventDay>();

            int length = list.Count;

            while(list.Count!=0)
            {
                if (list.Count != 1)
                {
                    //if()

                    if (list[0].DateDebut < list[1].DateFin && list[1].DateDebut < list[0].DateFin)
                    {
                        if (!OverlappingEvent.Contains(list[0]))
                            OverlappingEvent.Add(list[0]);

                        if (!OverlappingEvent.Contains(list[1]))
                            OverlappingEvent.Add(list[1]);
                    }

                    else
                    {
                        if (!OverlappingEvent.Contains(list[0]))
                        {
                            OverlappingEvent.Add(list[0]);
                        }

                        list.RemoveAt(0);
                        return OverlappingEvent;
                    }

                    list.RemoveAt(0);
                }

                else
                {
                    int last = OverlappingEvent.Count - 1;
                    if (last > 0)
                    {
                        if (list[0].DateDebut < OverlappingEvent[last].DateFin && OverlappingEvent[last].DateDebut < list[0].DateFin)
                        {
                            if (!OverlappingEvent.Contains(list[0]))
                                OverlappingEvent.Add(list[0]);
                        }
                    }

                    else
                    {
                        if (!OverlappingEvent.Contains(list[0]))
                            OverlappingEvent.Add(list[0]);
                    }

                    list.RemoveAt(0);
                }
            }


            return OverlappingEvent;
        }

        //DRAG & DROP Todo
        private void ListeTodo_Load(object sender, EventArgs e)
        {
            ListeTodo.AllowDrop = true;
            ListeTodo.DragEnter += new DragEventHandler(ListeTodo_DragEnter);
        }

        void ListeTodo_DragEnter(object sender, DragEventArgs e)
        {
            if (sender == e.Source)
            {
                // Get the drop ListViewItem destination
                ListView listView = sender as ListView;     //Ref de la list qui envoie l'event
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);        //Ref de l'item qui envoie l'event

                if (listViewItem == null)       //On vérifie si il éxiste
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }

                listViewItem.BorderThickness = new Thickness(0, 2, 0, 0);
                //listViewItem.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Application.Current.Resources["Font_Grey"].ToString()));
            }

            e.Effects = DragDropEffects.Copy;
        }

        private static T FindAnchestor<T>(DependencyObject current)
        where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void Todo_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                sender is FrameworkElement)         //On vérifie que le sender est bien un control
            {
                ListView listView = sender as ListView;

                //Précautions

                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);        //On cherche si la source du click est bien la liste
                if (listViewItem == null) return;              //Si elle ne l'est pas, alors il est inutile de continuer car on ne fera aucune opération.
                                                            
                TodoItem item = (TodoItem)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);      //On récupère ici le TodoItem selectionné
                if (item == null) return;                                       //Si il n'éxiste pas on arrête
                
                //On initialise notre index

                startIndex = listView.SelectedIndex;
                DataObject dragData = new DataObject("TodoItem", item);    //On créer une variable qui stocke les datas du todoItem pour le transférer

                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);       //On lance le drag & drop (soit on déplace soit on copy
            }
        }

        void ListeTodo_DragDrop(object sender, DragEventArgs e)
        {
            int index;

            if (sender == e.Source)
            {
                // Get the drop ListViewItem destination
                ListView listView = sender as ListView;     //Ref de la list qui envoie l'event
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);        //Ref de l'item qui envoie l'event

                if (listViewItem == null)       //On vérifie si il éxiste
                {
                    // Abort
                    e.Effects = DragDropEffects.None;
                    return;
                }

                //On obtient l'item concerné
                TodoItem item = (TodoItem)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                //L'action choisie est "Move"

                e.Effects = DragDropEffects.Move;

                //On prends l'index de l'item cible concerné (pour le bouger)

                index = _global._currentDay.Todo.IndexOf(item);

                if (startIndex >= 0 && index >= 0)
                {
                    _global._currentDay.Todo.Move(startIndex, index);
                    int numY = _global._currentDay.Date.Year - DateTime.Today.Year + 1;
                    _global.A[numY].M[_global._currentDay.Date.Month-1].J[_global._currentDay.Date.Day - 1].Todo.Move(startIndex, index);
                }
                startIndex = -1;        // On réinitialise notre index 

                listViewItem.BorderThickness = new Thickness(0, 0, 0, 0);

            }
        }

        //On collection changed

        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            VerifTodoNull();
            EventDay temp = _prim.SearchNextEvent(_mainView.delai_nextEvent, DateTime.Today);
            InitDetailsEvent(temp);
        }

        private void ListViewTodo_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            VerifTodoNull();
        }

        //Primitive Todo

        public void VerifTodoNull()
        {
            Random rnd = new Random();
            int min = 0;
            int max = 2;
            int ramdom_index = rnd.Next(min, max);

            if (_global._currentDay.Todo.Count == 0)
            {
                AjouterTitre.Visibility = Visibility.Hidden;
                UneTache.Visibility = Visibility.Hidden;
                AucuneTache.Visibility = Visibility.Visible;
                ImgTodo.Visibility = Visibility.Visible;
                try
                {
                    ImgTodo.Source = new BitmapImage(new Uri("/Img/People/Wait.png", UriKind.Relative));
                }
                catch
                {
                    Console.WriteLine("Image introuvable");
                }
            }

            else
            {
                AjouterTitre.Visibility = Visibility.Visible;

                ImgTodo.Visibility = Visibility.Hidden;
                AucuneTache.Visibility = Visibility.Hidden;
                UneTache.Visibility = Visibility.Visible;
            }
        }

        

        private void AddToDo(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            _mainView.ThingToAdd = 1;
            _mainView.AddEvent(sender, e);
        }

        private void AddEvent(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            _mainView.ThingToAdd = 0;
            _mainView.AddEvent(sender, e);
        }

        //Event Todo
        private void CheckTodo(object sender, RoutedEventArgs e)
        {
            var item = (sender as CheckBox).DataContext;
            int index = ListeTodo.Items.IndexOf(item);

            int numY = _global._currentDay.Date.Year - DateTime.Today.Year + 1;

            TodoItem Tache = _global.A[numY].M[_global._currentDay.Date.Month - 1].J[_global._currentDay.Date.Day - 1].Todo[index];

            DataBaseTodo._dbTodo.ChangeEtatTacheDB(Tache);
            _prim.ChangeEtatTache(Tache);
        }

        //Primitive DetailsEvent

        private void InitDetailsEvent(EventDay temp)
        {
            _global.NextEvent.Clear();

            if (temp == null)
            {
                NextEvent.Visibility = Visibility.Hidden;
                NoNextEvent.Visibility = Visibility.Visible;
                try
                {
                    ImgNoEvent.Source = new BitmapImage(new Uri("/Img/People/NoEvent.png", UriKind.Relative));
                }

                catch { }
            }

            else
            {
                NextEvent.Visibility = Visibility.Visible;
                NoNextEvent.Visibility = Visibility.Hidden;

                _global.NextEvent.Add(temp);
            }
        }

        //Primitive Evenement

        private void Supprimer(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            EventDay item = null;

            MenuItem mnu = sender as MenuItem;
            Button btn;
            if (mnu != null)
            {
                btn = ((ContextMenu)mnu.Parent).PlacementTarget as Button;
                item = btn.DataContext as EventDay;
            }


            int pos = _global._currentDay.ListeEvent.IndexOf(item);

            _global._currentDay.ListeEvent[pos].RemoveEvent();

            InitDetailsEvent(_prim.SearchNextEvent(_mainView.delai_nextEvent, DateTime.Today));

            e.Handled = true;
        }

        //Event Evenement

       

        private void ContextMenuEvent(object sender, RoutedEventArgs e)
        {
            var cm = ContextMenuService.GetContextMenu(sender as DependencyObject);
            if (cm == null)
            {
                return;
            }
            cm.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            cm.PlacementTarget = sender as UIElement;
            cm.IsOpen = true;

            e.Handled = true;
        }

        private void DataGrid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta/3);
            e.Handled = true;

        }


        private void ListeTodo_DragLeave(object sender, DragEventArgs e)
        {
            if (sender == e.Source)
            {
                // Get the drop ListViewItem destination
                ListView listView = sender as ListView;     //Ref de la list qui envoie l'event
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);        //Ref de l'item qui envoie l'event

                if (listViewItem == null)       //On vérifie si il éxiste
                {
                    // Abort
                    e.Effects = DragDropEffects.None;
                    return;
                }

                listViewItem.BorderThickness = new Thickness(0, 0, 0, 0);
            }
        }

        private void SuppTache(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var item = btn.DataContext;

            int pos = ListeTodo.Items.IndexOf(item);

            TodoItem Tache = _global._currentDay.Todo[pos];
            int numY = Tache.DateDebut.Year - DateTime.Today.Year + 1;

            _global.A[numY].M[Tache.DateDebut.Month - 1].J[Tache.DateDebut.Day - 1].DeleteTodo(Tache);
        }

        private void TodayUpdate(object sender, RoutedEventArgs e)
        {
            HomeView.ActCalendrier(DateTime.Today);
        }

        private void MoinsJours(object sender, RoutedEventArgs e)
        {
            HomeView.ActCalendrier(_global._currentDay.Date.AddDays(-1));
        }

        private void PlusJours(object sender, RoutedEventArgs e)
        {
            HomeView.ActCalendrier(_global._currentDay.Date.AddDays(1));
        }
        private void DropFile(object sender, DragEventArgs e)
        {
            Button btn = sender as Button;
            var item = btn.DataContext;

            int pos = ListeEvent.Items.IndexOf(item);

            DateTime date_actuelle = _global._currentDay.Date;
            int year = date_actuelle.Year - DateTime.Today.Year + 1;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string fic in files)
                {
                    _prim.AddFileToEvent(fic, _global.A[year].M[date_actuelle.Month - 1].J[date_actuelle.Day - 1].ListeEvent[pos]);
                }

            }

            btn.Style = (Style)Application.Current.FindResource("BasiqueBtnList");
        }

        private void ClickOnEvent(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            EventDay item = btn.DataContext as EventDay;

            int pos = _global._currentDay.ListeEvent.IndexOf(item);


            var event_window = new MoreEventPage(_global._currentDay.ListeEvent[pos]);
            event_window.Owner = Window.GetWindow(this);
            event_window.ShowDialog();
        }


        private void DragOnEvent(object sender, DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Button btn = sender as Button;
                btn.Style = (Style)Application.Current.FindResource("BasiqueBtnListDrop");
            }
        }

        private void DragOffEvent(object sender, DragEventArgs e)
        {
            base.OnDragLeave(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Button btn = sender as Button;
                btn.Style = (Style)Application.Current.FindResource("BasiqueBtnList");
            }
        }
    }
}
