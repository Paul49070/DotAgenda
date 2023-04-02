using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public static int ID = -1;
        public static Profil User;
        
        public static string SystemDB_Path = "DataSource =SystemDB.db";
        public static string DB_Path = "";

        public static string NameDB_System  {get;} = "SystemDB.db";

        GestionnaireEvent _global;
        private void ApplicationStart(object sender, StartupEventArgs e)
        {


            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var dialog = new LoginForm();

            if (dialog.ShowDialog() == true)
            {
                InitGlobals();

                var mainWindow = new MainWindow();
                //Re-enable normal shutdown mode.
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Unable to load data.", "Error", MessageBoxButton.OK);
                Current.Shutdown(-1);
            }
        }

        private void InitGlobals()
        {

            _global = GestionnaireEvent._global;
            _global._currentDay.Date = DateTime.Today;

            _global._db = DataBase._db;
            _global._db._global = _global;
            _global._db._dict = GlobalDict._dict;
            _global._db._prim = Primitives._prim;
            _global._db.InitProfil();


            _global._db.File = DataBaseFile._dbFile;
            _global._db.File._global = _global;
            _global._db.File._dict = GlobalDict._dict;
            _global._db.File._prim = Primitives._prim;
            _global._db.File._db = DataBase._db;

            _global._db.Event = DataBaseEvents._dbEvent;
            _global._db.Event._global = _global;
            _global._db.Event._dict = GlobalDict._dict;
            _global._db.Event._prim = Primitives._prim;
            _global._db.Event._db = DataBase._db;

            _global._db.Todo = DataBaseTodo._dbTodo;
            _global._db.Todo._global = _global;
            _global._db.Todo._dict = GlobalDict._dict;
            _global._db.Todo._prim = Primitives._prim;
            _global._db.Todo._db = DataBase._db;


            GlobalDict._dict.InitAll(_global._db.InitClasse());
            _global._dict = GlobalDict._dict;
            _global._dict._db = _global._db;

            _global._prim = Primitives._prim;
            _global._prim._global = _global;
            _global._prim._dict = _global._dict;
            _global._prim._db = _global._db;


            _global.ListeDossiersType = _global.InitFolderType();
            _global.ListeFichiers = _global.InitListeFichiers();
            _global.A = _global.InitArray();
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
            }

            else
            {
                Current.Resources["bg"] = Current.Resources["bg_W"];
                Current.Resources["Secondary"] = Current.Resources["Secondary_W"];
                Current.Resources["Tertiary"] = Current.Resources["Tertiary_W"];
                Current.Resources["Font"] = Current.Resources["Font_W"];
                Current.Resources["Font_Search"] = Current.Resources["Font_Grey"];
                Current.Resources["PopupBG"] = Current.Resources["PopupBG_W"];
            }
        }
    }
}
