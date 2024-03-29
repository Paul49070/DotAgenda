﻿using DevExpress.Utils.About;
using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBaseFile
    {
        private static readonly DataBaseFile dbFile = new DataBaseFile();
        public static DataBaseFile _dbFile => dbFile;

        protected DataBaseFile()
        {
        }

        public void DeleteFileToDB(Fichier fic)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("DELETE FROM FileEvent WHERE IDfile=@id and UserID=@userID", connection))
                {
                    command.Parameters.AddWithValue("@userID", App.User.id);
                    command.Parameters.AddWithValue("@id", fic.ID);
                    command.ExecuteNonQuery();

                    command.CommandText = "DELETE FROM Fichiers WHERE ID = @id and UserID = @userID";
                    command.ExecuteNonQuery();
                }
            }

        }

        public bool AddFileToDB(Fichier fic)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Fichiers(UserID, ID, Nom, DateAjout) VALUES(@userID, @id, @nom, @date)", connection))
                {
                    command.Parameters.AddWithValue("userID", App.User.id);
                    command.Parameters.AddWithValue("id", fic.ID);
                    command.Parameters.AddWithValue("nom", fic.Nom);
                    command.Parameters.AddWithValue("date",fic.DateAjout.ToString("s"));
                    command.ExecuteNonQuery();
                    return true;
                }
            }
        }



        public void DeleteFileToDB_Event(Fichier fic, EventDay evenement)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("DELETE FROM FileEvent WHERE (IDfile = @idfile and IDevent = @idevent and UserID = @userID)", connection))
                {
                    command.Parameters.AddWithValue("@userID", App.User.id);
                    command.Parameters.AddWithValue("@idevent", evenement.ID);
                    command.Parameters.AddWithValue("@idfile", fic.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddFileToDB_Event(Fichier fic, EventDay evenement)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO FileEvent (UserID, IDfile, IDevent) VALUES (@userID, @idfile, @idevent)", connection))
                {
                    command.Parameters.AddWithValue("@userID", App.User.id);
                    command.Parameters.AddWithValue("@idevent", evenement.ID);
                    command.Parameters.AddWithValue("@idfile", fic.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Fichier GetFileWithID(string id)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Fichiers WHERE ID = @id and UserID = @userID", connection))
                {
                    command.Parameters.AddWithValue("userID", App.User.id);
                    command.Parameters.AddWithValue("id", id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            foreach (Fichier fic in GestionnaireEvent._global.ListeFichiers)
                            {
                                if (fic.ID == id)
                                     return fic;
                            }
                        }
                    }

                    return null;
                }
            }
        }



        public void SetListeFichiers()
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Fichiers WHERE UserID = @userID", connection))
                {
                    command.Parameters.AddWithValue("userID", App.ID);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        string ID, Nom;
                        DateTime DateAjout;

                        while (reader.Read())
                        {
                            ID = reader.GetString(1);
                            Nom = reader.GetString(2);

                            if(DateTime.TryParse(reader.GetString(3).ToString(), out DateAjout))
                            {
                                new Fichier(Nom, ID, DateAjout);
                            }

                        }
                    }
                }
            }
        }
    }
}
