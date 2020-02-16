using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using replacer.Model;
using replacer.View;
using replacer.Model;

namespace replacer.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SpecialSettingsWindow : Window
    {
        public static SpecialSettingsWindow Instance = null;

        public SpecialSettingsWindow()
        {
            Instance = this;
            InitializeComponent();

        }        

        private void NoModificationNoSpecialSettingUsed()
        {
            ConfigureDocumentProperties(false);           

            MainWindow.Instance.WindowState = WindowState.Normal;
            MainWindow.Instance.Activate();
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            NoModificationNoSpecialSettingUsed();
            
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            ConfigureDocumentProperties(true);

            DeserializeSpecialSetting();

            System.Windows.MessageBox.Show(string.Format("Your settings has been saved. Text to look for: {0}",tbPattern.Text.Trim()));

            this.Close();         

        }

        private void DeserializeSpecialSetting()
        {
            string specPattern = DocumentModel.Instance.SpecialPattern;
            if (specPattern.Contains("{") && specPattern.Contains("}"))
            {
                DocumentModel.Instance.SpecialSeperator = specPattern.Substring(specPattern.IndexOf('{')+1, specPattern.IndexOf('}') -1).Substring(0,1);
            }
            if (specPattern.Contains('|'))
            {
                DocumentModel.Instance.MultiRobotTypeDefined = true;
                specPattern = specPattern.Substring(specPattern.IndexOf("Robots:")+7);
               string[] robotTypesDefined=specPattern.Split('|');

                foreach (string robotDefined in robotTypesDefined)
                {
                    DocumentModel.Instance.TextsToLookFor.Add(robotDefined.Trim());
                }
            }else
            {
                DocumentModel.Instance.MultiRobotTypeDefined = false;
                specPattern = specPattern.Substring(specPattern.IndexOf("Robots:") + 7);
                DocumentModel.Instance.TextToLookFor = specPattern.Trim();
            }
        }

        private void ConfigureDocumentProperties(bool isSpecialTextUsed)
        {
            DocumentModel.Instance.IsSpecialModeUsed = isSpecialTextUsed;
           
            if (isSpecialTextUsed)
                DocumentModel.Instance.SpecialPattern = tbPattern.Text.Trim();
            else
                DocumentModel.Instance.SpecialPattern = string.Empty;            
        }

        private void btHelp_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog();
            helpWindow.Close();
            this.Activate();
            this.BringIntoView();
        }
    }
}
