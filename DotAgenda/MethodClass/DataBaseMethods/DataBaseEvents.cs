using DotAgenda.Models;
using DotAgenda.ViewModels;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.SqlServer.Management.XEvent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace DotAgenda.MethodClass.DataBaseMethods
{
    public class DataBaseEvents
    {
        private static readonly DataBaseEvents dbEvent = new DataBaseEvents();
        public static DataBaseEvents _dbEvent => dbEvent;
        protected DataBaseEvents()
        {
        }

        public void InitEvent()
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Evenement WHERE UserID = @userID", connection))
                {
                    command.Parameters.AddWithValue("userID", App.User.id);

                    int numY;
                    string titre, classe, description, loc, ID, GroupID;
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ID = reader.GetString(1);
                            GroupID = reader.GetString(2);
                            titre = reader.GetString(3);
                            classe = reader.GetString(4);
                            description = reader.GetString(5);
                            loc = reader.GetString(6);

                            if (DateTime.TryParse(reader.GetString(7), out DateTime debut) && DateTime.TryParse(reader.GetString(8), out DateTime fin))
                            {
                                numY = debut.Year - DateTime.Today.Year + 1;

                                GestionnaireEvent._global.A[numY].M[debut.Month - 1].J[debut.Day - 1].AddEventToList(new EventDay(titre, debut, fin, classe, ID, GroupID, location:loc, desc:description));
                            }
                        }
                    }
                }
            }
        }


        public EventDay GetEventWithID(string ID)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT DateDebut, DateFin FROM Evenement WHERE ID = @id and UserID = @userID", connection))
                {
                    command.Parameters.AddWithValue("userID", App.User.id);
                    command.Parameters.AddWithValue("id", ID);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (DateTime.TryParse(reader.GetString(0), out DateTime start) && DateTime.TryParse(reader.GetString(1), out DateTime end))
                            {

                                int year = start.Year - DateTime.Today.Year + 1;
                                foreach (EventDay eventDay in GestionnaireEvent._global.A[year].M[start.Month - 1].J[start.Day - 1].ListeEvent)
                                {
                                    if (eventDay.ID == ID)
                                        return eventDay;
                                }
                            }
                        }
                    }

                    return null;
                }
            }
        }


        public void InitFileEvent()
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM FileEvent WHERE UserID = @userID", connection))
                {
                    command.Parameters.AddWithValue("userID", App.User.id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        string ID_event, ID_file;

                        while (reader.Read())
                        {
                            ID_event = reader["IDevent"].ToString();
                            ID_file = reader["IDfile"].ToString();

                            EventDay e = GetEventWithID(ID_event);
                            Fichier f = DataBaseFile._dbFile.GetFileWithID(ID_file);

                            if (e.Titre != null && f.Nom != null)
                                e.AttachFile(f);
                        }
                    }
                }
            }
        }

        public void ModifEvent(EventDay nvx)
        {
            using(var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("UPDATE Evenement Set GroupID = @groupID, Classe = @classe, Titre = @titre, DateDebut = @debut, DateFin = @fin, Description = @description, Localisation = @loc " +
                    "WHERE ID = @id and UserID = @userID", connection))
                {
                    command.Parameters.AddWithValue("userID", App.ID);
                    command.Parameters.AddWithValue("id", nvx.ID);
                    command.Parameters.AddWithValue("groupID", nvx.GroupID);
                    command.Parameters.AddWithValue("titre", nvx.Titre);
                    command.Parameters.AddWithValue("classe", nvx.Classe);
                    command.Parameters.AddWithValue("debut", nvx.DateDebut.ToString());
                    command.Parameters.AddWithValue("fin", nvx.DateFin.ToString());
                    command.Parameters.AddWithValue("description", nvx.Description);
                    command.Parameters.AddWithValue("loc", nvx.Lieu);
                    command.ExecuteNonQuery();
                }
            }
        }




        public void DeleteEventToDB(EventDay EventRemove)
        {
            using(var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using(SQLiteCommand command = new SQLiteCommand("DELETE FROM FileEvent WHERE IDevent=@id and UserID=@userID", connection))
                {
                    command.Parameters.AddWithValue("@userID", App.User.id);
                    command.Parameters.AddWithValue("@id", EventRemove.ID);
                    command.ExecuteNonQuery();

                    command.CommandText = "DELETE FROM Evenement WHERE ID = @id and UserID = @userID";
                    command.ExecuteNonQuery();
                }
            }
        }


        public bool AddEventToDB(EventDay EventAdd)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("INSERT INTO " +
                    "Evenement (UserID, ID, GroupID, Titre, Classe, Description, Localisation, DateDebut, DateFin) " +
                    "VALUES (@userID, @id, @groupID, @titre, @classe, @description, @lieu, @debut, @fin)", connection))
                {
                    command.Parameters.AddWithValue("@userID", App.User.id);
                    command.Parameters.AddWithValue("@id", EventAdd.ID);
                    command.Parameters.AddWithValue("@groupID", EventAdd.GroupID);
                    command.Parameters.AddWithValue("@titre", EventAdd.Titre);
                    command.Parameters.AddWithValue("@classe", EventAdd.Classe);
                    command.Parameters.AddWithValue("@description", EventAdd.Description);
                    command.Parameters.AddWithValue("@lieu", EventAdd.Lieu);
                    command.Parameters.AddWithValue("@debut", EventAdd.DateDebut.ToString("s"));
                    command.Parameters.AddWithValue("@fin", EventAdd.DateFin.ToString("s"));
                    command.ExecuteNonQuery();
                }
            }

            return true;
        }

        public void DeleteGroupEvent(EventDay EventRemove, DateTime fromThisDate = default, bool ExceptSender = false)
        {
            using (var connection = new SQLiteConnection(App.SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand commandFileEvent = new SQLiteCommand(connection))
                {
                    if (ExceptSender)
                    {
                        MessageBox.Show(fromThisDate.ToString());
                        commandFileEvent.CommandText = "DELETE FROM FileEvent WHERE IDevent in (SELECT ID FROM Evenement WHERE UserID=@userID and GroupID=@groupID and DateDebut > @date and ID != @id)";
                        commandFileEvent.Parameters.AddWithValue("@id", EventRemove.ID);
                    }

                    else
                        commandFileEvent.CommandText = "DELETE FROM FileEvent WHERE IDevent in (SELECT ID FROM Evenement WHERE UserID=@userID and GroupID=@groupID and DateDebut > @date";

                    commandFileEvent.Parameters.AddWithValue("@userID", App.User.id);
                    commandFileEvent.Parameters.AddWithValue("@groupID", EventRemove.GroupID);
                    commandFileEvent.Parameters.AddWithValue("@date", fromThisDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    commandFileEvent.ExecuteNonQuery();


                    if (ExceptSender)
                        commandFileEvent.CommandText = "DELETE FROM Evenement WHERE UserID=@userID and GroupID=@groupID and DateDebut > @date and ID != @id";

                    else commandFileEvent.CommandText = "DELETE FROM Evenement WHERE UserID=@userID and GroupID=@groupID and DateDebut > @date";


                    commandFileEvent.ExecuteNonQuery();
                }
            }
        }
    }
}
