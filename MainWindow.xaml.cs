using replacer.Controller;
using replacer.Helpers;
using replacer.Model;
using replacer.View;
using replacer.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace replacer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        public static MainWindow Instance = null;
        public ConfigurationWindow configWindow = null;
        public ProgressWindow pw = null;
        public Thread progressThread;
        #endregion

        #region Properties

        private bool showProgressOnProgressBar;

        public bool ShowProgressOnProgressBar
        {
            get { return showProgressOnProgressBar; }
            set { showProgressOnProgressBar = value; }
        }
        private string swVersion = "3.0";

        public string SwVersion
        {
            get { return swVersion; }
            set { swVersion = value; }
        }

        private string documentsPath;
        public string DocumentsPath
        {
            get
            {
                return documentsPath;
            }
            set
            {
                documentsPath = value;
            }
        }
        private string textFilesPath;
        public string TextFilesPath
        {
            get
            {
                return textFilesPath;
            }
            set
            {
                textFilesPath = value;
            }
        }

        private Dictionary<string, int> specialTestEnvironmentFiles = new Dictionary<string, int>();
        public Dictionary<string, int> SpecialTestEnvironmentFiles
        {
            get { return specialTestEnvironmentFiles; }
            set { specialTestEnvironmentFiles = value; }
        }


        public string ObjectTextFileName { get; private set; }
        public string EnvironmentTextFileName200 { get; private set; }
        public string EnvironmentTextFileName1500 { get; private set; }

        public Dictionary<KeyValuePair<string, string>, int> testEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();

        private Dictionary<string, string> settingsDict;

        public Dictionary<string, string> SettingsDict
        {
            get { return settingsDict; }
            set { settingsDict = value; }
        }

        public Dictionary<KeyValuePair<string, string>, int> TestEnvironmentFiles
        {
            get
            {
                return testEnvironmentFiles;
            }
            set
            {
                testEnvironmentFiles = value;
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            if (Instance == null)
            {
                Instance = this;
            }
            else
                throw new Exception("This class is singleton DO NOT create more!");

            DocumentModel dmModell = new DocumentModel();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.Closing += MainWindow_Closing;

            this.Loaded += MainWindow_Loaded;

            Trace.TraceInformation("MainWindow constructor");

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
            Environment.Exit(Environment.ExitCode);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Exception ex = (Exception)e.ExceptionObject;
            // unloading dragon medical one
            if (ex is TaskCanceledException)
                return;  // ignore
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TraceHelper.SetupListener();
            SwVersion versionControll = new SwVersion();
            this.Title = string.Format("{0}     Version: {1}", Title, versionControll.ActualSWVersion);
            Trace.TraceInformation("{0}     Version: {1}", Title, versionControll.ActualSWVersion);

        }
       
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            Environment.Exit(Environment.ExitCode);
        }
        private void BtReplaceDefault_Click(object sender, RoutedEventArgs e)
        {
            Trace.TraceInformation("Replace Process Started");
            StartProgressBarOnOtherThread(true);
            Trace.TraceInformation("Progress thread created");
            configWindow = new ConfigurationWindow(false);
            GetValuesFromConfigurationWindow();

            if (String.IsNullOrEmpty(documentsPath))
            {
                documentsPath = configWindow.DefaultPathToLookForDocuments;
            }
            if (String.IsNullOrEmpty(textFilesPath))
            {
                textFilesPath = configWindow.DefaultPathToLookForInformationFiles;
            }
            Trace.TraceInformation("MainProcess instance created with docPath: {0}; textsPath: {1}", documentsPath, textFilesPath);

            if (String.IsNullOrEmpty(documentsPath) || String.IsNullOrEmpty(textFilesPath))
                throw new Exception("TextFiles Path and Path of documents has not benn set successfull!");

            new MainProcess(documentsPath, textFilesPath);

            Trace.TraceInformation("MainProcess ended");
            StartProgressBarOnOtherThread(false);
            DocumentModel.Instance.SetText(string.Format("Replacement at {0} folder has been completed.", documentsPath));
            DocumentModel.Instance.KillAllWordProcess();
            CleanUp();
            progressThread.Abort();
        }

      
        public void CleanUp()
        {
            DocumentModel.Instance = null;
            DocumentModel.Instance =new DocumentModel();
            testEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();
            specialTestEnvironmentFiles = new Dictionary<string, int>();
            TestEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();
        }

        private void GetValuesFromConfigurationWindow()
        {
            foreach (Configuration item in Configurations.Collection)
            {
                if (item.SettingName.ToLower().Contains("path") && item.SettingName.ToLower().Contains("document"))
                {
                    DocumentsPath = item.SettingValue;
                }
                else if (item.SettingName.ToLower().Contains("file") && item.SettingName.ToLower().Contains("path"))
                {
                    TextFilesPath = item.SettingValue;
                }
                else if (item.SettingName.ToLower().Contains("object"))
                {
                    ObjectTextFileName = item.SettingValue;
                }

                string endNumber = string.Empty;
                var result = Regex.Match(item.SettingName.Trim(), @"\d+$", RegexOptions.RightToLeft);
                if (result.Success)
                {
                    Trace.TraceInformation("The settings name ending with number: {0}", result);
                    endNumber = Convert.ToString(result);
                    testEnvironmentFiles.Add(new KeyValuePair<string, string>(item.SettingName, item.SettingValue), Int32.Parse(endNumber));
                }
            }
            TestEnvironmentFiles = testEnvironmentFiles;
        }

        private void ReadOutEnvironmentFileNames(Dictionary<KeyValuePair<string, string>, int> testEnvironmentFiles)
        {
            TestEnvironmentFiles = testEnvironmentFiles;
            foreach (KeyValuePair<string, string> setting in testEnvironmentFiles.Keys)
            {
                switch (testEnvironmentFiles[setting])
                {
                    case 200:
                        EnvironmentTextFileName200 = setting.Value;
                        Trace.TraceInformation("Environment file for 200 is found: {0}", setting.Value);
                        break;
                    case 1500:
                        EnvironmentTextFileName1500 = setting.Value;
                        Trace.TraceInformation("Environment file for 1500 is found: {0}", setting.Value);
                        break;
                    default:
                        Trace.TraceWarning("Environment file number not expected, creating property for it: " +
                            "Name: {0} Value:{1} Number: {2}", setting.Key, setting.Value, testEnvironmentFiles[setting]);
                        specialTestEnvironmentFiles.Add(setting.Value, testEnvironmentFiles[setting]);
                        break;
                }
            }
        }


       

        private void StartProgressBarOnOtherThread(bool showProgress)
        {
            ShowProgressOnProgressBar = showProgress;
            progressThread = new Thread(new ThreadStart(ShowProgress));
            progressThread.SetApartmentState(ApartmentState.STA);
            progressThread.Start();
        }
     
       
        delegate void ShowProgressDelegate();
        private void ShowProgress()
        {
            new ProgressWindow().ShowDialog();
            //pw = new ProgressWindow();
            //HideMainWindowThreadSafe(false);
            //pw.ShowDialog();
            
            ////HideProcessWindowThreadSafe(true);
            //this.WindowState = WindowState.Normal;
        //     HideMainWindowThreadSafe(true);

        }
        delegate void HideMainWindowThreadSafeDelegate(bool show);
        public void HideMainWindowThreadSafe(bool show)
        {
            if (this.CheckAccess())
            {             
                    if (show)
                      this.Hide();
                    else
                        this.Show();
               
            }else
                this.Dispatcher.BeginInvoke(new HideMainWindowThreadSafeDelegate(HideMainWindowThreadSafe),show);
        }
        delegate void HideProcessWindowThreadSafeDelegate(bool show);
        public void HideProcessWindowThreadSafe(bool show)
        {
            if (ProgressWindow.Instance.CheckAccess())
            {
                if (show)
                    ProgressWindow.Instance.ShowDialog();
                else
                    ProgressWindow.Instance.Hide();

            }
            else
                this.Dispatcher.BeginInvoke(new HideProcessWindowThreadSafeDelegate(HideProcessWindowThreadSafe), show);
        }

        private void BtSetDefaults_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            new ConfigurationWindow(true);
            this.WindowState = WindowState.Normal;
        }

        private void BtShowSpecialSettings_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.Hide();
            SpecialSettingsWindow specialSettingsWindow = new SpecialSettingsWindow();
            specialSettingsWindow.Activate();
            specialSettingsWindow.BringIntoView();
            specialSettingsWindow.ShowDialog();
            
            this.Show();
            this.WindowState = WindowState.Normal;
        }
    }


}
