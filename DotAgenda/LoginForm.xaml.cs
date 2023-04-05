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

namespace DotAgenda
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

            InitSmtpList();
        }

        private void InitSmtpList()
        {
            SmtpList.Add("neuf", "smtp.neuf.fr");
            SmtpList.Add("aliceadsl", "smtp.aliceadsl.fr");
            SmtpList.Add("aol", "smtp.aol.com");
            SmtpList.Add("att", "outbound.att.net");
            SmtpList.Add("bluewin", "smtpauths.bluewin.ch");
            SmtpList.Add("bouygtel", "smtp.bouygtel.fr");
            SmtpList.Add("club-internet", "mail.club-internet.fr");
            SmtpList.Add("free", "smtp.free.fr");
            SmtpList.Add("gmail", "smtp.gmail.com");
            SmtpList.Add("ifrance", "smtp.ifrance.fr");
            SmtpList.Add("hotmail", "smtp.live.com");
            SmtpList.Add("laposte", "smtp.laposte.fr");
            SmtpList.Add("netcourrier", "smtp.netcourrier.com");
            SmtpList.Add("o2", "smtp.o2.com");
            SmtpList.Add("outlook", "smtp.live.com");
            SmtpList.Add("sympatico", "smtphm.sympatico.ca");
            SmtpList.Add("tiscali", "smtp.tiscali.fr");
            SmtpList.Add("verizon", "outgoing.verizon.net");
            SmtpList.Add("voila", "smtp.voila.fr");
            SmtpList.Add("wanadoo", "smtp.wanadoo.fr");
            SmtpList.Add("yahoo", "mail.yahoo.com");

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
                using(SQLiteCommand command = new SQLiteCommand("SELECT ID FROM User WHERE Mail = @user AND Password = @mdp", connection))
                {
                    command.Parameters.AddWithValue("user", MailTXT2.Text);
                    command.Parameters.AddWithValue("mdp", mdpTXT.Password.ToString());

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            UserFinded = true;
                            WrongIDTXT.Visibility = Visibility.Hidden;
                            App.ID = reader.GetInt32(0);
                        }
                    }
                }
            }

            if (!UserFinded)
            {
                WrongIDTXT.Visibility = Visibility.Visible;
            }

            else
            {
                UpdateConnectionDB();
                DialogResult = true;
            }
        }


        private void UpdateConnectionDB()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("UPDATE User SET EstConnecte = 0 where (id!=@id)", connection))
                {
                    command.Parameters.AddWithValue("id", App.ID);
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command_Update = new SQLiteCommand("UPDATE User SET EstConnecte = @connection WHERE ID = @id", connection))
                {
                    int RememberConnection;
                    if (CheckBoxResterConnecter.IsChecked == true) RememberConnection = 1;
                    else RememberConnection = 0;

                    command_Update.Parameters.AddWithValue("connection", RememberConnection);
                    command_Update.Parameters.AddWithValue("id", App.ID);

                    command_Update.ExecuteNonQuery();
                }
            }
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
                DataBase._db.AddUser(connection, mail, mdp, ResterConnecter);
                App.ID = DataBase._db.GetUserID(connection, mail, mdp);

                if (App.ID == -1)
                {
                    //throw exception
                }

                DataBase._db.AddUserDetails(connection, prenom, nom, mail);
                DataBase._db.AddDefaultClasseDB(connection);

                UpdateConnectionDB();
                SendMail(mail);


                DialogResult = true;
            }
        }

        private void SendMail(string mailAdr)
        {
            int Debutsmtp = mailAdr.IndexOf("@")+1;
            string server_temp = mailAdr.Substring(Debutsmtp, mailAdr.Length - Debutsmtp);
            int FinSmtp = server_temp.IndexOf(".");
            server_temp = server_temp.Substring(0, FinSmtp);
            server_temp = server_temp.ToLower();
            string server = SmtpList[server_temp];

            MailAddress from = new MailAddress("DotAgendaContact1@gmail.com");
            MailAddress to = new MailAddress(mailAdr);



            MailMessage message = new MailMessage(from, to);
            message.Subject = "Création de compte .agenda !";
            string Body = @"<!DOCTYPE html>

<html>
<head>
<title></title>
<meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type""/>
<meta content=""width=device-width, initial-scale=1.0"" name=""viewport""/>
<html>
<style>
	@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300&display=swap');


	body
	{
		font-family: 'Lucida', monospace;
	}

	h1
	{
		font-family: 'Lucida', monospace;
		text-align: center;
		color: black;
		font-size: 30px;
	}

	.box {
            display: flex;
            align-content: center;
            text-align: center;
            justify-content: center;
          }
      

	h2
	{
		font-family: 'Lucida', monospace;
		text-align: center;
		color: black;
		font-weight: normal;
		font-size: 22px;
	}

	#point{
		color:#573fff;
		letter-spacing: -1px;
		font-size: 25px;
	}

	p
	{
		text-align: center;
		font-size: 14px;

	}
</style>
<body>
	<table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
    <tr>
    	<td>
    	<h1>
		.agenda
	</h1>
    </td>
</td>


</tr>
	<tr>
	<td width=""auto"" align=""center"">        
	    	<img src=cid:ImageMail width=""200"">

		</td>
    </tr>

<tr height= ""5px"">
	<td style=""font-weight: 2000px"">
	<h1 style=""font-size: 22px"">
		Bienvenue !
	</h1>
	<p>
		Votre compte à bien été créé !
	</p>
</td>
</tr>

</table>
	

</body>
</html>";
            message.IsBodyHtml = true;

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");

            LinkedResource mailImage = new LinkedResource("C:\\Users\\paull\\OneDrive\\Bureau\\DotAgenda\\DotAgenda\\DotAgenda\\Img\\Mail\\HappyMail.png");
            mailImage.ContentId = "ImageMail";

            htmlView.LinkedResources.Add(mailImage);

            message.AlternateViews.Add(htmlView);
            //message.Body = @"Bienvenue ! Ton compte à bien été créer. Au plaisir d'organiser ton planning ensemble !";


            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential("DotAgendaContact1@gmail.com", "lantpoajyrhsgone")
            };            // Include credentials if the server requires them.

            try
            {
                smtpClient.Send(message);
            }
            catch
            {
                MessageBox.Show("Impossible d'envoyer le mail de confirmation !", "Error", MessageBoxButton.OK);
            }
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
            bool result=false;


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

        private void WrongValues_Anim()
        {
            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true,
            };


            //this.RegisterName("WidthNav", NavBar.Width);
            //Storyboard.SetTargetName(widthAnimation, "WidthNav");
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Window.LeftProperty));
            Storyboard.SetTarget(widthAnimation, LoginFormXML);

            Storyboard s = new Storyboard();
            s.Children.Add(widthAnimation);
            s.Duration = TimeSpan.FromSeconds(20);
            s.Begin();
        }

        private void MailTXT2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
