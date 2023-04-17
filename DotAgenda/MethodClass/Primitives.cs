using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using DotAgenda.View.Popups;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using DevExpress.Data.TreeList;
using static DotAgenda.Models.EventDay;
using System.Web.UI.Design;
using System.Windows;
using Microsoft.SqlServer.Management.XEvent;
using System.Security.Cryptography;
using System.Diagnostics;

namespace DotAgenda.MethodClass
{
    public class Primitives
    {
        private static readonly Primitives prim = new Primitives();

        public static Primitives _prim => prim;

        private Primitives()
        {

        }

        public bool IsNotificationAppAlreadyRunning()
        {
            Process[] instances = Process.GetProcessesByName("NotificationSystemDotAgenda");
            return instances.Length > 0;
        }

        public void KillNotificationApp()
        {
            if(IsNotificationAppAlreadyRunning())
            {
                Process[] actualInstance = Process.GetProcessesByName("NotificationSystemDotAgenda");
                
                foreach(Process p in actualInstance)
                {
                    p.Kill();
                }
            }
        }


        

        public void StartNotificationApp()
        {
            if (!IsNotificationAppAlreadyRunning())
            {
                string NotificationSystemPath = "C:\\Users\\paull\\OneDrive\\Bureau\\DotAgenda\\NotificationSystemDotAgenda\\NotificationSystemDotAgenda\\bin\\Debug\\net6.0-windows10.0.17763.0\\NotificationSystemDotAgenda.exe";
                string args = App.ID.ToString();
                ProcessStartInfo processStartInfo = new ProcessStartInfo(NotificationSystemPath, args);

                try
                {
                    Process.Start(processStartInfo);
                }

                catch (Exception e)
                {
                    MessageBox.Show("Impossible de lancer l'app de notif : " + e.Message);
                }
            }
        }

        public static string GenerateRandomString(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string CryptPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedPassword);
            }
        }

        public string DecryptPassword(string cryptedPassword)
        {
            MessageBox.Show(cryptedPassword);

            byte[] encryptedPassword = Encoding.UTF8.GetBytes(cryptedPassword);

            byte[] decryptedPassword = ProtectedData.Unprotect(
                   encryptedPassword,
                   null,
                   DataProtectionScope.CurrentUser);

            string stringDecryptPassword = Convert.ToBase64String(decryptedPassword);

            MessageBox.Show(stringDecryptPassword);

            return stringDecryptPassword;
        }

        public string GenerateID()
        {
            Guid myuuid = Guid.NewGuid();

            return myuuid.ToString();
        }

        public EventDay SearchNextEvent(int delai, DateTime Date)
        {
            //Si y'a plus d'évènement ajd on va a demain

            GestionnaireEvent _global = GestionnaireEvent._global;

            for (int i = 0; i < delai; ++i)
            {
                int numY = Date.Year - DateTime.Today.Year + 1;
                int numM = Date.Month - 1, numJ = Date.Day - 1;

                if (_global.A[numY].M[numM].J[numJ].ListeEvent.Count != 0)
                {

                    for (int j = 0; j < _global.A[numY].M[numM].J[numJ].ListeEvent.Count; ++j)
                    {
                        if (_global.A[numY].M[numM].J[numJ].ListeEvent[j].DateDebut > DateTime.Now)
                        {
                            return _global.A[numY].M[numM].J[numJ].ListeEvent[j];
                        }
                    }
                }

                Date = Date.AddDays(1);
            }

            return null;
        }

        public int Tri(DateTime Date)
        {
            GestionnaireEvent _global = GestionnaireEvent._global;

            int numM = Date.Month - 1;
            int numJ = Date.Day - 1;
            int numY = Date.Year - DateTime.Today.Year + 1;
            int index = 0;

            if (_global.A[numY].M[numM].J[numJ].ListeEvent.Count() == 0)      //Si il n'y a pas d'élément dans la liste d'évènement on l'insère en premier
            {
                return 0;
            }

            else
            {
                for (int i = 0; i < _global.A[numY].M[numM].J[numJ].ListeEvent.Count(); ++i)
                {
                    if (Date > _global.A[numY].M[numM].J[numJ].ListeEvent[i].DateDebut)        //Si l'heure que l'on a fixée est plus petite que l'heure du prochain event
                        index = i + 1;

                    else return index;
                }
            }
            return index;
        }

        public void DetachFileToEvent(Fichier fic, EventDay evenement)
        {
            if (evenement.RemoveFile(fic))
            {
                DataBaseFile._dbFile.DeleteFileToDB_Event(fic, evenement);
            }

            else
            {
                Console.WriteLine("Erreur : fichier " + fic + " pas détaché de " + evenement);
            }
        }


        public Fichier CreateFile(string nomFichier)
        {
            Fichier ficAdded = new Fichier(nomFichier);

            if (DataBaseFile._dbFile.AddFileToDB(ficAdded))
            {
            }

            return ficAdded;
        }

        
        public void AddFileToEvent(string nomFichier, EventDay evenement)
        {
            Fichier fic;

            if (GlobalDict._dict.DictFile.Keys.ToList().IndexOf(nomFichier) == -1)
            {
                fic = new Fichier(nomFichier);
                DataBaseFile._dbFile.AddFileToDB(fic);
            }

            else
                fic = GlobalDict._dict.DictFile[nomFichier];

            if (evenement.AttachFile(fic))
                DataBaseFile._dbFile.AddFileToDB_Event(fic, evenement);

            else new AlreadyImportedFile(AlreadyAttached: true).Show();
        }


        public void DeleteFile(Fichier fic)
        {
            GestionnaireEvent._global.ListeFichiers.Remove(fic);
            fic.DetachAll();
            fic.RemoveFromDossierType();

            DataBaseFile._dbFile.DeleteFileToDB(fic);
        }

        public bool Event_Fini(DateTime Date)
        {
            if (Date <= DateTime.Now) return true;
            else return false;
        }

        public int FindTodoIndex(TodoItem Todo)
        {
            GestionnaireEvent _global = GestionnaireEvent._global;

            int numY = Todo.DateDebut.Year - DateTime.Today.Year + 1;
            int numM = Todo.DateDebut.Month - 1;
            int numJ = Todo.DateDebut.Day - 1;

            for (int i = 0; i < _global.A[numY].M[numM].J[numJ].Todo.Count; ++i)
            {
                if (_global.A[numY].M[numM].J[numJ].Todo[i].Fini && _global.A[numY].M[numM].J[numJ].Todo[i] != Todo)
                {
                    if (i == 0)
                        return 0;

                    return i - 1;
                }
            }

            if (_global.A[numY].M[numM].J[numJ].Todo.Count == 0)
                return 0;

            else return _global.A[numY].M[numM].J[numJ].Todo.Count - 1;
        }

        public bool DeleteGroupEvent(EventDay EventToRemove, string GroupID, DateTime start = default, DateTime end = default)
        {
            DataBaseEvents._dbEvent.DeleteGroupEvent(EventToRemove, start, ExceptSender: true);

            GroupEvent grpEvent;

            try
            {
                grpEvent = GlobalDict._dict.DictGroupEvent[GroupID];
            }

            catch { return false; }

            List<EventDay> EventDuGroupe = grpEvent.GroupEventListe.OrderBy(e => e.DateDebut).ToList();

            foreach(EventDay e in EventDuGroupe)
            {
                if(e.DateDebut >= end && e.DateFin <= start)
                {
                    e.RemoveEvent();
                }
            }

            return true;
        }

        public void ModifEvent(EventDay nvx, EventDay old)
        {
            if (nvx.Reccurence != old.Reccurence)
            {

                if (nvx.Reccurence.Repeat == RepeatType.None)
                {
                    //Il y avait des reccurences, on n'en veut plus
                    DeleteGroupEvent(nvx, nvx.GroupID, start:old.DateDebut);
                }
                //Si on ajoute une réccurence, alors on doit créer un groupID

                else
                {

                    nvx = nvx.Clone(GenerateNewGroupID: true);
                    AddEvent(nvx, true);
                }
            }

            int numY_old = old.DateDebut.Year - DateTime.Today.Year + 1;
            int mois_old = old.DateDebut.Month - 1;
            int jour_old = old.DateDebut.Day - 1;

            int numY = nvx.DateDebut.Year - DateTime.Today.Year + 1;
            int mois = nvx.DateDebut.Month - 1;
            int jour = nvx.DateDebut.Day - 1;

            GestionnaireEvent._global.A[numY_old].M[mois_old].J[jour_old].DeleteEventToList(old);
            GestionnaireEvent._global.A[numY].M[mois].J[jour].AddEventToList(nvx);

            DataBaseEvents._dbEvent.ModifEvent(nvx);
        }
        


        public void AddEvent(EventDay EventToAdd, bool ExceptSender = false)
        {
            DateTime EndDate = App.LastDate;

            DateTime start = EventToAdd.DateDebut, end = EventToAdd.DateFin;            
            
            if(!ExceptSender)
                GestionnaireEvent._global.A[start.Year - DateTime.Today.Year + 1].M[start.Month - 1].J[start.Day - 1].AjouterEvent(EventToAdd);

            switch (EventToAdd.Reccurence.Repeat)
            {
                case RepeatType.None:
                    break;

                case RepeatType.Daily:


                    if (EventToAdd.Reccurence.ForXtime == -1)    //Infini
                    {
                        while (end<=EndDate)
                        {
                            AddWithDayFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    else
                    {
                        for(int i = 0; i<EventToAdd.Reccurence.ForXtime; ++i)
                        {
                            AddWithDayFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    break;

                case RepeatType.Weekly:


                    if (EventToAdd.Reccurence.ForXtime == -1)    //Infini
                    {
                        while (end <= EndDate)
                        {
                            AddWithWeekFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    else
                    {
                        for (int i = 0; i < EventToAdd.Reccurence.ForXtime; ++i)
                        {
                            AddWithWeekFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    break;

                case RepeatType.Monthly:

                    if (EventToAdd.Reccurence.ForXtime == -1)    //Infini
                    {
                        while (end <= EndDate)
                        {
                            AddWithMonthFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    else
                    {
                        for (int i = 0; i < EventToAdd.Reccurence.ForXtime; ++i)
                        { 
                            AddWithMonthFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    break;

                case RepeatType.Yearly:

                    if (EventToAdd.Reccurence.ForXtime == -1)    //Infini
                    {
                        while (end <= EndDate)
                        {
                            AddWithYearFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    else
                    {
                        for (int i = 0; i < EventToAdd.Reccurence.ForXtime; ++i)
                        {
                            AddWithYearFrequency(EventToAdd, ref start, ref end);
                        }
                    }

                    break;
            }
        }

        private void AddWithDayFrequency(EventDay EventInitial, ref DateTime start, ref DateTime end)
        {

            start = start.AddDays(EventInitial.Reccurence.EveryXtime);    // => tout les x jours
            end = end.AddDays(EventInitial.Reccurence.EveryXtime);

            CreateAndAddevent(EventInitial, start, end);
        }

        private void AddWithWeekFrequency(EventDay EventInitial, ref DateTime start, ref DateTime end)
        {
            start = start.AddDays(EventInitial.Reccurence.EveryXtime * 7);
            end = end.AddDays(EventInitial.Reccurence.EveryXtime * 7);

            CreateAndAddevent(EventInitial, start, end);
        }

        private void AddWithMonthFrequency(EventDay EventInitial, ref DateTime start, ref DateTime end)
        {

            start = start.AddMonths(EventInitial.Reccurence.EveryXtime);    // => tout les x jours
            end = end.AddMonths(EventInitial.Reccurence.EveryXtime);

            CreateAndAddevent(EventInitial, start, end);
        }

        private void AddWithYearFrequency(EventDay EventInitial, ref DateTime start, ref DateTime end)
        {
            start = start.AddYears(EventInitial.Reccurence.EveryXtime);    // => tout les x jours
            end = end.AddYears(EventInitial.Reccurence.EveryXtime);

            CreateAndAddevent(EventInitial, start, end);
        }

        private void CreateAndAddevent(EventDay EventInitial, DateTime start, DateTime end)
        {
            if (end <= App.LastDate)
            {
                EventDay newEvent = new EventDay(EventInitial.Titre, start, end, EventInitial.Classe, GroupID: EventInitial.GroupID);

                GestionnaireEvent._global.A[start.Year - DateTime.Today.Year + 1].M[start.Month - 1].J[start.Day - 1].AjouterEvent(newEvent);
            }
        }


        public ObservableCollection<Dossier> InitFolderType()
        {
            GlobalDict _dict = GlobalDict._dict;

            ObservableCollection<Dossier> ListeDossiersType = new ObservableCollection<Dossier>();

            for (int i = 0; i < _dict.DossierTypeIconDict.Count(); ++i)
            {
                ListeDossiersType.Add(new Dossier
                {
                    Titre = _dict.DossierTypeIconDict.Values.ElementAt(i)[0],
                    Icon = _dict.DossierTypeIconDict.Values.ElementAt(i)[1],
                    Couleur = _dict.DossierTypeIconDict.Values.ElementAt(i)[2],
                    Type = true,
                });
            }

            return ListeDossiersType;
        }

        public void SetCurrentDate()
        {
            GestionnaireEvent _global = GestionnaireEvent._global;

            int numY = _global._currentDay.Date.Year - DateTime.Today.Year + 1;
            int numM = _global._currentDay.Date.Month - 1;
            int numJ = _global._currentDay.Date.Day - 1;

            _global._currentDay.ListeEvent = _global.A[numY].M[numM].J[numJ].ListeEvent;
            _global._currentDay.Todo = _global.A[numY].M[numM].J[numJ].Todo;
        }

        public ClasseEvent_item FindClasse(string NomClasse)
        {
            try
            {
                return GlobalDict._dict.DictClasse[NomClasse];
            }

            catch { return null; }
        }

        public void ChangeEtatTache(TodoItem Tache)
        {
            GestionnaireEvent _global = GestionnaireEvent._global;

            int numY = Tache.DateDebut.Year - DateTime.Today.Year + 1;

            int new_index = _prim.FindTodoIndex(Tache);

            _global.A[numY].M[Tache.DateDebut.Month - 1].J[Tache.DateDebut.Day - 1].Todo.Remove(Tache);
            _global.A[numY].M[Tache.DateDebut.Month - 1].J[Tache.DateDebut.Day - 1].Todo.Insert(new_index, Tache);


            _prim.SetCurrentDate();
        }

        public int BoolToInt(bool val)
        {
            if (val == true) return 1;
            else return 0;
        }

        public bool IntToBool(int val)
        {
            if (val == 1) return true;
            else return false;
        }
    }
}
