using DotAgenda.Models;
using DotAgenda.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBase
    {
        private static readonly DataBase db = new DataBase();

        public static DataBase _db => db;

        public GestionnaireEvent _global;
        public GlobalDict _dict;
        public Primitives _prim;

        public DataBaseFile File;
        public DataBaseEvents Event;
        public DataBaseTodo Todo;

        private DataBase()
        {
        }

        public void InitProfil()
        {
            SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path);
            connection.Open();

            string requete = "SELECT * from Profil WHERE ID=@id";

            SQLiteCommand command = new SQLiteCommand(requete, connection);

            command.Parameters.AddWithValue("id", App.ID);
            SQLiteDataReader reader = command.ExecuteReader();

            //On récupère toutes les infos

            bool light = false, bmail = false, notif = false;
            string mail = "", prenom = "", nom = "";

            while (reader.Read())
            {


                prenom = reader["Prenom"].ToString();
                nom = reader["Nom"].ToString();
                mail = reader["Mail"].ToString();


                if (reader["LightMode"].ToString() == "0")
                    light = false;
                else
                    light = true;


                if (reader["MailAllow"].ToString() == "0")
                    bmail = false;
                else bmail = true;


                if (reader["NotificationAllow"].ToString() == "0")
                    notif = false;

                else notif = true;
            }

            //App.lightMode = light;
            connection.Close();

            string FirstLetterToUpper(string value)
            {
                string first_letter = value.ToString().Substring(0, 1);

                string new_str = first_letter.ToUpper() + value.ToString().Substring(1);

                return new_str;
            }

            App.User = new Profil(App.ID, light, bmail, notif, mail, FirstLetterToUpper(prenom), FirstLetterToUpper(nom));
        }


        public List<ClasseEvent_item> InitClasse()
        {
            List<ClasseEvent_item> Classe = new List<ClasseEvent_item>();

            SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            command.CommandText = "SELECT * FROM Classe";
            SQLiteDataReader reader = command.ExecuteReader();
            string classe, icon;
            while (reader.Read())
            {
                classe = reader["Titre"].ToString();
                icon = reader["Icon"].ToString();

                Classe.Add(new ClasseEvent_item
                {
                    Titre = classe,
                    Couleur = reader["Couleur"].ToString(),
                    Icon = icon
                });
            }

            return Classe;

        }


        public void Deconnect()
        {
            string requete = "UPDATE User SET EstConnecte = @connecte where (id=@id)";

            SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path);
            connection.Open();
            SQLiteCommand command_UseId = new SQLiteCommand(requete, connection);

            SQLiteParameter tempID = new SQLiteParameter("id", App.ID);
            SQLiteParameter tempconnecte = new SQLiteParameter("connecte", 0);
            command_UseId.Parameters.Add(tempID);
            command_UseId.Parameters.Add(tempconnecte);
            command_UseId.ExecuteNonQuery();
        }


        public void ChangeLightDB(bool light)
        {
            string requete = "UPDATE Profil SET LightMode = @light";

            SQLiteConnection c = new SQLiteConnection(App.SystemDB_Path);
            c.Open();

            //Ajout dans base de donnée


            int lightBinaire;
            if (light == true)
                lightBinaire = 1;

            else lightBinaire = 0;

            SQLiteCommand command_Add = new SQLiteCommand(requete, c);
            SQLiteParameter p1 = new SQLiteParameter("light", lightBinaire);

            command_Add.Parameters.Add(p1);

            command_Add.ExecuteNonQuery();
        }
    }
}
