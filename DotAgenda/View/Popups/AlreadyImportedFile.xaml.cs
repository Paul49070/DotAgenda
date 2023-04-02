using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DotAgenda.View.Popups
{
    /// <summary>
    /// Logique d'interaction pour AlreadyImportedFile.xaml
    /// </summary>
    public partial class AlreadyImportedFile : Window
    {

        public AlreadyImportedFile(bool AlreadyAttached = false)
        {
            InitializeComponent();

            if (AlreadyAttached)
            {
                CoreTXT.Text = "Fichier déjà attaché à cet évènement.";
            }

            else CoreTXT.Text = "Fichier déjà importé.";
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 20;
            this.Top = desktopWorkingArea.Bottom - this.Height - 20;


            CloseWindow();
        }

        private bool closeStoryBoardCompleted = false;

        private void CloseWindow()
        {

            // Create a new DispatcherTimer
            DispatcherTimer timer = new DispatcherTimer();

            // Set the interval to 3 seconds
            timer.Interval = TimeSpan.FromSeconds(3);

            // Set the event handler for when the timer elapses
            timer.Tick += (sender, e) =>
            {
                // Close the window
                //Window.GetWindow(this).Close();
                StartFadeOut();
                // Stop the timer
                timer.Stop();
            };

            // Start the timer
            timer.Start();


            
        }

        private void StartFadeOut()
        {
            if (!closeStoryBoardCompleted)
            {
                var closeStoryBoard = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
                closeStoryBoard.Completed += new EventHandler(closeStoryBoard_Completed);
                this.BeginAnimation(UIElement.OpacityProperty, closeStoryBoard);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            
        }

        private void closeStoryBoard_Completed(object sender, EventArgs e)
        {
            closeStoryBoardCompleted = true;
            this.Close();
        }
    }
}
