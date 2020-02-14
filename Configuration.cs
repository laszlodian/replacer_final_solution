using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace replacer
{
    public class Configuration : INotifyPropertyChanged
    {
        private string settingName;
        public string SettingName
        {
            get { return settingName; }
            set
            {
                settingName = value;
                OnPropertyChanged();
            }
        }
        private string settingValue;      

        public string SettingValue
        {
            get { return settingValue; }
            set
            {
                settingValue = value;
                OnPropertyChanged();
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
