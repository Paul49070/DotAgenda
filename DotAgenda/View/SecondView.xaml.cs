using System.Diagnostics;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using DotAgenda.Models;
using DotAgenda.View.Popups;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Collections.Specialized;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour SecondView.xaml
    /// </summary>
    public partial class SecondView : UserControl
    {

        GestionnaireEvent _global;
        DataBase _db;
        Primitives _prim;
        GlobalDict _dict;

        public ObservableCollection<Fichier> ListeFichiersRecents = new ObservableCollection<Fichier>();
        public ObservableCollection<Fichier> ListeFichiersDossiers = new ObservableCollection<Fichier>();

        public ObservableCollection<Dossier> ListeDossiers = new ObservableCollection<Dossier>();

        public Dossier useless = new Dossier { Titre = "Récents", Couleur = Application.Current.Resources["Contrast"].ToString() };
        public Dossier Current_Folder;

        int recent_delay = 7; //en nb de jours

        string ColorDrop;
        public SecondView()
        {
            InitializeComponent();

            _global = GestionnaireEvent._global;

            _dict = _global._dict;
            _db = _global._db;
            _prim = _global._prim;

            ListeFichiersRecents.Clear();
            ListeFichiersDossiers.Clear();

            ColorDrop = BorderDrop.BorderBrush.ToString();
            BorderDrop.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FFFFFF"));

            SetListeRecent();
            SetListeDossier();
            SetList(useless, false);

            ((INotifyCollectionChanged)ListeView_File.Items).CollectionChanged += ListView_File_CollectionChanged;

            ListeView_File.ItemsSource = ListeFichiersRecents;
            FileBoxs.ItemsSource = ListeDossiers;
        }

        private void ListView_File_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetListeDossier();
        }

        private void SetListeDossier()
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            ObservableCollection<Dossier> liste_temp = new ObservableCollection<Dossier>();

            for (int i = 0; i < _global.ListeDossiersType.Count; ++i)
            {
                liste_temp.Add(_global.ListeDossiersType[i]);
            }


            int max_temp;
            int index_temp;

            ListeDossiers.Clear();

            for (int i = 0; i < 6; ++i)
            {
                index_temp = 0;
                max_temp = -1;
                foreach (Dossier folder in liste_temp)
                {
                    if (max_temp < folder.ListeFichiers.Count)
                    {
                        index_temp = liste_temp.IndexOf(folder);
                        max_temp = folder.ListeFichiers.Count;
                    }
                }

                ListeDossiers.Add(liste_temp[index_temp]);
                liste_temp.RemoveAt(index_temp);
            }
        }

        private void SetListeRecent()
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            List<Fichier> list_temp = new List<Fichier>();

            foreach (Fichier fic in _global.ListeFichiers)
            {
                if (fic.DateAjout >= DateTime.Today.AddDays(-recent_delay))
                {
                    list_temp.Add(fic);
                }
            }

            IEnumerable<Fichier> temp = list_temp.OrderBy(fic => fic.DateAjout);

            temp = temp.Reverse();

            for (int i = 0; i < temp.Count(); ++i)
            {
                ListeFichiersRecents.Add(temp.ElementAt(i));
            }
        }

        private void GetFile(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            string CheminFichier;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CheminFichier = openFileDialog.FileName;

                Fichier fichier = _prim.CreateFile(CheminFichier);
                if (fichier.Type == null)
                {
                    var event_window = new AlreadyImportedFile();
                    event_window.Show();
                }

                else ListeFichiersRecents.Insert(0, fichier);
            }

            SetList(Current_Folder, Current_Folder.Type);
        }


        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;

            int index = ListeView_File.Items.IndexOf(item);

            Process launchingFile = new Process();

            try
            {
                launchingFile.StartInfo.FileName = ListeFichiersRecents[index].Nom;
                launchingFile.Start();
            }

            catch
            {
                ShowOpenWithDialog(ListeFichiersRecents[index].Nom);
            }
        }

        public static void ShowOpenWithDialog(string path)
        {
            var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + path;
            Process.Start("rundll32.exe", args);
        }

        private void DataGrid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }

        private void ChangeList(object sender, RoutedEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;
            var item = (sender as FrameworkElement).DataContext;

            int index = FileBoxs.Items.IndexOf(item);

            if (index == -1)
                SetList(useless, false);

            else SetList(ListeDossiers[index], true);
        }

        private void SetList(Dossier folder, bool bfolder)
        {
            Current_Folder = folder;

            if (bfolder)
            {
                ListeFichiersDossiers.Clear();

                foreach (Fichier fic in folder.ListeFichiers)
                {
                    ListeFichiersDossiers.Add(fic);
                }

                ListeView_File.ItemsSource = ListeFichiersDossiers;
            }

            else ListeView_File.ItemsSource = ListeFichiersRecents;

            TitleFileList.Text = folder.Titre;
            TitleFileList.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(folder.Couleur));
        }

        private void cm(object sender, RoutedEventArgs e)
        {

        }

        private void MoreEvent(object sender, RoutedEventArgs e)
        {

        }

        private void Supprimer(object sender, RoutedEventArgs e)
        {

        }

        private void DropFile(object sender, DragEventArgs e)
        {
            var _mainView = (MainWindow)Application.Current.MainWindow;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string fic in files)
                {
                    Fichier fichier = _prim.CreateFile(fic);
                    if (fichier.Type == null)
                    {
                        var event_window = new AlreadyImportedFile();
                        event_window.Show();
                    }

                    else ListeFichiersRecents.Insert(0,fichier);
                }

                SetList(Current_Folder, Current_Folder.Type);
            }
        }

        private void DragOn(object sender, DragEventArgs e)
        {
            BorderDrop.Style = (Style)Application.Current.FindResource("BorderDropped");
        }

        private void DragOff(object sender, DragEventArgs e)
        {
            BorderDrop.Style = (Style)Application.Current.FindResource("BorderDrop");
        }

        private void DetachFile(object sender, RoutedEventArgs e)
        {
            var dialog = new AreYouSureDelete("Vous allez supprimer ce fichier de l'application", "Supprimer");
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                Button btn = sender as Button;
                var item = btn.DataContext;

                int pos = ListeView_File.Items.IndexOf(item);

                Fichier fic;

                if (ListeView_File.ItemsSource == ListeFichiersRecents)
                {
                    fic = ListeFichiersRecents[pos];
                    ListeFichiersRecents.Remove(fic);
                    _prim.DeleteFile(fic);
                }

                else
                {
                    fic = ListeFichiersDossiers[pos];
                    ListeFichiersDossiers.Remove(fic);
                    _prim.DeleteFile(ListeFichiersDossiers[pos]);
                }
            }

            e.Handled = true;
        }
    }
}
