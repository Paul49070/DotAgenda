using CommunityToolkit.Mvvm.Input;
using DotAgenda.MethodClass;
using DotAgenda.Models;
using DotAgenda.View.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DotAgenda.Core
{
    internal class FileCommands
    {
        public RelayCommand DetachFileCommand { get; set; }
        public RelayCommand GetFileCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public FileCommands()
        {
            OpenFileCommand = new RelayCommand(OpenFile);
            DetachFileCommand = new RelayCommand(Detach);
            GetFileCommand = new RelayCommand(GetFile);
        }

        public bool CorrectArgumentsEventAndFile(object obj)
        {
            if (obj is object[] values)
            {
                if (!(values[0] is Fichier))
                    return false;
                if (!(values[1] is EventDay))
                    return false;

                return true;
            }

            return false;
        }

        public bool CorrectArgumentsEventAndFileNames(object obj)
        {
            if (obj is object[] values)
            {
                if (!(values[0] is string[]))
                    return false;
                if (!(values[1] is EventDay))
                    return false;

                return true;
            }

            return false;
        }

        public bool IsEvent(object obj)
        {
            if (obj is EventDay) return true;
            else return false;
        }

        public bool IsFile(object obj)
        {
            if (obj is Fichier) return true;
            else return false;
        }

        public void GetFile(object obj)
        {
            if(IsEvent(obj))
            {
                EventDay evenement = (EventDay)obj;
                string CheminFichier;
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    CheminFichier = openFileDialog.FileName;
                    Primitives._prim.AddFileToEvent(CheminFichier, evenement);
                }
            }
            
        }

        public void Detach(object obj)
        {
            if(CorrectArgumentsEventAndFile(obj))
            {
                var values = (object[])obj;
                Fichier fic = (Fichier)values[0];
                EventDay evenement = (EventDay)values[1];

                var dialog = new AreYouSureDelete("Vous allez détâcher ce fichier et cet évènement", "Détacher");
                bool? result = dialog.ShowDialog();

                if (result == true)
                    Primitives._prim.DetachFileToEvent(fic, evenement);
            }
        }
        
        public void OpenFile(object obj)
        {
            if (IsFile(obj))
            {
                Fichier fic = (Fichier)obj;
                Process launchingFile = new Process();

                try
                {
                    launchingFile.StartInfo.FileName = fic.Nom;
                    launchingFile.Start();
                }

                catch
                {
                    ShowOpenWithDialog(fic.Nom);
                }
            }
        }
        public static void ShowOpenWithDialog(string path)
        {
            var args = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + path;
            Process.Start("rundll32.exe", args);
        }

    }
}
