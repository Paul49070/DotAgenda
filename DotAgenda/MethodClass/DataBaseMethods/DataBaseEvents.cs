using DotAgenda.Models;
using DotAgenda.ViewModels;
using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBaseEvents
    {
        private static readonly DataBaseEvents dbEvent = new DataBaseEvents();
        public static DataBaseEvents _dbEvent => dbEvent;

        public DataBase _db;
        public Primitives _prim;
        public GlobalDict _dict;
        public GestionnaireEvent _global;

        private DataBaseEvents()
        {
        }

        public void InitEvent()
        {

            string titre, classe, description, loc ;
            DateTime debut, fin;
            int ID;

            //Connection base de donnée

            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            command.CommandText = "SELECT * FROM Evenement";
            SQLiteDataReader reader = command.ExecuteReader();

            //On récupère toutes les infos

            int numY;

            while (reader.Read())
            {

                //assigns year, month, day, hour, min, seconds

                description = reader["Description"].ToString();
                loc = reader["Localisation"].ToString();

                debut = DateTime.Parse(reader["DateDebut"].ToString());
                fin = DateTime.Parse(reader["DateFin"].ToString());

                classe = reader["Classe"].ToString();

                titre = reader["Titre"].ToString();

                ID = int.Parse(reader["ID"].ToString());

                //On ajoute un speDay pour ce jour (pastille calendrier)

                numY = debut.Year - DateTime.Today.Year + 1;

                _global.A[numY].M[debut.Month - 1].J[debut.Day - 1].AddEventToList(new EventDay(ID, titre, debut, fin, loc, description, classe));


                int i = _dict.DictClasse.Keys.ToList().IndexOf(classe);
                //Classe[i].Add(A[numY].M[debut.Month - 1].J[debut.Day - 1].ListeEvent[index]);
            }

            connection.Close();
        }


        public EventDay GetEventWithID(int ID)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "SELECT * FROM Evenement WHERE ID = @id";

            SQLiteParameter p1 = new SQLiteParameter("id", ID);

            command.Parameters.Add(p1);

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                EventDay e = new EventDay(-1,
                reader["Titre"].ToString(),
                DateTime.Parse(reader["DateDebut"].ToString()),
                DateTime.Parse(reader["DateFin"].ToString()),
                "", "",
                reader["Classe"].ToString());

                int year = e.DateDebut.Year - DateTime.Today.Year + 1;


                foreach (EventDay temp in _global.A[year].M[e.DateDebut.Month - 1].J[e.DateDebut.Day - 1].ListeEvent)
                {
                    if (temp.Titre == e.Titre && temp.Classe == e.Classe && temp.DateDebut == e.DateDebut && temp.DateFin == e.DateFin)
                        return temp;
                }
            }

            return null;
        }


        public void InitFileEvent()
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "SELECT * FROM FileEvent";

            SQLiteDataReader reader = command.ExecuteReader();
            int ID_event, ID_file;
            while (reader.Read())
            {
                ID_event = int.Parse(reader["IDevent"].ToString());
                ID_file = int.Parse(reader["IDfile"].ToString());

                EventDay e = GetEventWithID(ID_event);
                Fichier f = _db.File.GetFileWithID(ID_file);

                if (e.Titre != null && f.Nom != null)
                    e.AddFile(f);
            }
        }



        public void ModifEvent(EventDay nvx)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);


            command.CommandText = "UPDATE Evenement Set Classe = @classe, Titre = @titre, DateDebut = @debut, DateFin = @fin, Description = @description, Localisation = @loc WHERE ID = @id";

            command.Parameters.AddWithValue("titre", nvx.Titre);
            command.Parameters.AddWithValue("classe", nvx.Classe);
            command.Parameters.AddWithValue("debut", nvx.DateDebut.ToString());
            command.Parameters.AddWithValue("fin", nvx.DateFin.ToString());
            command.Parameters.AddWithValue("id", nvx.ID);
            command.Parameters.AddWithValue("description", nvx.Description);
            command.Parameters.AddWithValue("loc", nvx.Lieu);

            command.ExecuteNonQuery();
        }




        public void DeleteEventToDB(EventDay EventRemove)
        {
            int id = EventRemove.ID;

            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            command.CommandText = "DELETE FROM FileEvent WHERE IDevent=@id ";

            SQLiteParameter p1 = new SQLiteParameter("id", EventRemove.ID);
            command.Parameters.Add(p1);

            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM Evenement WHERE ID = @id";

            command.Parameters[0] = new SQLiteParameter("id", id);
            command.ExecuteNonQuery();
        }


        public int AddEventToDB(EventDay EventAdd)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "INSERT INTO Evenement (Titre, Classe, DateDebut, DateFin) VALUES (@titre, @classe, @debut, @fin)";


            SQLiteParameter p1 = new SQLiteParameter("titre", EventAdd.Titre);
            SQLiteParameter p2 = new SQLiteParameter("classe", EventAdd.Classe);
            SQLiteParameter p3 = new SQLiteParameter("debut", EventAdd.DateDebut.ToString());
            SQLiteParameter p4 = new SQLiteParameter("fin", EventAdd.DateFin.ToString());

            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            command.Parameters.Add(p4);

            command.ExecuteNonQuery();

            //On récupère l'ID

            SQLiteCommand CommandGetID = new SQLiteCommand(connection);
            CommandGetID.CommandText = "SELECT ID FROM Evenement WHERE (Titre = @titre and Classe = @classe and DateDebut = @debut and DateFin = @fin)";

            p1 = new SQLiteParameter("titre", EventAdd.Titre);
            p2 = new SQLiteParameter("classe", EventAdd.Classe);
            p3 = new SQLiteParameter("debut", EventAdd.DateDebut.ToString());
            p4 = new SQLiteParameter("fin", EventAdd.DateFin.ToString());

            CommandGetID.Parameters.Add(p1);
            CommandGetID.Parameters.Add(p2);
            CommandGetID.Parameters.Add(p3);
            CommandGetID.Parameters.Add(p4);

            SQLiteDataReader reader = CommandGetID.ExecuteReader();

            while (reader.Read())
            {
                return int.Parse(reader["ID"].ToString());
            }

            return -1;
        }
    }
}
