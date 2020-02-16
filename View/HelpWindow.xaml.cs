using Microsoft.Office.Interop.Word;
using replacer.View;
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
using Application = Microsoft.Office.Interop.Word.Application;
using Word = Microsoft.Office.Interop.Word;

namespace replacer.View
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : System.Windows.Window
    {
        private Object refmissing = System.Reflection.Missing.Value;
        public static HelpWindow Instance;

        public HelpWindow()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            InitializeComponent();

            this.Loaded += HelpWindow_Loaded;
            this.Closed += HelpWindow_Closed;
            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownFinished;
        }

        private void Dispatcher_ShutdownFinished(object sender, EventArgs e)
        {
            SpecialSettingsWindow.Instance.Activate();
            SpecialSettingsWindow.Instance.BringIntoView();
        }

        private void HelpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitHelpDocument();
        }

        private void HelpWindow_Closed(object sender, EventArgs e)
        {
            ShowSpecialSettingWindow();
           
        }

        private void InitHelpDocument()
        {
            String strFileName = @"HelpDocument.html";

            documentViewerWebBrowser.Navigate(@"C:\Replacer Projekt\"+strFileName);// = new Uri("file://"+strFileName);
            documentViewerWebBrowser.Focus();
            documentViewerWebBrowser.UseLayoutRounding = true;
            HelpWindow.Instance.Topmost = true;

        }

     

        private void ShowSpecialSettingWindow()
        {
            HelpWindow.Instance.Close();

            SpecialSettingsWindow.Instance.WindowState = WindowState.Normal;
            SpecialSettingsWindow.Instance.Activate();
        }

     
    }
}
