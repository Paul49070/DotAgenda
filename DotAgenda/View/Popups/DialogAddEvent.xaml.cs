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
using System.Drawing;
using DotAgenda.View;
using System.Windows.Forms;
using RadioButton = System.Windows.Controls.RadioButton;
using Application = System.Windows.Application;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Security.Cryptography;

namespace DotAgenda.View.Popups
{
    public partial class Popup : Window
    {
        public string classe="";
        public bool Activ = true;

        public Popup()
        {
            this.InitializeComponent();
            var _mainView = (MainWindow)Application.Current.MainWindow;

            _mainView.ShowPopup();

            if (_mainView.ThingToAdd == 0)
            {
                Evenement.IsChecked = true;
                Evenement.Command.Execute(Evenement.CommandParameter);

            }
            else
            {
                Tache.IsChecked = true;
                Tache.Command.Execute(Tache.CommandParameter);
            }

            _mainView.ThingToAdd = 0;

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Activ = false;
            base.OnClosing(e);
            if (this.Owner != null)
            {
                this.Owner.Topmost = true;
            }
        }
        
        protected override void OnClosed(EventArgs e)
        {
            Activ = false;

            base.OnClosed(e);
            
                if (this.Owner != null)
                {
                    this.Owner.Activate();
                    this.Owner.Topmost = false;
                }

            var _mainView = (MainWindow)Application.Current.MainWindow;
            _mainView.HidePopup();
            
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (Activ == true)
            {
                Close();
                Activ = false;
            }
        }   
    }
}
