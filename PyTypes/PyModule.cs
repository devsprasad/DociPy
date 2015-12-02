using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy.PyTypes
{

    public class PyModule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PyFunction> Functions { get; set; }
        public List<PyClass> Classes { get; set; }
        public List<string> RawDocLine { get; set; }
        public List<string> Imports { get; set; }
        public PyModule()
        {
            Functions = new List<PyFunction>();
            Classes = new List<PyClass>();
            Imports = new List<string>();
            this.RawDocLine = new List<string>();
            this.Description = "";
        }
    }
}
