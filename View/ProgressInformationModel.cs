using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace replacer.View
{
    public class ProgressInformationModel : INotifyPropertyChanged
    {
        #region Properties
        private string infoText;

        public string InfoText
        {
            get
            {
                return infoText;
            }
            set
            {
                infoText = value;
                OnPropertyRaised("InfoText");
            }
        } 
        #endregion


        #region INotifíPropertyChanged interface implementation
        public static ProgressInformationModel Instance;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (propertyname == "InfoText")
            {
                ChangeInfoLabelContent(InfoText);
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private void ChangeInfoLabelContent(string infoText)
        {
            throw new NotImplementedException();
        }
        #endregion

        public ProgressInformationModel()
        {
            Instance = this;
            Trace.TraceInformation("ProgressInformationModell constructor");


        }
    }
}
