using replacer.View;
using replacer.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataGridCell = System.Windows.Controls.DataGridCell;
using MessageBox = System.Windows.Forms.MessageBox;

namespace replacer.View
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {

        #region Properties

        private List<string> settingValues = new List<string>();
        public List<string> SettingValues
        {
            get
            {
                return settingValues;
            }
            set
            {
                settingValues = value;
            }
        }

        private List<string> settingNames = new List<string>();
        public List<string> SettingNames
        {
            get
            {
                return settingNames;
            }
            set
            {
                settingNames = value;
            }
        }

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

        private Dictionary<KeyValuePair<string, string>, int> testEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();

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

        #region Variables

        public Dictionary<string, string> settingsDictionary = new Dictionary<string, string>();

        private List<string> rowsList = new List<string>();
        private bool objOK = false;
        private bool envOK = false;

        public static ConfigurationWindow Instance;
        private int rowIndex;
        private DataRowView actualRowView;
        private int actualRowIndex;
        private string settingNameText;
        #endregion

        public ConfigurationWindow(bool forShow)
        {

            Instance = this;


            InitializeComponent();

            ClearAllRowsInDataGrid();            

            LoadSettings();
            if (forShow)
            {
         
                this.Activate();
                this.BringIntoView();
                this.WindowState = WindowState.Normal;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.ShowDialog();
            }
        }

        public static ConfigurationWindow CreateInstance()
        {

            Instance = new ConfigurationWindow(false);

            return Instance;
        }

        public void CollectAllSettings()
        {

            var configuration = ConfigurationWindow.Instance;

            configuration.LoadSettings();

          

        }


        public void LoadSettings()
        {
            Trace.TraceInformation("Load settings");


            if (Properties.Settings.Default.IsLocalPropertiesActive)
            {
                ReadSettingsFromProperties();
            }
            else
                ReadSettingsFileContent(Properties.Settings.Default.SettingsFileName);




            DisplaySettingsInDataGrid(settingsDictionary);
            CheckSettingValues();



            MainWindow.Instance.SettingsDict = settingsDictionary;
        }
        public void CleanUp()
        {

            settingNames = new List<string>();

            settingValues = new List<string>();

            settingsDictionary = new Dictionary<string, string>();

            rowsList = new List<string>();

            testEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();
        }
        public void ReadSettingsFromProperties()
        {
            int i = 0;
            foreach (SettingsPropertyValue item in Properties.Settings.Default.PropertyValues)
            {
                if (item.Name.ToLower().Contains("environment") || item.Name.ToLower().Contains("object") || item.Name.ToLower().Contains("path") || item.Name.ToLower().Contains("document") || item.Name.ToLower().Contains("file"))
                {
                    settingsDictionary.Add(item.Name, Convert.ToString(item.PropertyValue));
                    i++;
                }

            }
        }



        /// <summary>
        /// az 1.sor a documentumokat tartatalmazó könyvtár
        ///a 2. sor a textfile-okat tartalmazó könyvtár
        ///amelyik setting neve számmal végződik és tartatalmazza az 'environment'-et az testenvironment file
        ///amelyik setting neve tartalmazza az 'object' -et az TestObject file
        /// </summary>
        private void TryToGetSettingsUsage()
        {
            testEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();
            int settingsCounter = 0;
            foreach (string key in settingsDictionary.Keys)
            {

                if (key.ToLower().Contains("document") && key.ToLower().Contains("path"))
                {
                    defaultPathToLookForDocuments = settingsDictionary[key].Trim();
                }
                else if (key.ToLower().Contains("file") && key.ToLower().Contains("path"))
                {
                    defaultPathToLookForInformationFiles = settingsDictionary[key].Trim();
                }
                else if (key.ToLower().Contains("object") && key.ToLower().Contains("file"))
                {
                    testObjectFileName = settingsDictionary[key].Trim();
                }
                else
                {

                    string endNumber = string.Empty;
                    var result = Regex.Match(key.Trim(), @"\d+$", RegexOptions.RightToLeft);
                    if (result.Success)
                    {
                        Trace.TraceInformation("The settings name ending with number: {0}", result);
                        endNumber = Convert.ToString(result);
                        testEnvironmentFiles.Add(new KeyValuePair<string, string>(key.Trim(), settingsDictionary[key].Trim()), Int32.Parse(endNumber));
                    }
                    else
                        Trace.TraceError("Couldn't find number at the end of the settings name!!");
                }
                settingsCounter++;
            }
        }

        public void WriteSettingsToTextFile()
        {
            ReadSettingsFromDataGrid(mainGrid);
            if (Properties.Settings.Default.IsLocalPropertiesActive)
            {
                StoreConfigsToLocalSettings();
            }
            else
                ReCreatingSettingsFile();

        }

        private void StoreConfigsToLocalSettings()
        {
            List<string> names = new List<string>();
            List<string> values = new List<string>();
            Dictionary<string, string> settingNameValuePairs = new Dictionary<string, string>();

            foreach (string line in rowsList)
            {
                names.Add(line.Split('=')[0]);
                values.Add(line.Split('=')[1]);

                if (!settingNameValuePairs.ContainsKey(line.Split('=')[0]))
                {
                    settingNameValuePairs.Add(line.Split('=')[0], line.Split('=')[1]);
                }

            }
            int i = 0;
            foreach (SettingsPropertyValue item in Properties.Settings.Default.PropertyValues)
            {
                if (names.Contains(item.Name) || values.Contains(item.PropertyValue))
                {
                    if (names.IndexOf(item.Name) != -1)
                    {
                        if (values[names.IndexOf(item.Name)] != Convert.ToString(item.PropertyValue))
                        {
                            item.PropertyValue = values[names.IndexOf(item.Name)];

                        }
                        if (names[values.IndexOf(Convert.ToString(item.PropertyValue))] != item.Name)
                        {
                            item.Name.Replace(item.Name, names[values.IndexOf(Convert.ToString(item.PropertyValue))]);
                        }

                    }
                }
            }

            int propCounter = 0;
            foreach (string propName in names)
            {
                if (!Properties.Settings.Default.Context.ContainsKey(propName))
                {
                    Properties.Settings.Default.Context.Add(propName, values[propCounter]);
                }
            }

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }

        private void ReCreatingSettingsFile()
        {
            Trace.TraceInformation("ReCreating the settings file");
            if (File.Exists(Properties.Settings.Default.SettingsFileName))
            {

                while (File.Exists(Properties.Settings.Default.SettingsFileName))
                {
                    try
                    {
                        File.Delete(Properties.Settings.Default.SettingsFileName);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("ex message: {0}", ex.Message);
                    }
                }


            }
            using (var fs = File.Create(Properties.Settings.Default.SettingsFileName))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fs))
                {
                    foreach (string line in rowsList)
                    {
                        file.WriteLine(line);
                    }
                    file.Close();
                }
                fs.Close();
            }
        }

        private void ReadSettingsFromDataGrid(System.Windows.Controls.DataGrid mainGrid)
        {


            foreach (Configuration item in Configurations.Collection)
            {
                rowsList.Add(string.Format("{0} = {1}", item.SettingName, item.SettingValue));
            }
        }

        private void DisplaySettingsInDataGrid(Dictionary<string, string> settingsDictionary)
        {
            foreach (string key in settingsDictionary.Keys)
                Configurations.Collection.Add(new Configuration()
                {
                    SettingName = key,
                    SettingValue = settingsDictionary[key]
                });
            this.mainGrid.Items.Refresh();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to save the modifications in the settings of the application?",
                "Save before close settings window", System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                CheckSettingValues();

                ReadSettingsFromDataGrid(mainGrid);

                if (Properties.Settings.Default.IsLocalPropertiesActive)
                {
                    StoreConfigsToLocalSettings();
                }
                else
                    ReCreatingSettingsFile();
            }

            this.Close();

            MainWindow.Instance.WindowState = WindowState.Normal;
            MainWindow.Instance.Activate();
        }


        private bool CheckSettingValues()
        {


            bool res = true;
            int itemCounter = 0;

            foreach (Configuration item in Configurations.Collection)
            {
                if ((item.SettingValue == string.Empty || item.SettingValue == null) || (item.SettingName == string.Empty || item.SettingName == null))
                {
                    res = false;

                    if (item.SettingName == string.Empty || item.SettingName == null)
                    {
                        System.Windows.MessageBox.Show(string.Format("{0} beállításnak nincs neve", item.SettingName));
                    }
                    if (item.SettingValue == string.Empty || item.SettingValue == null)
                    {
                        System.Windows.MessageBox.Show(string.Format("{0} beállításnak nincs értéke", item.SettingValue));
                    }
                }
                else if (item.SettingValue.Contains(".txt"))
                {
                    if (defaultPathToLookForInformationFiles != string.Empty && defaultPathToLookForDocuments != null)
                    {
                        if (!File.Exists(System.IO.Path.Combine(defaultPathToLookForInformationFiles.Trim(), item.SettingValue.Trim())))
                        {
                            System.Windows.MessageBox.Show(string.Format("{0} fájl nem található!", item.SettingValue));
                            res = false;
                        }
                        else
                        {
                            string line = string.Empty;
                            if (defaultPathToLookForInformationFiles != string.Empty)
                            {
                                using (StreamReader tr = new StreamReader(string.Format("{0}/{1}", defaultPathToLookForInformationFiles.Trim(), item.SettingValue.Trim())))
                                {
                                    line = tr.ReadLine();

                                    if (line.Trim() == string.Empty)
                                    {
                                        System.Windows.MessageBox.Show(string.Format("{0} fájl nem tartalmaz egyetlen sort sem!", item.SettingValue.Trim()));
                                        res = false;
                                    }
                                }

                            }

                        }
                    }


                }
                else
                {
                    if (!Directory.Exists(string.Format("{0}", item.SettingValue)))
                    {
                        System.Windows.MessageBox.Show(string.Format("{0} könyvtár nem található!", item.SettingValue));
                        res = false;
                    }
                    else
                    {
                        if (new DirectoryInfo(item.SettingValue).GetFiles().Length <= 0)
                        {
                            System.Windows.MessageBox.Show(string.Format("A {0} könyvtár nem tartalmaz egyetlen fájlt sem!", item.SettingValue));
                            res = false;
                        }
                    }

                }

                if (item.SettingName.ToLower().Contains("file") && item.SettingName.ToLower().Contains("path"))
                {
                    defaultPathToLookForInformationFiles = item.SettingValue;
                    if (Directory.Exists(defaultPathToLookForInformationFiles))
                    {
                        DefaultPathToLookForInformationFiles = item.SettingValue;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(string.Format("{0} könyvtár nem található!", item.SettingValue));
                        res = false;
                    }
                }
                else if (item.SettingName.ToLower().Contains("path") && item.SettingName.ToLower().Contains("document"))
                {
                    defaultPathToLookForDocuments = item.SettingValue;
                    if (Directory.Exists(defaultPathToLookForDocuments))
                    {
                        DefaultPathToLookForDocuments = item.SettingValue;
                    }
                    else
                    {
                        res = false;
                        System.Windows.MessageBox.Show(string.Format("{0} könyvtár nem található!", item.SettingValue));
                    }
                }
                else if (item.SettingName.ToLower().Contains("object"))
                {
                    objOK = true;
                }
                else if (item.SettingName.ToLower().Contains("environment"))
                {
                    envOK = true;
                }

                itemCounter++;
            }



            return res && objOK && envOK;
        }

        private void ClearAllRowsInDataGrid()
        {

            Configurations.Collection.Clear();
        }

        private void BtOK_Click(object sender, RoutedEventArgs e)
        {
            if (CheckSettingValues())
            {
                ReadSettingsFromDataGrid(mainGrid);

                ReCreatingSettingsFile();

                this.Close();

                MainWindow.Instance.WindowState = WindowState.Normal;
                MainWindow.Instance.Activate();
            }
        }

        public void ReadSettingsFileContent(string settingsFile)
        {
            string settingsText = string.Empty;
            string line = string.Empty;

            if (!File.Exists(Properties.Settings.Default.SettingsFileName))
                ReCreatingSettingsFile();

            using (StreamReader sr = new StreamReader(Properties.Settings.Default.SettingsFileName))
            {
                while ((line = sr.ReadLine()) != null && line != string.Empty)
                {
                    if (!settingsDictionary.ContainsKey(line.Split('=')[0]))
                    {
                        settingsDictionary.Add(line.Split('=')[0], line.Split('=')[1]);
                    }

                }
                sr.Close();
            }


            TryToGetSettingsUsage();
            ReadSettingsFromDataGrid(mainGrid);

        }

        

   

      
    }
}
