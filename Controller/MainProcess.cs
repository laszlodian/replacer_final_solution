using Microsoft.Office.Interop.Word;
using replacer.Model;
using replacer.View;
using replacer.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Application = Microsoft.Office.Interop.Word.Application;
using Word = Microsoft.Office.Interop.Word;


namespace replacer.Controller
{
    public class MainProcess 
    {
        #region Variables
        public object matchCase, matchWholeWord, matchWildCards, matchSoundsLike, matchAllWordForms, forward, format, matchKashida, matchDiacritics, matchAlefHamza, matchControl, replace, wrap;
        private string OBJECT_TEXT = "{Object}";
        // C# doesn't have optional arguments so we'll need a dummy value
        object oMissing = System.Reflection.Missing.Value;
        public Dictionary<string, string> TextToFindAndTextToReplace = new Dictionary<string, string>();
        private Dictionary<FileInfo, List<int>> collectedFiles = new Dictionary<FileInfo, List<int>>();
        public Dictionary<string, int> indexOfObjectWord = new Dictionary<string, int>();
        public Dictionary<string, int> indexOfDocumentumTypeWord = new Dictionary<string, int>();
        public Application wordApp = null;
        public Document wordDoc = null;
        public Range docContent = null;

        private List<int> DocTypesFromTextFile = new List<int>();
        public List<Paragraph> paragrapshInCell = new List<Paragraph>();
        public const string TESTOBJECT_FILE_CONTENT_START_EXPRESSION = "KUKA Sunrise";
        public List<string> environmentFilesNeedToInsert = new List<string>();
        private bool environmentContentNeededToAppend = false;
        private List<string> possibleEnvFiles = new List<string>();
        public int filesCounter = 0;
        public bool bothEnvironmentContentNeeded = false;
        string allRobotText = string.Empty;
        private string allTextToLookFor = string.Empty;
        private object VAR_NULL_VALUE = null;
        #endregion

        #region Properties

        private List<int> docType = new List<int>();

        public List<int> DocType
        {
            get { return docType; }
            set { docType = value; }
        }


        private string documentsPath;
        public string DocumentsPath
        {
            get { return documentsPath; }
            set { documentsPath = value; }
        }

        private string textFilesPath;
        private List<Paragraph> cellParagraphs = new List<Paragraph>();
        private List<string> linesInParagraph = new List<string>();
        private Dictionary<int, string> paragDictionary = new Dictionary<int, string>();
        private List<FileInfo> filesToRemove = new List<FileInfo>();
        private int documentCount  = 0;
        private List<string> RobotTypesInDocument;
        private string textToLookForInDocument;
        public string TextToLookForInDocument { get {return textToLookForInDocument; } set {textToLookForInDocument=value; } }
        public int DocumentCount { get {return documentCount; } set {documentCount=value; } }
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
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pathOfDocuments"></param>
        /// <param name="pathOfTextFiles"></param>
        public MainProcess(string pathOfDocuments, string pathOfTextFiles)
        {

            Trace.TraceInformation("MainProcess constructor");
            TextFilesPath = pathOfTextFiles;
            DocumentsPath = pathOfDocuments;

            if (String.IsNullOrEmpty(TextFilesPath) || String.IsNullOrEmpty(DocumentsPath))
            {
                throw new Exception("Given argument TextFilesPath or DocumentsPath has not yet been initialized!");
            }

            FillDictionary();
        //    FillIndexOfWords();

            Trace.TraceInformation("Scaning of docx files preparing to start");
            DocumentModel.Instance.SetText("Scaning of docx files preparing to start");
            NewScanFiles(pathOfDocuments);

            Trace.TraceInformation("Specially close objects of Word with Marshall: ReleaseCOMObjects()");
            ReleaseCOMObjects();


        }

        /// <summary>
        /// Release COM object which are not needed,
        /// but reserves memory, these will released by Marshal method
        /// </summary>
        public void ReleaseCOMObjects()
        {
            Trace.TraceInformation("Releaseing COM objects");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (wordDoc != null)
                Marshal.FinalReleaseComObject(wordDoc);
            if (wordApp != null)
                Marshal.FinalReleaseComObject(wordApp);
        }
        public List<string> GetRobotTypesFromDocument(Word.Document wordDoc, Word.Application winWord)
        {
            List<string> robotTypes = new List<string>();
            Cell environmentRowCell = wordDoc.Content.Tables[3].Rows[4].Cells[2];
            string cellContent = environmentRowCell.Range.Text;
            TextToLookForInDocument = cellContent.Trim();
            string robotType = string.Empty;
            
            if (cellContent.Contains(Properties.Settings.Default.RobotTypeSeparatorChar))
            {
                foreach (string type in cellContent.Split(Properties.Settings.Default.RobotTypeSeparatorChar))
                {
                    if (type.EndsWith("\r\a"))
                    {
                        robotType=type.Substring(0, type.Length - 2);
                    }else
                        robotType = type;
         
                    robotType = robotType.Trim();
                    robotTypes.Add(robotType);
                }
            }
            else
                robotTypes.Add(cellContent.Substring(0, cellContent.Length - 2));

            return robotTypes;
        }

        private string GetRobotType(string fullRobotType)
        {
            var result = VAR_NULL_VALUE;
            string type = string.Empty;
            if (fullRobotType.Contains("omniMove"))
                result = Regex.Match(fullRobotType.Split(' ')[1], @"\d+$", RegexOptions.RightToLeft);
            else
                result = Regex.Match(fullRobotType, @"\d+$", RegexOptions.RightToLeft);

            return type;
        }
        
        private static void OpenDocument(string documentLocation, out Application winword, out Document wordDoc)
        {
            winword = new Word.Application();
            wordDoc = winword.Documents.Open(documentLocation, ReadOnly: true);
        }
             
        public void NewScanFiles(string path_in)
        {
            string type = string.Empty;
            Trace.TraceInformation("Scanning given folder({0}) for docx files and collect them",path_in);
            DocumentModel.Instance.SetText(string.Format("Scanning given folder({0}) for docx files and collect them", path_in));
            documentCount = new DirectoryInfo(path_in).GetFiles("*.docx").Length;
            foreach (FileInfo file in new DirectoryInfo(path_in).GetFiles("*.docx"))
            {
                Trace.TraceInformation("Found  docx file in path({0}): {1}", path_in, file);

                OpenDocument(file.FullName, out wordApp, out wordDoc);

                RobotTypesInDocument = GetRobotTypesFromDocument(wordDoc, wordApp);

                if (RobotTypesInDocument.Count == 1)
                    ReplaceText(file.Name, path_in, wordApp, wordDoc, RobotTypesInDocument.First());
                else
                    ReplaceMultipleText(file.Name, path_in, wordApp, wordDoc, RobotTypesInDocument);

            }
        }
        public void ReplaceMultipleText(string fileName, string path, Word.Application wordApp, Document wordDoc, List<string> robotTypes)
        {
            Trace.TraceInformation("Setting replacement options");
            DocumentModel.Instance.SetText("Setting replacement options");
            SetReplaceOptions(out matchCase, out matchWholeWord, out matchWildCards, out matchSoundsLike, out matchAllWordForms, out forward, out format, out matchKashida, out matchDiacritics, out matchAlefHamza, out matchControl, out replace, out wrap);

            Trace.TraceInformation("Read out predefined text from file for the given type");

            ReplaceTextObjectCellContent();

            ReplaceMultipleEnvironmentCellContent(robotTypes);

            UpdateTOC(wordDoc);
            SaveFixedDocumentToSpecifiedSubfolder(fileName, path);
            ExportAsPDF(fileName, path);
            FinalizeReplacement();
        }

        private void ReplaceMultipleEnvironmentCellContent(List<string> robotTypes)
        {
            string val = string.Empty;
            string itemToLookInDictionary = string.Empty;
            foreach (string item in robotTypes)
            {
                if (item.EndsWith("\r\n"))
                {
                    itemToLookInDictionary = item.Substring(0, item.Length - 2);
                  
                }else
                    itemToLookInDictionary = item;
                
                itemToLookInDictionary = itemToLookInDictionary.Trim();

                if (!TextToFindAndTextToReplace.TryGetValue(itemToLookInDictionary, out val))
                    throw new Exception(string.Format("Couldn't find value for {0} in dictionary TextToFindAndTextToReplace", item));
                allTextToLookFor = TextToLookForInDocument.Substring(0,textToLookForInDocument.Length-2);

                if (allRobotText == string.Empty)
                {
                    allRobotText += val;
                }
                else
                    allRobotText += Environment.NewLine + val;
            }

            ReplaceTextInTable( Properties.Settings.Default.IndexOfEnvironmentCell_Row,  Properties.Settings.Default.IndexOfEnvironmentCell_Column, allTextToLookFor, allRobotText);
            DocumentModel.Instance.SetText(string.Format("{0} word has been replaced", allTextToLookFor));
            Trace.TraceInformation("{0} word has been replaced", allTextToLookFor);
            allTextToLookFor = string.Empty;
            allRobotText = string.Empty;
            TextToLookForInDocument = string.Empty;
        }
        public void ReplaceText(string fileName, string path, Word.Application wordApp, Document wordDoc, string robotType)
        {
            Trace.TraceInformation("Setting replacement options");
            DocumentModel.Instance.SetText("Setting replacement options");
            SetReplaceOptions(out matchCase, out matchWholeWord, out matchWildCards, out matchSoundsLike, out matchAllWordForms, out forward, out format, out matchKashida, out matchDiacritics, out matchAlefHamza, out matchControl, out replace, out wrap);

            Trace.TraceInformation("Read out predefined text ({0})from file for the given type",robotType);
            DocumentModel.Instance.SetText(string.Format("Read out predefined text ({0}) from file for the given type",robotType));
            ReplaceTextObjectCellContent();
            ReplaceEnvironmentCellContent(robotType);

            #region Update TOC
            UpdateTOC(wordDoc);
            #endregion
            SaveFixedDocumentToSpecifiedSubfolder(fileName, path);

            #region Export as PDF
            ExportAsPDF(fileName, path);
            #endregion

            #region Close word-documentum, and wordApp
            FinalizeReplacement();
            #endregion
        }

        /// <summary>
        /// Updating the auto-generated Table Of Contents in the given Word Documentum
        /// </summary>
        private void UpdateTOC(Word.Document document)
        {
            Trace.TraceInformation("Updating Table Of Contents...");
            DocumentModel.Instance.SetText("Updating Table Of Contents...");
            foreach (TableOfContents item in document.TablesOfContents)
            {
                item.Update();
                item.UpdatePageNumbers();
            }
        }

        /// <summary>
        /// After replacement have been done for all documents could be found
        /// in the given document's path, then the fixed document is saved in docx format
        /// to a local property specified subfolder
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private string SaveFixedDocumentToSpecifiedSubfolder(string fileName, string path)
        {
            string file = fileName;
            string folderName = path;
            Directory.CreateDirectory(string.Format("{0}{1}{2}{1}",folderName ,Path.DirectorySeparatorChar,Properties.Settings.Default.FixedDocumentSubfolderName));

            object replacedFixedFile = string.Format("{0}{3}{2}{3}{1}", folderName, file, Properties.Settings.Default.FixedDocumentSubfolderName,Path.DirectorySeparatorChar).Trim();
      
            wordDoc.SaveAs2(ref replacedFixedFile);
       
            return folderName.Trim();
        }


        /// <summary>
        /// Closes Word document if it hasn't disposed(if it's value not null)
        /// Also Quit from WordApplication if it is alive, then kill all running
        /// process which name contains 'WinWord'
        /// </summary>
        private void FinalizeReplacement()
        {
            Trace.TraceInformation("Closing word document and word app");
            DocumentModel.Instance.SetText("Closing word document and word app");
         
            wordDoc.Close(false);
            wordApp.Quit();
        
            DocumentModel.Instance.KillAllWordProcess();

            Trace.TraceInformation("Replacement has been finished");
            DocumentModel.Instance.SetText("Replacement has been finished");
        }
        /// <summary>
        /// Exporting the fixed word docx document as a pdf file, then move it
        /// into the predefined subfolder at the local properties.Settings
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        private void ExportAsPDF(string fileName, string folderName)
        {
            Trace.TraceInformation("Exporting docx({0}) as pdf",fileName);
            DocumentModel.Instance.SetText(string.Format("Exporting docx({0}) as pdf", fileName));
            
            ExportAsPDFProcess(fileName, folderName);
          
            MoveAllPDFFilesToReplacedFolder(folderName);
        }

        /// <summary>
        /// Export the corrected word docx document to the
        /// specified path as a pdf file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        public void ExportAsPDFProcess(string fileName, string folderName)
        {
            FileInfo wordFile = new FileInfo(string.Format("{0}{1}{2}",folderName,Path.DirectorySeparatorChar,fileName));
            object outputFileName = wordFile.FullName.Replace(".docx", ".pdf");
            object fileFormat = WdSaveFormat.wdFormatPDF;

            // Save document into PDF Format
            wordDoc.SaveAs(ref outputFileName,
                ref fileFormat, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing);
        }

        private void ReplaceEnvironmentCellContent(string robotType)
        {
            string val = string.Empty;

            if (!TextToFindAndTextToReplace.TryGetValue(robotType, out val))
                throw new Exception(string.Format("Couldn't get value for key:{0} in TextToFindAndTextToReplace", robotType));

            ReplaceTextInTable( Properties.Settings.Default.IndexOfEnvironmentCell_Row,  Properties.Settings.Default.IndexOfEnvironmentCell_Column, robotType, val);
            Trace.TraceInformation("{0} word has been replaced", robotType);
            DocumentModel.Instance.SetText(string.Format("{0} word has been replaced", robotType));
        }

        private void ReplaceTextObjectCellContent()
        {
            string val;
            #region TestObject nearly cell content replacement
            if (!TextToFindAndTextToReplace.TryGetValue(TextToFindAndTextToReplace.Keys.ToArray()[0], out val))
                throw new Exception(string.Format("Couldn't get value for key:{0} in TextToFindAndTextToReplace", TextToFindAndTextToReplace.Keys.ToArray()[0]));

            ReplaceTextInTable(Properties.Settings.Default.IndexOfObjectWordCell_Row, Properties.Settings.Default.IndexOfObjectWordCell_Column, TextToFindAndTextToReplace.Keys.ToArray()[0], val);
            Trace.TraceInformation("{Object} word has been replaced");
            DocumentModel.Instance.SetText("{Object} word has been replaced");
            #endregion

        }

        /// <summary>
        /// Moves the exported fixed documents (pdf files) to
        /// the predefined subfolder in the local Properties.Settings
        /// </summary>
        /// <param name="folderName"></param>
        private void MoveAllPDFFilesToReplacedFolder(string folderName)
        {
            string sourcePath = folderName.Trim();
            string destinationPath = string.Format("{0}{1}{2}{1}",folderName,Path.DirectorySeparatorChar, Properties.Settings.Default.FixedDocumentSubfolderName);
            foreach (string sourceFile in Directory.GetFiles(sourcePath, "*.pdf"))
            {
                string fileName = Path.GetFileName(sourceFile);
                string destinationFile = Path.Combine(destinationPath, fileName);

                if (!File.Exists(destinationFile))
                {
                    File.Move(sourceFile, destinationFile);
                }
            }
        }

        public void ReplaceTextInTable(int rowIndex, int cellIndex, string textToFind, string newText)
        {
            Trace.TraceInformation("Replacing {0} text to {1} in document",textToFind,newText);
            DocumentModel.Instance.SetText(string.Format("Replacing {0} text to {1} in document", textToFind, newText));

            Microsoft.Office.Interop.Word.Table table = wordDoc.Tables[3];
            Row row = table.Rows[rowIndex];
            Cell cell = row.Cells[cellIndex];

            if (cell.Range.Find.Execute(textToFind))
            {
                cell.Range.Cut();

                #region If testObject cell is manipulated then formating needed
                if (newText.StartsWith(TESTOBJECT_FILE_CONTENT_START_EXPRESSION))
                {
                    FormattingParagraphInCell(cell, newText);
                }
                #endregion              
                cell.Range.Text = newText;
            }

        }
        public void FormattingParagraphInCell(Cell cell_in, string text)
        {
            int paragraphCount = 0;
            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string line in lines)
            {
                if (line.StartsWith("KUKA Sunrise"))
                {
                    string paragraph = stringBuilder.ToString();
                    stringBuilder.Append(Environment.NewLine + Environment.NewLine);

                    paragraphCount++;
                    paragraph = string.Empty;
                }
                stringBuilder.AppendLine(string.Format("{0}\r\n", line));
            }
        }

        //private void FillIndexOfWords()
        //{
        //    if (indexOfObjectWord.Count == 0)
        //    {
        //        indexOfObjectWord.Add("row", 3);
        //        indexOfObjectWord.Add("cell", 2);
        //        Trace.TraceInformation("Filling dictionary:  the {Object} word is in 3rd row and 2nd cell");
        //    }

        //    if (indexOfDocumentumTypeWord.Count == 0)
        //    {
        //        Trace.TraceInformation("Filling dictionary:  the KMP200 word is in 4rd row and 2nd cell");
        //        indexOfDocumentumTypeWord.Add("row", 4);
        //        indexOfDocumentumTypeWord.Add("cell", 2);
        //    }

        //}
        /// <summary>
        /// Configuring up the different parameters and mods for the replacement
        /// </summary>
        /// <param name="matchCase"></param>
        /// <param name="matchWholeWord"></param>
        /// <param name="matchWildCards"></param>
        /// <param name="matchSoundsLike"></param>
        /// <param name="matchAllWordForms"></param>
        /// <param name="forward"></param>
        /// <param name="format"></param>
        /// <param name="matchKashida"></param>
        /// <param name="matchDiacritics"></param>
        /// <param name="matchAlefHamza"></param>
        /// <param name="matchControl"></param>
        /// <param name="replace"></param>
        /// <param name="wrap"></param>
        private static void SetReplaceOptions(out object matchCase, out object matchWholeWord, out object matchWildCards, out object matchSoundsLike, out object matchAllWordForms, out object forward, out object format, out object matchKashida, out object matchDiacritics, out object matchAlefHamza, out object matchControl, out object replace, out object wrap)
        {

         
            //options
            matchCase = false;
            matchWholeWord = true;
            matchWildCards = false;
            matchSoundsLike = false;
            matchAllWordForms = false;
            forward = true;
            format = false;
            matchKashida = false;
            matchDiacritics = false;
            matchAlefHamza = false;
            matchControl = false;
            object read_only = false;
            object visible = true;
            replace = 2;
            wrap = 1;
        }
        public string ReadTextFromFile(string path, string textFileName)
        {
            Trace.TraceInformation("Read textfile content");
            string stringsFromFile = string.Empty;
            Trace.TraceInformation("Read textfile content from path: {0} and filename: {1}", path, textFileName);
            DocumentModel.Instance.SetText(string.Format("Read textfile content from path: {0} and filename: {1}", path, textFileName));
        
            string completePath = Path.Combine(path, textFileName.Trim());
            Trace.TraceInformation("Complet path is: {0}", completePath);
            File.Copy(completePath, string.Format("{0}{2}{1}", path, Properties.Settings.Default.SettingsFileName,Path.DirectorySeparatorChar), true);
            
            foreach (string line in File.ReadLines(string.Format("{0}/{1}", path, Properties.Settings.Default.SettingsFileName)))
            {
                stringsFromFile += line + Environment.NewLine;
            }

            File.Delete(string.Format("{0}{2}{1}", path, Properties.Settings.Default.SettingsFileName,Path.DirectorySeparatorChar));

            return stringsFromFile;

        }
        public void FillReplaceDictionary(string textToReplace, string stringsFromFile)
        {
            TextToFindAndTextToReplace.Add(textToReplace, stringsFromFile);
        }
        private void FillDictionary()
        {
            string stringsFromFile = string.Empty;

            foreach (Configuration config in Configurations.Collection)
            {
                if (config.SettingName.ToLower().Contains("object"))
                {
                    stringsFromFile = ReadTextFromFile(TextFilesPath, config.SettingValue);
                    FillReplaceDictionary(OBJECT_TEXT, stringsFromFile);
                }
            }
            foreach (KeyValuePair<string, string> setting in MainWindow.Instance.TestEnvironmentFiles.Keys)
            {
                GetContentFromTextFile(TextFilesPath, setting.Value);
            }
        }

        private void GetContentFromTextFile(string path_in, string textFileName)
        {
            string docType = string.Empty;

            var result = Regex.Match(textFileName.Split('.')[0], @"\d+$", RegexOptions.RightToLeft);
            if (result.Success)
            {
                docType = Convert.ToString(result);
                
                if (DocumentModel.Instance.IsSpecialModeUsed)
                {

                    if (DocumentModel.Instance.MultiRobotTypeDefined)
                    {
                        foreach (string word in DocumentModel.Instance.TextsToLookFor)
                        {
                            var resultOfSpecialDefinedRobot = Regex.Match(word, @"\d+$", RegexOptions.RightToLeft);
                            if (result.Success)
                            {
                                if (!textFileName.Contains(Convert.ToString(result)))
                                {
                                    return;
                                }
                            }
                        }
                    }else
                    {
                        var resultOfSpecialDefinedRobot = Regex.Match(DocumentModel.Instance.TextToLookFor, @"\d+$", RegexOptions.RightToLeft);
                        if (resultOfSpecialDefinedRobot.Success)
                        {
                            if (!textFileName.Contains(Convert.ToString(resultOfSpecialDefinedRobot)))
                            {
                                return;
                            }
                        }
                    }

                    if (DocumentModel.Instance.MultiRobotTypeDefined)
                    {
                        foreach (string robotDefined in DocumentModel.Instance.TextsToLookFor)
                        {
                            FillDictionaryManager(path_in, textFileName, docType, robotDefined);
                        }
                    }else
                        FillDictionaryManager(path_in, textFileName, docType, DocumentModel.Instance.TextToLookFor);
                }
                else
                {
                    if (docType == "1500" || docType == "200")
                    {
                        FillDictionaryManager(path_in, textFileName, docType, string.Format("KMP {0} omniMove", docType));
                    }
                    else
                        FillDictionaryManager(path_in, textFileName, docType, string.Format("KMP{0}", docType));
                }

            }
            else
                Trace.TraceError("One of the filenames is not correctly named because it has no number at the end of the environment file");
        }

        private void FillDictionaryManager(string path_in, string textFileName, string docType, string robotType)
        {
            string stringsFromFile = ReadTextFromFile(path_in, textFileName);
            Trace.TraceInformation("Readed string from TestEnvironmentFile file: {0}", stringsFromFile);
            DocumentModel.Instance.SetText(string.Format("Readed string from TestEnvironmentFile file: {0}", stringsFromFile));
       
            string txtToReplace = string.Format(robotType, docType);
            FillReplaceDictionary(txtToReplace, stringsFromFile);
        }
        /// <summary>
        /// Clean Ups the variables,fields and properties to be able run again 
        /// the replace process without restarting application
        /// </summary>
        private void CleanUp()
        {
            TextToFindAndTextToReplace = new Dictionary<string, string>();
            collectedFiles = new Dictionary<FileInfo, List<int>>();
            indexOfObjectWord = new Dictionary<string, int>();
            indexOfDocumentumTypeWord = new Dictionary<string, int>();
            DocTypesFromTextFile = new List<int>();
            paragrapshInCell = new List<Paragraph>();
            environmentFilesNeedToInsert = new List<string>();
            possibleEnvFiles = new List<string>();
            cellParagraphs = new List<Paragraph>();
            docType = new List<int>();
            linesInParagraph = new List<string>();
            paragDictionary = new Dictionary<int, string>();
        }
    }
}
