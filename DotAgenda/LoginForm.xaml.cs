using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
            BorderMail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Application.Current.Resources["Contrast"].ToString()));
        }

        private void MailTXT_LostFocus(object sender, RoutedEventArgs e)
        {
            BorderMail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Application.Current.Resources["Tertiary"].ToString()));

        }

        //Focus Event



        private void RememberConnection()
        {
            string requete = "select * From user where EstConnecte = 1";

            SQLiteConnection connection = new SQLiteConnection("Data Source =" + SystemDB_path);
            connection.Open();

            SQLiteCommand command = new SQLiteCommand(requete, connection);

            SQLiteDataReader reader = command.ExecuteReader();

            //On récupère toutes les infos


            while (reader.Read())
            {
                if (reader["SupportdeConnection"].ToString() == Environment.MachineName)
                {
                    WrongIDTXT.Visibility = Visibility.Hidden;

                    App.ID = int.Parse(reader["ID"].ToString());

                    DialogResult = true;
                }
            }
        }
        private void LoginSubmit(object sender, RoutedEventArgs e)
        {
            string mdp=mdpTXT.Password.ToString();
            string username = MailTXT2.Text;

            bool UserFinded = false;

            string requete = "SELECT ID FROM User WHERE Username = @user AND Password = @mdp";

            SQLiteConnection connection = new SQLiteConnection("Data Source =" + SystemDB_path);
            connection.Open();

            SQLiteParameter p1 = new SQLiteParameter("user", username);
            SQLiteParameter p2 = new SQLiteParameter("mdp", mdp);

            SQLiteCommand command = new SQLiteCommand(requete, connection);

            command.Parameters.Add(p1);
            command.Parameters.Add(p2);

            SQLiteDataReader reader = command.ExecuteReader();

            //On cherche l'utilisateur correspondant

            while (reader.Read())
            {
                UserFinded = true;
                WrongIDTXT.Visibility = Visibility.Hidden;           
                App.ID = Convert.ToInt32(reader["ID"]);
            }

            if (UserFinded)
            {
                requete = "UPDATE User SET EstConnecte = @connection WHERE ID = @id";

                int RememberConnection;
                if (CheckBoxResterConnecter.IsChecked == true) RememberConnection = 1;
                else RememberConnection = 0;

                SQLiteParameter par1 = new SQLiteParameter("connection", RememberConnection);
                SQLiteParameter par2 = new SQLiteParameter("id", App.ID);

                SQLiteCommand commandUPDATE = new SQLiteCommand(requete, connection);

                commandUPDATE.Parameters.Add(par1);
                commandUPDATE.Parameters.Add(par2);

                commandUPDATE.ExecuteNonQuery();
                UpdateConnectionDB();
                DialogResult = true;
            }

            //Si aucun utilisateur de correspond

            if(UserFinded==false)
            {
                WrongIDTXT.Visibility = Visibility.Visible;
            }
        }

        private void UpdateConnectionDB()
        {
            string requete = "UPDATE User SET EstConnecte = @connecte where (id!=@id)";

            SQLiteConnection connection = new SQLiteConnection("Data Source =" + SystemDB_path);
            connection.Open();
            SQLiteCommand command_UseId = new SQLiteCommand(requete, connection);

            int temp = 0;

            SQLiteParameter tempID = new SQLiteParameter("id", App.ID);
            SQLiteParameter tempconnecte = new SQLiteParameter("connecte", temp);
            command_UseId.Parameters.Add(tempID);
            command_UseId.Parameters.Add(tempconnecte);
            command_UseId.ExecuteNonQuery();
        }

        private void AddUser()
        {
            //Pour UserDB

            string nom=NomTXT.Text, prenom=PrenomTXT.Text;

            //Pour system db

            string mdp = mdpTXT2.Password;
            string username = MailTXT.Text;
            int ResterConnecter;

            if (CheckBoxResterConnecter.IsChecked == true) ResterConnecter = 1;
            else ResterConnecter = 0;

            string requete = "INSERT INTO User (Username, Password, EstConnecte, SupportdeConnection) VALUES (@user, @mdp, @connecter, @appareil)";

            SQLiteConnection connection = new SQLiteConnection("Data Source =" + SystemDB_path);
            connection.Open();

            //Ajout dans base de donnée

            SQLiteParameter p1 = new SQLiteParameter("user", username);
            SQLiteParameter p2 = new SQLiteParameter("mdp", mdp);
            SQLiteParameter p1bis = new SQLiteParameter("connecter", ResterConnecter);
            SQLiteParameter p2bis = new SQLiteParameter("appareil", Environment.MachineName);

            SQLiteCommand command_Add = new SQLiteCommand(requete, connection);

            command_Add.Parameters.Add(p1);
            command_Add.Parameters.Add(p2);
            command_Add.Parameters.Add(p1bis);
            command_Add.Parameters.Add(p2bis);

            command_Add.ExecuteNonQuery();

            //On récupère l'ID de l'utilisateur créé

            requete = "SELECT ID FROM User where Username = @user AND Password = @mdp";

            SQLiteCommand command_SelectID = new SQLiteCommand(requete, connection);

            command_SelectID.Parameters.Add(p1);
            command_SelectID.Parameters.Add(p2);

            SQLiteDataReader reader2 = command_SelectID.ExecuteReader();

            //On récupère toutes les infos

            if(reader2.Read())
            {   
                App.ID = int.Parse(reader2["ID"].ToString());
            }

            UpdateConnectionDB();


            //On créer la database du user

            string CheminCreaDB = "UserDataBase/" + App.ID.ToString() + "_DataBase.db";

            SQLiteConnection.CreateFile(@CheminCreaDB);

            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + CheminCreaDB + ";Version=3;");
            m_dbConnection.Open();

            //On créer les tables

            //Classe

            string sql = "create table Classe (Titre varchar(30), Couleur varchar(7), Icon varchar(30))";
            SQLiteCommand cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();


            //Les 3 classes par défault

            sql = "INSERT INTO Classe (Titre, Couleur, Icon) Values('Important','#ff4f78','AlertCircleOutline')";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            sql = "INSERT INTO Classe (Titre, Couleur, Icon) Values('Loisir','#4fa2ff','RobotHappyOutline')";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            sql = "INSERT INTO Classe (Titre, Couleur, Icon) Values('Cours','#FFB93F','BookOutline')";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            sql = "INSERT INTO Classe (Titre, Couleur, Icon) Values('Divers','#3fff92','ArchiveOutline')";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();


            //Evenement

            sql = "create table Evenement (Titre varchar(30), Classe char(30), Description varchar(600), Localisation varchar(200), ID INTEGER, DateDebut varchar(100), DateFin varchar(100), PRIMARY KEY(ID))";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            //Todo

            sql = "create table Todo (Titre varchar(30), Projet varchar(30), Etat INTEGER, Date varchar(50), ID INTEGER, PRIMARY KEY(ID))";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            //Fichiers

            sql = "create table Fichiers (Nom varchar(200), DateAjout varchar(50), ID INTEGER, PRIMARY KEY(ID))";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            //FichiersEvents

            sql = "create table FileEvent (IDfile INTEGER, IDevent INTEGER)";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            //DossiersPersos

            sql = "create table Dossiers (Nom varchar(200), Icon varchar(50), Couleur varchar(10), ID INTEGER, PRIMARY KEY(ID))";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            //Profil

            sql = "create table Profil (Prenom varchar(30), Nom varchar(30), ID INTEGER, Mail varchar(30), LightMode integer, MailAllow integer, NotificationAllow integer, PRIMARY KEY(ID))";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);
            cmd_AddTable.ExecuteNonQuery();

            sql = "INSERT INTO Profil (Prenom, Nom, ID, Mail, LightMode, MailAllow, NotificationAllow) VALUES(@prenom, @nom, @id, @mail, @light, @MA, @NA)";
            cmd_AddTable = new SQLiteCommand(sql, m_dbConnection);

            p1 = new SQLiteParameter("prenom", prenom);
            p2 = new SQLiteParameter("nom", nom);


            SQLiteParameter p3 = new SQLiteParameter("id", App.ID);
            SQLiteParameter p4 = new SQLiteParameter("mail", username);
            SQLiteParameter p5 = new SQLiteParameter("light", 0);
            SQLiteParameter p6 = new SQLiteParameter("MA", 1);
            SQLiteParameter p7 = new SQLiteParameter("NA", 1);

            cmd_AddTable.Parameters.Add(p1);
            cmd_AddTable.Parameters.Add(p2);
            cmd_AddTable.Parameters.Add(p3);
            cmd_AddTable.Parameters.Add(p4);
            cmd_AddTable.Parameters.Add(p5);
            cmd_AddTable.Parameters.Add(p6);
            cmd_AddTable.Parameters.Add(p7);

            cmd_AddTable.ExecuteNonQuery();

            m_dbConnection.Close();
            SendMail(username);
            DialogResult = true;
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
            RememberConnection();
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
