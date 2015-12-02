using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy.PyTypes
{
    public class PyArg
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }

        public List<string> RawDocLine { get; set; }
        public PyArg()
        {
            this.RawDocLine = new List<string>();
        }
    }
}
