using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.Generic;
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

namespace DotAgenda.View.Popups
{
    /// <summary>
    /// Logique d'interaction pour AreYouSureDelete.xaml
    /// </summary>
    public partial class AreYouSureDelete : Window
    {

        public AreYouSureDelete(string ActionDescription, string Action)
        {
            InitializeComponent();

            CoreTXT.Text = ActionDescription;
            OkButton.Content = Action;
        }

        private void OkBtn(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
