using replacer.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace replacer.Model
{
 public class DocumentModel
    {
        #region Properties
        private string specialSeperator;

        public string SpecialSeperator
        {
            get { return specialSeperator; }
            set { specialSeperator = value; }
        }

        private List<string> textsToLookFor = new List<string>();

        public List<string> TextsToLookFor
        {
            get { return textsToLookFor; }
            set { textsToLookFor = value; }
        }

        private bool isSpecialModeUsed;

        public bool IsSpecialModeUsed
        {
            get { return isSpecialModeUsed; }
            set { isSpecialModeUsed = value; }
        }

        private static string textToLookFor;
        public static DocumentModel Instance;

        public string TextToLookFor
        {
            get { return textToLookFor; }
            set { textToLookFor = value; }
        }


        private string specialPattern;

        public string SpecialPattern
        {
            get { return specialPattern; }
            set { specialPattern = value; }
        }

        private bool multiRobotTypeDefined;

        public bool MultiRobotTypeDefined
        {
            get { return multiRobotTypeDefined; }
            set { multiRobotTypeDefined = value; }
        } 
        #endregion

        /// <summary>
        /// Default Contstructor
        /// </summary>
        public  DocumentModel()
        {
            Instance = this;
            Trace.TraceInformation("DocumentModell has been created");
        }
        /// <summary>
        /// Get all the running processes from the operating system,
        /// and if the process name contains 'WINWORD' string then try to
        /// kill it.
        /// </summary>
        public void KillAllWordProcess()
        {
            Trace.TraceInformation("Kill all proccess with string containing WINWORD");
            foreach (Process item in Process.GetProcesses())
            {
                if (item.ProcessName.Contains("WINWORD"))
                {
                    try
                    {
                        item.Kill();
                    }
                    catch
                    {
#if Debug
                        MessageBox.Show(string.Format("Could kill {0} process because of 'Access Denied'", item.ProcessName));
#endif
                        Trace.TraceError("Could kill {0} process because of 'Access Denied'", item.ProcessName);
                    }
                }
            }
        }

        /// <summary>
        /// This method helps to change the content of the info label to
        /// inform the user from the actual task
        /// </summary>
        /// <param name="text"></param>
        private delegate void SetTextDelegate(string text);
        public void SetText(string text)
        {
            if (ProgressWindow.Instance.Dispatcher.CheckAccess())
            {
              
                    ProgressWindow.Instance.lbInfo.Content = text;
                
                ProgressInformationModel.Instance.InfoText = text;
                
            }
            else
                ProgressWindow.Instance.Dispatcher.BeginInvoke(new SetTextDelegate(DocumentModel.Instance.SetText), text);
        }

    }
}
