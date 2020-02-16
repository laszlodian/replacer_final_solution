using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace replacer.ViewModel
{
    public class Configuration : INotifyPropertyChanged
    {
        private static System.Windows.Controls.Button btBrowse = new System.Windows.Controls.Button();
        #region Properties

        private string settingName;
        public string SettingName
        {
            get { return settingName; }
            set
            {
                settingName = value;
                OnPropertyChanged("SettingName");
            }
        }
        private string settingValue;

        public string SettingValue
        {
            get { return settingValue; }
            set
            {
                settingValue = value;
                OnPropertyChanged("SettingValue");
            }
        }
        //private System.Windows.Controls.Button browseButtonProperty = btBrowse;

        //public System.Windows.Controls.Button BrowseButtonProperty
        //{
        //    get { return browseButtonProperty; }
        //    set { browseButtonProperty = value; OnPropertyChanged("BrowseButton"); }
        //}

        #endregion

        //public Configuration()
        //{
        //    btBrowse.Content = "...";
        //    btBrowse.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        //    btBrowse.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
        //    btBrowse.VerticalAlignment = System.Windows.VerticalAlignment.Center;
        //    btBrowse.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        //    btBrowse.BorderBrush = System.Windows.Media.Brushes.Black;
        //    btBrowse.BorderThickness = new Thickness(2);
        //    btBrowse.FontFamily = new System.Windows.Media.FontFamily("Arial Black");
        //    btBrowse.FontWeight = FontWeights.Bold;
        //    btBrowse.Click += BtBrowse_Click;
        //}

        //private void BtBrowse_Click(object sender, RoutedEventArgs e)
        //{
        //    System.Windows.MessageBox.Show("btBrowse clicked");
        //}

        #region INotifyPropertyChange Interface Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
