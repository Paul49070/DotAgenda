using DotAgenda.Models;
using DotAgenda.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBase
    {
        private static readonly DataBase db = new DataBase();

        public static DataBase _db => db;
        protected DataBase()
        {
        }

        public void InitProfil()
        {
            bool bmail = true, notif = true, light = true;
            string prenom = "", nom = "", mail = "";

            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * from UserDetails WHERE UserID=@id", connection))
                {
                    command.Parameters.AddWithValue("id", App.ID);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bmail = reader.GetBoolean(1);
                            notif = reader.GetBoolean(2);
                            light = reader.GetBoolean(3);

                            prenom = reader.GetString(4);
                            nom = reader.GetString(5);
                            mail = reader.GetString(6);
                        }

                        else
                        {
                            //Throw new exception
                        }
                    }
                }
            }

            App.User = new Profil(App.ID, light, bmail, notif, mail, prenom, nom);
        }

        public void InitClasse()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Classe", connection))
                {
                    using(SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           new ClasseEvent_item(reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        }
                    }
                }
            }          
        }


        public void Deconnect()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("UPDATE User SET EstConnecte = 0 where (id=@id)", connection))
                {
                    command.Parameters.AddWithValue("id", App.ID);
                    command.ExecuteNonQuery();
                }
            }
        }


        public void ChangeLightDB(bool light)
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("UPDATE UserDetails SET LightTheme = @light WHERE UserID = @id", connection))
                {
                    int lightBinaire;
                    if (light == true)
                        lightBinaire = 1;

                    else lightBinaire = 0;

                    command.Parameters.AddWithValue("id", App.ID);
                    command.Parameters.AddWithValue("light", lightBinaire);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddUser(SQLiteConnection connection, string mail, string mdp, int ResterConnecter)
        {
            using (SQLiteCommand command_Add = new SQLiteCommand("INSERT INTO User (Password, Mail, EstConnecte, SupportdeConnection) VALUES (@mdp, @user, @connecter, @appareil)", connection))
            {
                command_Add.Parameters.AddWithValue("user", mail);
                command_Add.Parameters.AddWithValue("mdp", mdp);
                command_Add.Parameters.AddWithValue("connecter", ResterConnecter);
                command_Add.Parameters.AddWithValue("appareil", Environment.MachineName);

                command_Add.ExecuteNonQuery();
            }
        }

        public int GetUserID(SQLiteConnection connection, string mail, string mdp)
        {
            using (SQLiteCommand command_Select = new SQLiteCommand("SELECT ID FROM User where Mail = @user AND Password = @mdp", connection))
            {
                command_Select.Parameters.AddWithValue("user", mail);
                command_Select.Parameters.AddWithValue("mdp", mdp);

                using (SQLiteDataReader reader = command_Select.ExecuteReader())
                {
                    if (reader.Read())
                        return reader.GetInt32(0);
                }
            }

            return -1;
        }

        public void AddUserDetails(SQLiteConnection connection, string prenom, string nom, string mail)
        {
            using (SQLiteCommand command_Add_UserDetails = new SQLiteCommand("INSERT INTO UserDetails (UserID, MailAllow, NotificationAllow, LightTheme, Prenom, Nom, Mail) VALUES (@id, 0, 0, 0, @prenom, @nom, @mail)", connection))
            {
                string FirstLetterToUpper(string str)
                {
                    string first_letter = str.ToString().Substring(0, 1);

                    string new_str = first_letter.ToUpper() + str.ToString().Substring(1);

                    return new_str;
                }

                command_Add_UserDetails.Parameters.AddWithValue("id", App.ID);
                command_Add_UserDetails.Parameters.AddWithValue("prenom", FirstLetterToUpper(prenom));
                command_Add_UserDetails.Parameters.AddWithValue("nom", FirstLetterToUpper(nom));
                command_Add_UserDetails.Parameters.AddWithValue("mail", mail);

                command_Add_UserDetails.ExecuteNonQuery();
            }
        }

        public void AddDefaultClasseDB(SQLiteConnection connection)
        {
            foreach (string[] str in App.ListeDefaultClasse)
            {
                using (SQLiteCommand command_Add_DefaultClass = new SQLiteCommand("INSERT INTO Classe (UserID, ID, Titre, Couleur, Icon) VALUES(@userId, @id, @titre, @couleur, @icon)", connection))
                {
                    command_Add_DefaultClass.Parameters.AddWithValue("userID", App.ID);
                    command_Add_DefaultClass.Parameters.AddWithValue("id", Guid.NewGuid().ToString());
                    command_Add_DefaultClass.Parameters.AddWithValue("titre", str[0]);
                    command_Add_DefaultClass.Parameters.AddWithValue("couleur", str[1]);
                    command_Add_DefaultClass.Parameters.AddWithValue("icon", str[2]);

                    command_Add_DefaultClass.ExecuteNonQuery();
                }
            }
        }
    }
}
