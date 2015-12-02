using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy.PyTypes
{

    
    public class PyFunction
    {
        public string Name { get; set; }
        public List<PyArg> Args { get; set; }
        public string[] ReturnTypes { get; set; }
        public List<string> RawDocLine { get; set; }
        public string Description { get; set; }
        public PyFunction()
        {
            this.Name = this.Description = "";
            this.Args = new List<PyArg>();
            this.RawDocLine = new List<string>();
        }
    }

    

}

