using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace replacer
{
    /// <summary>
    /// Interaction logic for SetDefaultsWindow.xaml
    /// </summary>
    public partial class SetDefaultsWindow : Window
    {
        #region Variables
        private Visibility visibility;
        private System.Timers.Timer t = new System.Timers.Timer();
        public FolderBrowserDialog documentsPathFolderBrowserDialog = new FolderBrowserDialog();
        public OpenFileDialog fileDialog = new OpenFileDialog();
        public static PropertyInfo propert;
        public static SetDefaultsWindow Instance;
        #endregion


        #region Properties
        private string defaultPathToLookForDocuments;

        public string DefaultPathToLookForDocuments
        {
            get { return defaultPathToLookForDocuments; }
            set { defaultPathToLookForDocuments = value; }
        }

        private string defaultPathToLookForInformationFiles;

        public string DefaultPathToLookForInformationFiles
        {
            get { return defaultPathToLookForInformationFiles; }
            set { defaultPathToLookForInformationFiles = value; }
        }

        private string testObjectFileName;

        public string TestObjectFileName
        {
            get { return testObjectFileName; }
            set { testObjectFileName = value; }
        }

        private string testEnvironmentFileName;


        public string TestEnvironmentFileName
        {
            get { return testEnvironmentFileName; }
            set { testEnvironmentFileName = value; }
        }
        #endregion

        public SetDefaultsWindow()
        {
            InitializeComponent();

            SQLManager man = new SQLManager();
            Trace.TraceInformation("Creating instance of SQLManager");

            StartLabelVisibilityTimer();
            Instance = this;
            this.Loaded += SetDefaultsWindow_Loaded;

            Trace.TraceInformation("Start to fil textboxes from settings file");
            FillTextboxes(man);
        }

        public SetDefaultsWindow(bool showWindow)
        {
            if (showWindow)
            {
                InitializeComponent();
                StartLabelVisibilityTimer();

            }
            SQLManager man = new SQLManager();

            Instance = this;
            this.Loaded += SetDefaultsWindow_Loaded;

            if (showWindow)
            {
                FillTextboxes(man);
            }
        }

        #region Methods
        private void StartLabelVisibilityTimer()
        {
            System.Timers.Timer t = new System.Timers.Timer(1500);
            t.Elapsed += T_Elapsed;
            t.Start();
        }

        /// <summary>
        /// Update the database with the values of all textboxes Text property
        /// </summary>
        private void SaveDatabaseSettings()
        {
            Trace.TraceInformation("Saving settings");
            SQLManager man = new SQLManager();
            man.ReadTextFile();

            SQLManager.DefaultPathToDocuments = tbDefaultFolder.Text;
            SQLManager.DefaultPathToFiles = tbDefaultFolderTextFiles.Text;
            SQLManager.TestObjectFile = tbTestObjectFileName.Text;
            SQLManager.TestEnvironmentFile200 = tbTestEnvironmentFileName200.Text;
            SQLManager.TestEnvironmentFile1500 = tbTestEnvironmentFileName1500.Text;

           // SQLManager.Instance.Dispose();
        }

        /// <summary>
        /// Load all settings from the database then fill the textboxes on this Window
        /// </summary>
        private void LoadDatabaseSettings()
        {
            Trace.TraceInformation("Load settings");
            SQLManager man = new SQLManager();
            man.ReadTextFile();

            SQLManager.DefaultPathToLookForDocuments = Convert.ToString(SQLManager.DefaultPathToDocuments);
            SQLManager.DefaultPathToLookForInformationFiles = Convert.ToString(SQLManager.DefaultPathToFiles);
            SQLManager.TestObjectFile = Convert.ToString(SQLManager.TestObjectFile);
            SQLManager.TestEnvironmentFile200 = Convert.ToString(SQLManager.TestEnvironmentFile200);
            SQLManager.TestEnvironmentFile1500 = Convert.ToString(SQLManager.TestEnvironmentFile1500);

           // SQLManager.Instance.Dispose();
        }

        private void FillTextboxes(SQLManager man)
        {
            Trace.TraceInformation("Filling in textboxes");
            tbDefaultFolder.Text = Convert.ToString(SQLManager.DefaultPathToDocuments).Trim();
            tbDefaultFolderTextFiles.Text = Convert.ToString(SQLManager.DefaultPathToFiles).Trim();
            tbTestEnvironmentFileName1500.Text = Convert.ToString(SQLManager.TestEnvironmentFile1500).Trim();
            tbTestEnvironmentFileName200.Text = Convert.ToString(SQLManager.TestEnvironmentFile200).Trim();
            tbTestObjectFileName.Text = Convert.ToString(SQLManager.TestObjectFile).Trim();
        }

        private void SetLabelVisibility(Visibility visibility, string method)
        {
            lbResult.Content = "Default Values are " + method + " successfull!";
            lbResult.Visibility = visibility;
        }

        #endregion

        #region EventHandlers
        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            t.Stop();

            visibility = Visibility.Visible;

            if (lbResult.Dispatcher.CheckAccess())
            {
                lbResult.Content = "Default Values are loaded successfull!";
                lbResult.Visibility = visibility;
            }
            else
                lbResult.Dispatcher.Invoke(new SetLabelVisibilityDelegate(SetLabelVisibility), new object[] { visibility, "loaded" });
        }
        private void SetDefaultsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Load settings");
            LoadDatabaseSettings();


        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Cancel click");
            this.Close();
        }

        private void BtSave_Click_1(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Save click");
            CheckTextBoxValues();

            SaveDatabaseSettings();

            SQLManager man = new SQLManager();
            man.CreateSettingsFile();

            StartLabelVisibilityTimer();

            this.Close();
        }

        private void CheckTextBoxValues()
        {
            Trace.TraceInformation("Checking textboxes values");
            if (tbDefaultFolder.Text == string.Empty || tbDefaultFolderTextFiles.Text == string.Empty || tbTestEnvironmentFileName1500.Text == string.Empty || tbTestEnvironmentFileName200.Text == string.Empty || tbTestObjectFileName.Text == string.Empty)
            {
                System.Windows.Forms.MessageBox.Show("One or more field is empty!");
            }

            if (!File.Exists(string.Format(@"{0}\{1}", tbDefaultFolderTextFiles.Text, tbTestEnvironmentFileName1500.Text)))
            {
                System.Windows.Forms.MessageBox.Show(String.Format("File couldn't be found: {0} at folder: {1}", tbTestEnvironmentFileName1500.Text, tbDefaultFolderTextFiles.Text));
            }
            if (!File.Exists(string.Format(@"{0}\{1}", tbDefaultFolderTextFiles.Text, tbTestEnvironmentFileName200.Text)))
            {
                System.Windows.Forms.MessageBox.Show(String.Format("File couldn't be found: {0} at folder: {1}", tbTestEnvironmentFileName200.Text, tbDefaultFolderTextFiles.Text));
            }
            if (!File.Exists(string.Format(@"{0}\{1}", tbDefaultFolderTextFiles.Text, tbTestObjectFileName.Text)))
            {
                System.Windows.Forms.MessageBox.Show(String.Format("File couldn't be found: {0} at folder: {1}", tbTestObjectFileName.Text, tbDefaultFolderTextFiles.Text));
            }

            //if ((SQLManager.DocumentType == "KMP1500") && (tbDefaultFolder.Text.Contains("200")))
            //{
            //    System.Windows.Forms.MessageBox.Show("The configured Default Path To Documents Contains '200', is it correct?!");
            //}
            //else if ((SQLManager.DocumentType == "KMP200") && (tbDefaultFolder.Text.Contains("1500")))
            //{
            //    System.Windows.Forms.MessageBox.Show("The configured Default Path To Documents Contains '1500', is it correct?!");
            //}

        }
        #endregion

        private void BtFolderChoose_Click(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Documents folder folderchooser");
            DialogResult result = documentsPathFolderBrowserDialog.ShowDialog();

            tbDefaultFolder.Text = documentsPathFolderBrowserDialog.SelectedPath;

        }

        private void BtFolderChoose_Click2(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Textfiles folder folderchooser");
            DialogResult result = documentsPathFolderBrowserDialog.ShowDialog();

            tbDefaultFolderTextFiles.Text = documentsPathFolderBrowserDialog.SelectedPath;
        }

        private void BtFileChoose_Click(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("TestObject file chooser");
            DialogResult result = fileDialog.ShowDialog();
            tbTestObjectFileName.Text = fileDialog.FileName.Substring(fileDialog.FileName.LastIndexOf("\\") + 1);
        }

        private void BtFileChoose1500_Click(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("TestEnvironmentFileName1500 file chooser");
            DialogResult result = fileDialog.ShowDialog();
            tbTestEnvironmentFileName1500.Text = fileDialog.FileName.Substring(fileDialog.FileName.LastIndexOf("\\") + 1);
        }

        private void BtFileChoose200_Click(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("TestEnvironmentFileName200 file chooser");
            DialogResult result = fileDialog.ShowDialog();
            tbTestEnvironmentFileName200.Text = fileDialog.FileName.Substring(fileDialog.FileName.LastIndexOf("\\") + 1);
        }
    }

    delegate void SetLabelVisibilityDelegate(Visibility v, string methodString);


}

