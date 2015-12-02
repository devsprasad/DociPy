using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy.PyTypes
{
    public class PyClass
    {
        public string Name { get; set; }
        public List<PyFunction> Methods { get; set; }
        public string Description { get; set; }
        public string[] BaseClasses { get; set; }
        public PyClass()
        {
            this.Name = this.Description = "";
            this.Methods = new List<PyFunction>();
            this.BaseClasses = new string[] { };
        }
    }
}
