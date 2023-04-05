using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBaseTodo
    {
        private static readonly DataBaseTodo dbTodo = new DataBaseTodo();
        public static DataBaseTodo _dbTodo => dbTodo;

        protected DataBaseTodo()
        {
        }

        public void InitTodo()
        {
            GestionnaireEvent _global = GestionnaireEvent._global;

            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Todo WHERE UserID = @userID", connection))
                {
                    command.Parameters.AddWithValue("userID", App.User.id);

                    bool etat;
                    string titre, classe, ID;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ID = reader.GetString(1);
                            titre = reader.GetString(2);
                            classe = reader.GetString(3);
                            etat = reader.GetBoolean(4);

                            if (DateTime.TryParse(reader.GetString(5), out DateTime debut))
                            {
                                TodoItem Todo;

                                if (DateTime.TryParse(reader.GetString(6), out DateTime fin))
                                {
                                    //ajouter la date de fin
                                     new TodoItem(titre, debut, classe, etat, ID);
                                }

                                else
                                {
                                     new TodoItem(titre, debut, classe, etat, ID);
                                }

                            }
                        }
                    }
                }
            }
        }


        public bool AjouterTodo(TodoItem TodoAdd)
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Todo (UserID, ID, Titre, Projet, Etat, DateDebut, DateFin) VALUES (@userID, @ID, @titre, @projet, @etat, @start, @end)", connection))
                {
                    command.Parameters.AddWithValue("userID", App.ID);
                    command.Parameters.AddWithValue("ID", TodoAdd.ID);
                    command.Parameters.AddWithValue("titre", TodoAdd.Titre);
                    command.Parameters.AddWithValue("projet", TodoAdd.Classe);
                    command.Parameters.AddWithValue("etat", Primitives._prim.BoolToInt(TodoAdd.Fini));
                    command.Parameters.AddWithValue("start", TodoAdd.DateDebut.ToString("s"));
                    command.Parameters.AddWithValue("end", TodoAdd.DateFin.ToString("s"));

                    command.ExecuteNonQuery();
                }
            }

            return true;
        }


        public void ChangeEtatTacheDB(TodoItem Tache)
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("UPDATE Todo SET Etat = @etat WHERE (ID = @id and UserID = @userID)", connection))
                {
                    command.Parameters.AddWithValue("userID", App.ID);
                    command.Parameters.AddWithValue("id", Tache.ID);
                    command.Parameters.AddWithValue("etat", Primitives._prim.BoolToInt(Tache.Fini));

                    command.ExecuteNonQuery();
                }
            }
        }



        public void DeleteTodoDB(TodoItem Tache)
        {
            //Connection DB

            using (SQLiteConnection connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("DELETE FROM Todo WHERE (ID = @id and UserID = @userID)", connection))
                {
                    command.Parameters.AddWithValue("userID", App.ID);
                    command.Parameters.AddWithValue("id", Tache.ID);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
