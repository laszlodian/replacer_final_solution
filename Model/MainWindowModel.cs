using replacer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace replacer.Model
{
  public class MainWindowModel
    {

       
            private ObservableCollection<Configuration> configs = Configurations.Collection;

            public ObservableCollection<Configuration> ConfigurationCollection
            {
                get { return configs; }
                set
                {
                    configs = value;
                }
            }
        

    }
}
