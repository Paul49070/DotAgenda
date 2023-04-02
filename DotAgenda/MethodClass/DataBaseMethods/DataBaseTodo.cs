using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;


namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBaseTodo
    {
        private static readonly DataBaseTodo dbTodo = new DataBaseTodo();
        public static DataBaseTodo _dbTodo => dbTodo;

        public DataBase _db;
        public Primitives _prim;
        public GlobalDict _dict;
        public GestionnaireEvent _global;

        private DataBaseTodo()
        {
        }

        public void InitTodo()
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            command.CommandText = "SELECT * FROM Todo";
            SQLiteDataReader reader = command.ExecuteReader();
            bool TodoFini;

            while (reader.Read())
            {
                if (reader["Etat"].ToString() == "1") TodoFini = true;
                else TodoFini = false;

                TodoItem Tache = new TodoItem(

                    reader["Titre"].ToString(), 

                    DateTime.Parse(reader["Date"].ToString()), 

                    reader["Projet"].ToString(), 

                    TodoFini, 

                    reader["ID"].ToString()
                );

                int index;

                if (Tache.Fini)
                    index = _global.A[Tache.DateDebut.Year - DateTime.Today.Year + 1].M[Tache.DateDebut.Month - 1].J[Tache.DateDebut.Day - 1].Todo.Count;

                else index = _prim.FindTodoIndex(Tache);

                _global.A[Tache.DateDebut.Year - DateTime.Today.Year + 1].M[Tache.DateDebut.Month - 1].J[Tache.DateDebut.Day - 1].AjouterTodoToList(index, Tache);
            }
        }


        public TodoItem AjouterTodo(TodoItem TodoAdd)
        {
            //Connection Base de Donnée

            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            //Ajout dans base de donnée

            command.CommandText = "INSERT INTO Todo (Titre, Projet, Etat, Date) VALUES (@titre, @projet, @etat, @date)";


            SQLiteParameter p1 = new SQLiteParameter("titre", TodoAdd.Titre);
            SQLiteParameter p2 = new SQLiteParameter("projet", TodoAdd.Classe);
            SQLiteParameter p3 = new SQLiteParameter("etat", false);
            SQLiteParameter p4 = new SQLiteParameter("date", TodoAdd.DateDebut);

            command.Parameters.Add(p1);
            command.Parameters.Add(p2);
            command.Parameters.Add(p3);
            command.Parameters.Add(p4);

            command.ExecuteNonQuery();

            _global.A[TodoAdd.DateDebut.Year - DateTime.Today.Year + 1].M[TodoAdd.DateDebut.Month - 1].J[TodoAdd.DateDebut.Day - 1].AjouterTodoToList(_prim.FindTodoIndex(TodoAdd), TodoAdd);

            return TodoAdd;
        }


        public void ChangeEtatTacheDB(TodoItem Tache)
        {
            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = "UPDATE Todo SET Etat = @etat WHERE (ID = @id)"
            };

            int etat;
            if (Tache.Fini == true) etat = 1;
            else etat = 0;

            SQLiteParameter p1 = new SQLiteParameter("etat", etat);
            SQLiteParameter p2 = new SQLiteParameter("id", Tache.ID);

            command.Parameters.Add(p1);
            command.Parameters.Add(p2);

            command.ExecuteNonQuery();
        }



        public void DeleteTodoDB(TodoItem Tache)
        {
            //Connection DB

            SQLiteConnection connection = new SQLiteConnection(App.DB_Path);
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);

            command.CommandText = "DELETE FROM Todo WHERE (ID = @id)";

            //Suppression dans Base de Donnees

            SQLiteParameter p1 = new SQLiteParameter("id", Tache.ID);

            command.Parameters.Add(p1);
            command.ExecuteNonQuery();
        }
    }
}
