using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DotAgenda.MethodClass.DataBaseMethods;
using System.Data;
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
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DotAgenda.Core;
using System.Net.Mail;
using Microsoft.SqlServer.Management.Smo;
using System.Net;
using System.Windows.Media.Animation;
using DotAgenda.MethodClass;
using System.Windows.Forms;
using Windows.System;
using MessageBox = System.Windows.MessageBox;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using DotAgenda.Models;
using DevExpress.Data.Helpers;
using Path = System.IO.Path;

namespace DotAgenda.View.LoginView
{
    /// <summary>
    /// Logique d'interaction pour LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public string prenom;
        public string SystemDB_path = @"SystemDB.db";

        Dictionary<string,string> SmtpList = new System.Collections.Generic.Dictionary<string,string>();

        public LoginForm()
        {
            InitializeComponent();

            MailTXT.LostFocus += MailTXT_LostFocus;
            MailTXT.GotFocus += MailTXT_GotFocus;
        }

        private void MailTXT_GotFocus(object sender, RoutedEventArgs e)
        {
            BorderMail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(System.Windows.Application.Current.Resources["Contrast"].ToString()));
        }

        private void MailTXT_LostFocus(object sender, RoutedEventArgs e)
        {
            BorderMail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(System.Windows.Application.Current.Resources["Tertiary"].ToString()));

        }

        //Focus Event

        private void LoginSubmit(object sender, RoutedEventArgs e)
        {
            bool UserFinded = false;
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using(SQLiteCommand command = new SQLiteCommand("SELECT ID Password FROM User WHERE Mail = @user and Password=@mdp ", connection))
                {
                    command.Parameters.AddWithValue("user", MailTXT2.Text);
                    command.Parameters.AddWithValue("mdp", Primitives._prim.CryptPassword(mdpTXT.Password.ToString()));

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            //string enteredPassword = mdpTXT.Password.ToString();
                                UserFinded = true;
                                WrongIDTXT.Visibility = Visibility.Hidden;
                                App.ID = reader.GetInt32(0);
                        }
                    }
                }
            }

            if (!UserFinded)
                WrongIDTXT.Visibility = Visibility.Visible;

            else
                DialogResult = true;
        }
        

        private void AddUser()
        {
            string nom = NomTXT.Text;
            string prenom=PrenomTXT.Text;
            string mdp = mdpTXT2.Password;
            string mail = MailTXT.Text;

            int ResterConnecter = Primitives._prim.BoolToInt((bool)CheckBoxResterConnecter.IsChecked);

            using(SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                bool UserDontExist = DataBase._db.CheckDoubleUser(connection, mail);

                if (UserDontExist)
                {
                    DataBase._db.AddUser(connection, mail, mdp, ResterConnecter);
                    App.ID = DataBase._db.GetUserID(connection, mail);

                    if (App.ID == -1)
                    {
                        //throw exception
                    }

                    DataBase._db.AddUserDetails(connection, prenom, nom, mail);
                    DataBase._db.AddDefaultClasseDB(connection);

                    string code = Primitives.GenerateRandomString();

                    SendMail(prenom, mail, code);


                    DialogResult = true;
                }

                else MessageBox.Show("Mail déjà pris !!");
            }
        }

        

        private void SendMail(string prenom, string mailAdr, string code)
        {
            new Email(prenom, mailAdr, code);
        }

        private void InscriptionClick(object sender, RoutedEventArgs e)
        {
            WrongIDTXT.Visibility = Visibility.Hidden;

            Inscription.Visibility = Visibility.Visible;
            PasDeCompte.Visibility = Visibility.Hidden;

            Connection.Visibility = Visibility.Hidden;
            DejaUnCompte.Visibility = Visibility.Visible;
        }

        private void AddUserEvent(object sender, RoutedEventArgs e)
        {
            if (IsValidEmail(MailTXT.Text) == true)
            {
                AddUser();
            }
        }

        private void ConnectionClick(object sender, RoutedEventArgs e)
        {
            Inscription.Visibility = Visibility.Hidden;
            PasDeCompte.Visibility = Visibility.Visible;

            Connection.Visibility = Visibility.Visible;
            DejaUnCompte.Visibility = Visibility.Hidden;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void CloseLogin(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mdpTXT_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void mdpTXT2_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
        }
        private void MailTXT_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool result;


            if (MailTXT.Text.Length > 0)
            {
                result = IsValidEmail(MailTXT.Text);

                if (result)
                {
                    CorrectImgMailCross.Visibility = Visibility.Hidden;
                    CorrectImgMailCheck.Visibility = Visibility.Visible;

                }

                else
                {
                    CorrectImgMailCheck.Visibility = Visibility.Hidden;
                    CorrectImgMailCross.Visibility = Visibility.Visible;
                }
            }

            else
            {
                CorrectImgMailCheck.Visibility = Visibility.Hidden;
                CorrectImgMailCross.Visibility = Visibility.Hidden;
            }
        }

        private void MailTXT2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
