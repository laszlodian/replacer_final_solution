using replacer.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace replacer
{
    public class SQLManager
    {
        internal static object TestObjectFile;
        internal static object Instance;
        internal static string TestEnvironmentFile200;
        internal static string DefaultPathToLookForInformationFiles;
        internal static string DefaultPathToLookForDocuments;

        // #region Properties
        // private List<string> settingValues = new List<string>();

        // public List<string> SettingValues
        // {
        //     get
        //     {
        //         return settingValues;
        //     }
        //     set
        //     {
        //         settingValues = value;
        //     }
        // }

        // private List<string> settingNames = new List<string>();

        // public List<string> SettingNames
        // {
        //     get
        //     {
        //         return settingNames;
        //     }
        //     set
        //     {
        //         settingNames = value;
        //     }
        // }
        // private SqlConnection connection;

        // public SqlConnection Connection
        // {
        //     get { return connection; }
        //     set { connection = value; }
        // }

        // public static string DefaultPathToLookForDocuments { get; internal set; }
        // public static string DefaultPathToLookForInformationFiles { get; internal set; }
        // public static string TestObjectFileName { get; internal set; }

        // private static string TestEnvironmentFileName1500;

        // public static string TestEnvironmentFileName200 { get; internal set; }
        // #endregion

        // #region Variables
        // public static Dictionary<KeyValuePair<string, string>, int> TestEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();
        // private static string QUERY_ALL_COLUMN_FROM_SETTINGS_TABLE = string.Format("select * from dbo.Settings");
        // public static SQLManager Instance;
        // public string SETTINGS_FILE_NAME = "Settings.txt";
        // public static string DefaultPathToDocuments = string.Empty;
        // public static string DefaultPathToFiles = string.Empty;
        // public static string TestEnvironmentFile200 = string.Empty;
        // public static string TestEnvironmentFile1500 = string.Empty;
        // public static string TestObjectFile = string.Empty;

        // private static string ALL_SETTINGS_QUERY = "select DefaultPathForLookDocuments,DefaultPathToLookForInformationFiles,TestObjectFileName,TestEnvironmentFileName1500,TestEnvironmentFileName200 from dbo.Settings";
        // private static DataTable settingsDataTable = new DataTable();
        // public static List<string> settingsValuesFromDatabase = new List<string>();
        // private List<string> settingNamesList = new List<string>();
        // #endregion

        // public string[] lines =
        //{
        //     String.Format("DefaultPathToLookForDocuments = {0}", DefaultPathToDocuments),
        //     String.Format("DefaultPathToLookForInformationFiles = {0}", DefaultPathToFiles),
        //     String.Format("TestEnvironmentFileName200 = {0}", TestEnvironmentFile200),
        //     String.Format("TestEnvironmentFileName1500 = {0}", TestEnvironmentFile1500),
        //     String.Format("TestObjectFileName = {0}", TestObjectFile.Trim())
        // };

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SQLManager()
        {
            //            CreateInstance();
            //            Trace.TraceInformation("SQLManager constructor");

            //            FillProperties();
        }

        public static string TestEnvironmentFile1500 { get; internal set; }
        public static object DefaultPathToDocuments { get; internal set; }
        public static object DefaultPathToFiles { get; internal set; }

        internal void CreateSettingsFile()
        {
            throw new NotImplementedException();
        }

        internal void ReadTextFile()
        {
            throw new NotImplementedException();
        }
        //        public SQLManager(bool forShow)
        //        {
        //            Instance = this;

        //            if (forShow)
        //            {
        //                Trace.TraceInformation("SQLManager constructor");

        //                FillProperties();
        //            }
        //        }
        //        public void CleanUp()
        //        {
        //            settingNames = new List<string>();
        //            settingValues = new List<string>();
        //            TestEnvironmentFiles = new Dictionary<KeyValuePair<string, string>, int>();
        //            settingsValuesFromDatabase = new List<string>();
        //            settingNamesList = new List<string>();

        //            // }
        //            // public void CreateInstance()
        //            // {
        //            //     Instance = this;
        //            // }
        //            // private void FillProperties()
        //            // {

        //            //     if (Properties.Settings.Default.IsSQLDBUsed)
        //            //     {
        //            //         ReadSettingsFromDataBase();
        //            //     }
        //            //     else if (Properties.Settings.Default.IsLocalPropertiesActive)
        //            //     {
        //            //         ConfigurationWindow.Instance.ReadSettingsFromProperties();
        //            //     }
        //            //     else
        //            //     {
        //            //         ReadSettingsFromFile();
        //            //     }
        //            // }

        //            // #region File Handling Methods

        //            // private void ReadSettingsFromFile()
        //            // {
        //            //     Trace.TraceInformation("Reading from settings file");
        //            //     if (File.Exists(SETTINGS_FILE_NAME))
        //            //     {
        //            //         ReadSettingsFileContent(@"Settings.txt");
        //            //         //  ReadTextFile();
        //            //     }
        //            //     else if (DefaultPathToDocuments == string.Empty)
        //            //     {
        //            //         MessageBox.Show("The Settings File has only Empty values, please fill them for the first use!");
        //            //     }
        //            // }
        //            // public void ReadSettingsFileContent(string settingsFile)
        //            // {
        //            //     string settingsText = string.Empty;
        //            //     string line = string.Empty;

        //            //     using (StreamReader sr = new StreamReader(settingsFile))
        //            //     {
        //            //         while ((line = sr.ReadLine()) != null)
        //            //         {
        //            //             //settingsDictionary.Add(line.Split('=')[0], line.Split('=')[1]);
        //            //         }
        //            //         sr.Close();
        //            //     }

        //            // }

        //            // public void ReadTextFile()
        //            // {
        //            //     Trace.TraceInformation("Reading from settings file");
        //            //     if (!File.Exists(SETTINGS_FILE_NAME))
        //            //     {
        //            //         CreateSettingsFile();
        //            //     }
        //            //     using (var fs = new FileStream(SETTINGS_FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //            //     using (var sr = new StreamReader(fs))
        //            //     {
        //            //         string line = string.Empty;

        //            //         while ((line = sr.ReadLine()) != null)
        //            //         {
        //            //             line = sr.ReadLine();
        //            //             string[] keyValue = line.Split('=');

        //            //             if (keyValue[0].Contains("Document") && keyValue[0].Contains("Path"))
        //            //             {
        //            //                 DefaultPathToDocuments = keyValue[1];
        //            //             }
        //            //             else if (keyValue[0].Contains("environment") || keyValue[0].Contains("test"))
        //            //             {
        //            //                 int endNumber;
        //            //                 var result = Regex.Match(keyValue[0].Trim(), @"\d+$", RegexOptions.RightToLeft);
        //            //                 if (result.Success)
        //            //                 {
        //            //                     Trace.TraceInformation("The settings name ending with number: {0}", result);
        //            //                     endNumber = Convert.ToInt32(result);
        //            //                     TestEnvironmentFiles.Add(new KeyValuePair<string, string>(keyValue[0], keyValue[1]), endNumber);
        //            //                 }
        //            //             }
        //            //             else if (keyValue[0].Contains("object") && keyValue[0].Contains("file"))
        //            //             {
        //            //                 TestObjectFile = keyValue[1];
        //            //             }
        //            //         }
        //            //     }
        //            // }

        //            // public void CreateSettingsFile()
        //            // {
        //            //     Trace.TraceInformation("ReCreating the settings file");

        //            //     if (File.Exists(SETTINGS_FILE_NAME))
        //            //     {
        //            //         try
        //            //         {
        //            //             File.Delete(SETTINGS_FILE_NAME);
        //            //         }
        //            //         catch (Exception ex)
        //            //         {
        //            //             Trace.TraceError("ex message: {0}", ex.Message);
        //            //         }
        //            //     }

        //            //     using (var fs = File.Create(SETTINGS_FILE_NAME))
        //            //     {
        //            //         using (System.IO.StreamWriter file = new System.IO.StreamWriter(fs))
        //            //         {
        //            //             foreach (string line in lines)
        //            //             {
        //            //                 file.WriteLine(line);
        //            //             }
        //            //         }
        //            //     }
        //            // }
        //            // #endregion

        //            // #region SQL Commands
        //            // public object SQLExecuteScalar(string query)
        //            // {

        //            //     object res = null;
        //            //     using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
        //            //     {
        //            //         try
        //            //         {
        //            //             conn.Open();

        //            //             using (SqlCommand command = new SqlCommand(query))
        //            //             {
        //            //                 res = command.ExecuteScalar();

        //            //                 if (res == null)
        //            //                 {
        //            //                     throw new Exception();
        //            //                 }
        //            //             }
        //            //         }
        //            //         catch (Exception)
        //            //         {

        //            //             throw new Exception();
        //            //         }
        //            //         finally
        //            //         {
        //            //             conn.Close();
        //            //         }

        //            //     }
        //            //     return res;
        //            // }
        //            // public List<string> SQLQueryAllSettings()
        //            // {

        //            //     object res = null;
        //            //     using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
        //            //     {
        //            //         try
        //            //         {
        //            //             conn.Open();

        //            //             using (SqlDataReader reader = new SqlCommand("select * from dbo.Settings").ExecuteReader())
        //            //             {
        //            //                 string docsPath = Convert.ToString(reader["DefaultPathForLookDocuments"]);
        //            //                 string filesPath = Convert.ToString(reader["DefaultPathToLookForInformationFiles"]);
        //            //                 string testObjectFile = Convert.ToString(reader["TestObjectFileName"]);
        //            //                 string environment1500 = Convert.ToString(reader["TestEnvironmentFileName1500"]);
        //            //                 string environment200 = Convert.ToString(reader["TestEnvironmentFileName200"]);


        //            //                 settingNamesList.AddRange(new string[] { docsPath, filesPath, testObjectFile, environment1500, environment200 });
        //            //             }
        //            //         }
        //            //         catch (Exception)
        //            //         {

        //            //             throw new Exception();
        //            //         }
        //            //         finally
        //            //         {
        //            //             conn.Close();
        //            //         }

        //            //     }
        //            //     return settingNamesList;
        //            // }



        //            // /// <summary>
        //            // /// Returns the datatable 
        //            // /// </summary>
        //            // /// <param name="sqlCommand"></param>
        //            // /// <returns></returns>
        //            // public DataTable GetData(string sqlCommand)
        //            // {
        //            //     DataTable table;
        //            //     using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
        //            //     {
        //            //         table = new DataTable();

        //            //         SqlCommand command = new SqlCommand(sqlCommand, conn);
        //            //         SqlDataAdapter adapter = new SqlDataAdapter();
        //            //         adapter.SelectCommand = command;

        //            //         table.Locale = System.Globalization.CultureInfo.InvariantCulture;
        //            //         adapter.Fill(table);

        //            //     }
        //            //     return table;
        //            // }
        //            // public void SQLNonQuery()
        //            // {

        //            //     object res = null;
        //            //     using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
        //            //     {
        //            //         try
        //            //         {
        //            //             conn.Open();

        //            //             using (SqlCommand command = new SqlCommand(string.Format("insert into dbo.{0}(DefaultPathToLookForDocuments,DefaultPathToLookForDocuments,DefaultPathToLookForInformationFiles,TestObjectFileName,TestEnvironmentFileName1500,TestEnvironmentFileName1500) VALUES({1}, {2},{3},{4})", DefaultPathToDocuments, DefaultPathToFiles, TestObjectFile, TestEnvironmentFile1500, TestEnvironmentFile200)))
        //            //             {
        //            //                 res = command.ExecuteScalar();

        //            //                 if (res == null)
        //            //                 {
        //            //                     throw new Exception();
        //            //                 }
        //            //             }
        //            //         }
        //            //         catch (Exception)
        //            //         {

        //            //             throw new Exception();
        //            //         }
        //            //         finally
        //            //         {
        //            //             conn.Close();
        //            //         }

        //            //     }
        //            // }

        //            // private void ReadSettingsFromDataBase()
        //            // {
        //            //     throw new NotImplementedException();
        //            // }
        //            // public void RunQuickQuery()
        //            // {
        //            //     settingsDataTable = GetData(ALL_SETTINGS_QUERY);
        //            //     foreach (DataRow row in settingsDataTable.Rows)
        //            //     {
        //            //         for (int i = 1; i < row.ItemArray.Length; i++)
        //            //         {
        //            //             settingsValuesFromDatabase.Add(Convert.ToString(row.ItemArray[i]));
        //            //         }
        //            //     }
        //            // }

        //            // public void Dispose()
        //            // {
        //            //     connection = null;
        //            //     Connection = null;
        //            //     Instance = null;
        //            // }

        //            // public void SQLQueryAllField()
        //            // {

        //            //     using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
        //            //     {
        //            //         try
        //            //         {
        //            //             conn.Open();

        //            //             using (SqlDataReader queryAll = new SqlCommand(string.Format("select * from dbo.Settings")).ExecuteReader())
        //            //             {
        //            //                 SQLManager.DefaultPathToLookForDocuments = Convert.ToString(queryAll["DefaultPathToLookForDocuments"]);
        //            //                 SQLManager.DefaultPathToLookForInformationFiles = Convert.ToString(queryAll["DefaultPathToLookForInformationFiles"]);
        //            //                 SQLManager.TestObjectFileName = Convert.ToString(queryAll["TestObjectFileName"]);
        //            //                 SQLManager.TestEnvironmentFileName1500 = Convert.ToString(queryAll["TestEnvironmentFileName1500"]);
        //            //                 SQLManager.TestEnvironmentFileName200 = Convert.ToString(queryAll["TestEnvironmentFileName200"]);
        //            //             }


        //            //         }
        //            //         catch (Exception)
        //            //         {

        //            //             throw;
        //            //         }
        //            //     }
        //            // }
        //#endregion
        //}
    }
}
