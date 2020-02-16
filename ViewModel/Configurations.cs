using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace replacer.ViewModel
{
    public static class Configurations
    {
        private static ObservableCollection<Configuration> _configs = new ObservableCollection<Configuration>();

        public static ObservableCollection<Configuration> Collection
        {
            get { return _configs; }
        }

    }
}
