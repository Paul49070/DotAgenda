using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;


namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBaseFile
    {
        private static readonly DataBaseFile dbFile = new DataBaseFile();
        public static DataBaseFile _dbFile => dbFile;

        public DataBase _db;
        public Primitives _prim;
        public GlobalDict _dict;
        public GestionnaireEvent _global;

        private DataBaseFile()
        {
        }

        public void DeleteFileToDB(Fichier fic)
        {

            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "DELETE FROM Fichiers WHERE Nom=@nom ";

            SQLiteParameter p1 = new SQLiteParameter("nom", fic.Nom);
            command.Parameters.Add(p1);

            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM FileEvent WHERE IDfile=@idfile ";
            command.Parameters[0] = new SQLiteParameter("idfile", fic.ID);

            command.ExecuteNonQuery();

        }

        public void AddFileToDB(string nomFichier)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "INSERT INTO Fichiers(Nom, DateAjout) VALUES (@nom, @date)";


            SQLiteParameter p1 = new SQLiteParameter("nom", nomFichier);
            SQLiteParameter p2 = new SQLiteParameter("date", DateTime.Today);

            command.Parameters.Add(p1);
            command.Parameters.Add(p2);

            command.ExecuteNonQuery();
        }



        public int GetFileID(string nomFichier)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "SELECT ID FROM Fichiers WHERE Nom=@nom";


            SQLiteParameter p1 = new SQLiteParameter("nom", nomFichier);

            command.Parameters.Add(p1);

            SQLiteDataReader reader = command.ExecuteReader();

            int id = -1;

            while (reader.Read())
            {
                id = int.Parse(reader["ID"].ToString());
            }

            return id;
        }



        public void DeleteFileToDB_Event(Fichier fic, EventDay evenement)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "DELETE FROM FileEvent WHERE (IDfile = @idfile and IDevent = @idevent)";

            SQLiteParameter p1 = new SQLiteParameter("idfile", int.Parse(fic.ID.ToString()));
            SQLiteParameter p2 = new SQLiteParameter("idevent", int.Parse(evenement.ID.ToString()));

            command.Parameters.Add(p1);
            command.Parameters.Add(p2);

            command.ExecuteNonQuery();
        }

        public void AddFileToDB_Event(Fichier fic, EventDay evenement)
        {

            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "INSERT INTO FileEvent (IDfile, IDevent) VALUES (@idfile, @idevent)";

            SQLiteParameter p1 = new SQLiteParameter("idfile", int.Parse(fic.ID.ToString()));
            SQLiteParameter p2 = new SQLiteParameter("idevent", int.Parse(evenement.ID.ToString()));

            command.Parameters.Add(p1);
            command.Parameters.Add(p2);

            command.ExecuteNonQuery();
        }



        public Fichier GetFileWithID(int id)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "SELECT * FROM Fichiers WHERE ID = @id";

            SQLiteParameter p1 = new SQLiteParameter("id", id);

            command.Parameters.Add(p1);

            SQLiteDataReader reader = command.ExecuteReader();

            Fichier fic = new Fichier();

            while (reader.Read())
            {
                fic.Nom = reader["Nom"].ToString();
            }

            foreach (Fichier temp in _global.ListeFichiers)
            {
                if (temp.Nom == fic.Nom)
                    return temp;
            }

            return fic;
        }



        public ObservableCollection<Fichier> SetListeFichiers()
        {
            ObservableCollection<Fichier> ListeFichiers = new ObservableCollection<Fichier>();

            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "SELECT * FROM Fichiers";

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Fichier fichier = new Fichier();

                fichier.Nom = reader["Nom"].ToString();
                fichier.DateAjout = DateTime.Parse(reader["DateAjout"].ToString());
                fichier.ID = int.Parse(reader["ID"].ToString());

                FileInfo fi = new FileInfo(fichier.Nom);

                try
                {
                    fichier.Type = _dict.ExtensionDict[fi.Extension];
                }

                catch
                {
                    fichier.Type = _dict.Autre;
                }

                fichier.AddToFolderType();

                ListeFichiers.Add(fichier);
            }

            return ListeFichiers;
        }
    }
}
