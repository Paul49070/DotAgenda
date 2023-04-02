using DotAgenda.MethodClass;
using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IdentityModel.Claims;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Windows.UI.Xaml.Controls;

namespace DotAgenda.View.Popups
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        bool light;
        public Settings()
        {
            InitializeComponent();

            light = App.User.lightMode;
        }

        private void Submit(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (this.Owner != null)
            {
                this.Owner.Topmost = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (this.Owner != null)
            {
                this.Owner.Activate();
                this.Owner.Topmost = false;
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            App.User.lightMode = light;
            //App.User.Mail = mail;

            DialogResult = true;
        }

        public void LightMode(object sender, RoutedEventArgs e)
        {
            light = true;
            UpdateLight();
        }

        private void LightModeNIGHT(object sender, RoutedEventArgs e)
        {
            light = false;
            UpdateLight();
        }

        private void UpdateLight()
        {
            HomeView.ActCalendrier(DateTime.Now);
        }
    }
}
