using replacer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace replacer.View
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window, INotifyPropertyChanged
    {
        public static ProgressWindow Instance;
        public EventHandler<ProgressEventArgs> ProgressEventHandler;  

        public ProgressWindow()
        {

            Instance = this;
            Trace.TraceInformation("ProgressWindow constructor");
            InitializeComponent();
            //PropertyChanged += ProgressWindow_PropertyChanged;
            
            //ProgressEventHandler += SetStatus;
        }

        private void ProgressWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DocumentModel.Instance.SetText(ProgressInformationModel.Instance.InfoText);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        //private void SetStatus(object sender, ProgressEventArgs e)
        //{
        //    lbInfo.Content= e.Status;
        //}
    }
   
}
