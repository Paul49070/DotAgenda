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
using System.Windows.Shapes;


namespace DotAgenda
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Popup_modif : Window
    {
        private ObservableCollection<EventDay> eventday;
        public Popup_modif()
        {
            InitializeComponent();
            eventday = new ObservableCollection<EventDay>();
        }

        public string Titre { get; set; }
        //public string Vheure { get; set; }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Heure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
