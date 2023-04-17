using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Forms;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;
using Window = System.Windows.Window;
using DotAgenda.Models;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using DotAgenda.View.Popups;
using Button = System.Windows.Controls.Button;
using System.Linq;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Windows.Markup;
using static DotAgenda.Models.EventDay;
using System.Text.RegularExpressions;
using DotAgenda.MethodClass;
using DotAgenda.MethodClass.DataBaseMethods;
using DotAgenda.ViewModels;

namespace DotAgenda.View
{
    /// <summary>
    /// Logique d'interaction pour MoreEventPage.xaml
    /// </summary>
    public partial class MoreEventPageUsual : System.Windows.Controls.UserControl
    {
        public MoreEventPageUsual()
        {
            InitializeComponent();
        }

        private void DropFile(object sender, System.Windows.DragEventArgs e)
        {
            var vm = (MoreEventPageUsual_ViewModel)DataContext;

            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
                vm.DropFile.Execute(files);
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - e.Delta / 3);
        }
    }
}
