using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace DotAgenda
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int ID;
        public static Profil User;
        public static DateTime LastDate = new DateTime(2026, 12, 31);
        public static DateTime StartDate = new DateTime(2022, 01, 01);

        public static List<string[]> ListeDefaultClasse { get; } = new List<string[]>
        {
            new string[] { "Important", "#ff4f78", "AlertCircleOutline" },
            new string[] { "Loisir", "#4fa2ff", "RobotHappyOutline" },
            new string[] { "Cours", "#FFB93F", "BookOutline" },
            new string[] { "Divers", "#3fff92", "ArchiveOutline" }
        };

        public static string SystemDB_Path { get; } = "DataSource =SystemDB.db";

        GestionnaireEvent _global;
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            if (!RememberConnection())
            {
                var dialog = new LoginForm();

                if (dialog.ShowDialog() == true)
                    StartTheApp();
                else
                {
                    MessageBox.Show("Problème de connexion", "Error", MessageBoxButton.OK);
                    Current.Shutdown(-1);
                }
            }

            else
                StartTheApp();
        }

        private void StartTheApp()
        {
            InitGlobals();

            DataBase._db.UpdateConnectionDB();
            Primitives._prim.StartNotificationApp();

            var mainWindow = new MainWindow();
            //Re-enable normal shutdown mode.
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = mainWindow;
            mainWindow.Show();
        }

        private bool RememberConnection()
        {
            using (SQLiteConnection connection = new SQLiteConnection(SystemDB_Path))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("select UserID From UserConnection where Machine = @machine and userID != -1", connection))
                {
                    command.Parameters.AddWithValue("machine", Environment.MachineName);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ID = reader.GetInt32(0);
                            return true;
                        }

                        return false;
                    }
                }
            }
        }

        private void InitGlobals()
        {
            GlobalDict._dict.InitAll();
            DataBase._db.InitProfil();
            DataBase._db.InitClasse();
            GestionnaireEvent._global.Init();
        }

        public static void SwitchTheme(bool light)
        {
            DataBase._db.ChangeLightDB(light);

            if (light == false)
            {
                Current.Resources["bg"] = Current.Resources["bg_B"];
                Current.Resources["Secondary"] = Current.Resources["Secondary_B"];
                Current.Resources["Tertiary"] = Current.Resources["Tertiary_B"];
                Current.Resources["Font"] = Current.Resources["Font_B"];
                Current.Resources["Font_Search"] = Current.Resources["Font_B"];
                Current.Resources["PopupBG"] = Current.Resources["PopupBG_B"];
                Current.Resources["FieldColor"] = Current.Resources["FieldColor_B"];
            }

            else
            {
                Current.Resources["bg"] = Current.Resources["bg_W"];
                Current.Resources["Secondary"] = Current.Resources["Secondary_W"];
                Current.Resources["Tertiary"] = Current.Resources["Tertiary_W"];
                Current.Resources["Font"] = Current.Resources["Font_W"];
                Current.Resources["Font_Search"] = Current.Resources["Font_Grey"];
                Current.Resources["PopupBG"] = Current.Resources["PopupBG_W"];
                Current.Resources["FieldColor"] = Current.Resources["FieldColor_W"];
            }
        }
        /*
        protected void OnClosing(CancelEventArgs e)
        {
            if (!User.RememberMe)
                DataBase._db.Deconnect();

            Primitives._prim.KillNotificationApp();
        }*/
    }
}
