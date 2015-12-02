using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DociPy
{

    public abstract class AbstractDocEngine
    {
        public delegate void Log( object sender, string msg);
        public event Log NewLogMessage;

        public abstract string Theme
        {
            get;
            set;
        }
        public abstract string OutputDirectory
        {
            get;
            set;
        }
        public abstract string RootDirectory
        {
            get;
            set;
        }

        public  void log(string message)
        {
            if (NewLogMessage != null)
            {
                NewLogMessage(this,message);
            }
        }
        public virtual string Generate(string[] files)
        {
            throw new NotImplementedException();
        }
        
    }

  
}
