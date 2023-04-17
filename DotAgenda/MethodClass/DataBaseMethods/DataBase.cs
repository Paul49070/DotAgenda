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

        public void UpdateConnectionDB()
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                bool RowAlreadyExists;

                using (SQLiteCommand commandCheck = new SQLiteCommand("SELECT * FROM UserConnection WHERE Machine=@machine", connection))
                {
                    commandCheck.Parameters.AddWithValue("machine", Environment.MachineName);


                    using (SQLiteDataReader reader = commandCheck.ExecuteReader())
                        if (reader.Read())
                            RowAlreadyExists = true;
                        else RowAlreadyExists = false;
                }

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.Parameters.AddWithValue("userID", App.ID);
                    command.Parameters.AddWithValue("machine", Environment.MachineName);
                    command.Parameters.AddWithValue("date", DateTime.Now.ToString("s"));

                    if (RowAlreadyExists)
                        command.CommandText = "UPDATE UserConnection SET UserID = @userID, LastModification=@date WHERE Machine=@machine";

                    else command.CommandText = "INSERT INTO UserConnection (UserID, Machine, LastModification) VALUES (@userID, @machine, @date)";

                    command.ExecuteNonQuery();
                }
                

                using (SQLiteCommand command_Delete = new SQLiteCommand("DELETE FROM UserConnection WHERE LastModification < @limitDate", connection))
                {
                    command_Delete.Parameters.AddWithValue("limitDate", DateTime.Now.AddDays(-30));
                    command_Delete.ExecuteNonQuery();
                }
            }
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
                using (SQLiteCommand command = new SQLiteCommand("UPDATE UserConnection SET UserID = -1, LastModification=@date WHERE Machine=@machine",connection))
                {
                    command.Parameters.AddWithValue("machine", Environment.MachineName);
                    command.Parameters.AddWithValue("date", DateTime.Now.ToString("s"));

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

        public bool CheckDoubleUser(SQLiteConnection connection, string mail)
        {
            bool NotalreadyExist;

            using (SQLiteCommand commandCheckAlreadyExists = new SQLiteCommand("SELECT * FROM User WHERE Mail = @mail", connection))
            {
                commandCheckAlreadyExists.Parameters.AddWithValue("mail", mail);

                using (SQLiteDataReader reader = commandCheckAlreadyExists.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        NotalreadyExist = false; //check if there is already somebody with the mail
                    }


                    else NotalreadyExist = true;
                }
            }

            return NotalreadyExist;
        }

        public void AddUser(SQLiteConnection connection, string mail, string mdp, int ResterConnecter)
        { 
            using (SQLiteCommand command_Add = new SQLiteCommand("INSERT INTO User (Password, Mail, EstConnecte) VALUES (@mdp, @user, @connecter)", connection))
            {
                command_Add.Parameters.AddWithValue("user", mail);
                command_Add.Parameters.AddWithValue("mdp", Primitives._prim.CryptPassword(mdp));
                command_Add.Parameters.AddWithValue("connecter", ResterConnecter);

                command_Add.ExecuteNonQuery();
            }
        }

        public int GetUserID(SQLiteConnection connection, string mail)
        {
            int ID = -1;

            using (SQLiteCommand command_Select = new SQLiteCommand("SELECT ID FROM User where Mail = @user", connection))
            {
                command_Select.Parameters.AddWithValue("user", mail);

                using (SQLiteDataReader reader = command_Select.ExecuteReader())
                {
                    if (reader.Read())
                        ID = reader.GetInt32(0);
                }
            }

            return ID;
        }

        public void AddUserDetails(SQLiteConnection connection, string prenom, string nom, string mail)
        {
            using (SQLiteCommand command_UserDetails = new SQLiteCommand("INSERT INTO UserDetails (UserID, MailAllow, NotificationAllow, LightTheme, Prenom, Nom, Mail) VALUES (@id, 0, 0, 0, @prenom, @nom, @mail)", connection))
            {
                string FirstLetterToUpper(string str)
                {
                    string first_letter = str.ToString().Substring(0, 1);

                    string new_str = first_letter.ToUpper() + str.ToString().Substring(1);

                    return new_str;
                }

                command_UserDetails.Parameters.AddWithValue("id", App.ID);
                command_UserDetails.Parameters.AddWithValue("prenom", FirstLetterToUpper(prenom));
                command_UserDetails.Parameters.AddWithValue("nom", FirstLetterToUpper(nom));
                command_UserDetails.Parameters.AddWithValue("mail", mail);

                command_UserDetails.ExecuteNonQuery();
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
