using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace replacer
{
   public class SwVersion
    {
        private Version actualSWVersion=new Version("4.1.0.0");

        public Version ActualSWVersion
        {
            get { return actualSWVersion; }
            set { actualSWVersion = value; }
        }

       
        public static SwVersion Instance=null;
        public SwVersion()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
                throw new Exception("This class is singleton DO NOT CREATE more!!");

            actualSWVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        }



    }
}
