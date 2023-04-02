using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using DotAgenda.View.Popups;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAgenda.MethodClass
{
    public class Primitives
    {
        private static readonly Primitives prim = new Primitives();

        public static Primitives _prim => prim;


        public GlobalDict _dict;
        public GestionnaireEvent _global;
        public DataBase _db;

        private Primitives()
        {

        }

        public EventDay SearchNextEvent(int delai, DateTime Date)
        {
            //Si y'a plus d'évènement ajd on va a demain

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
                _db.File.DeleteFileToDB_Event(fic, evenement);
            }

            else
            {
                Console.WriteLine("Erreur : fichier " + fic + " pas détaché de " + evenement);
            }
        }


        public Fichier CreateFile(string nomFichier)
        {
            int fileID = _db.File.GetFileID(nomFichier);

            Fichier fichier = new Fichier();

            fichier.ID = fileID;

            if (fileID == -1)
            {
                _db.File.AddFileToDB(nomFichier);
                fileID = _db.File.GetFileID(nomFichier);

                fichier.Nom = nomFichier;
                fichier.Contenu = "null";
                fichier.DateAjout = DateTime.Today;

                FileInfo fi = new FileInfo(fichier.Nom);

                try
                {
                    fichier.Type = _dict.ExtensionDict[fi.Extension];
                }

                catch
                {
                    fichier.Type = _dict.ExtensionDict["null"];
                }

                fichier.ID = fileID;

                fichier.AddToFolderType();
                _global.ListeFichiers.Add(fichier);

                return fichier;
            }

            else
            {
                fichier.Type = null;
                return fichier;
            }
        }


        public void AddFileToEvent(string nomFichier, EventDay evenement)
        {
            Fichier fic = CreateFile(nomFichier);

            if (fic.Type == null)
            {
                fic = _db.File.GetFileWithID(fic.ID);
            }

            if (evenement.AddFile(fic))
                _db.File.AddFileToDB_Event(fic, evenement);

            else
            {
                var event_window = new AlreadyImportedFile(true);
                event_window.Show();
            }
        }


        public void DeleteFile(Fichier fic)
        {
            _global.ListeFichiers.Remove(fic);
            fic.DetachAll();
            fic.RemoveFromDossierType();

            _db.File.DeleteFileToDB(fic);
        }

        public bool Event_Fini(DateTime Date)
        {
            if (Date <= DateTime.Now) return true;
            else return false;
        }

        public int FindTodoIndex(TodoItem Todo)
        {
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



        public void ModifEvent(EventDay nvx, EventDay old, Annee[] A)
        {
            int numY = nvx.DateDebut.Year - DateTime.Today.Year + 1;
            int mois = nvx.DateDebut.Month - 1;
            int jour = nvx.DateDebut.Day - 1;

            int numY_old = old.DateDebut.Year - DateTime.Today.Year + 1;
            int mois_old = old.DateDebut.Month - 1;
            int jour_old = old.DateDebut.Day - 1;


            A[numY].M[mois].J[jour].AddEventToList(nvx);
            A[numY_old].M[mois_old].J[jour_old].DeleteEventToList(old);

            _db.Event.ModifEvent(nvx);
        }


        public ObservableCollection<Dossier> InitFolderType()
        {
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
                return _dict.DictClasse[NomClasse];
            }

            catch { return null; }
        }

        public void ChangeEtatTache(TodoItem Tache)
        {
            int numY = Tache.DateDebut.Year - DateTime.Today.Year + 1;

            int new_index = _prim.FindTodoIndex(Tache);

            _global.A[numY].M[Tache.DateDebut.Month - 1].J[Tache.DateDebut.Day - 1].Todo.Remove(Tache);
            _global.A[numY].M[Tache.DateDebut.Month - 1].J[Tache.DateDebut.Day - 1].Todo.Insert(new_index, Tache);


            _prim.SetCurrentDate();
        }

    }
}
