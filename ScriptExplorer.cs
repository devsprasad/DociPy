using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy
{

    public class ScriptInfo
    {
        public string SourceModule { get; set; }
        public string Description { get; set; }
        public List<KeyValuePair<string, PyFunction>> Functions { get; set; }
        public List<KeyValuePair<string, PyClass>> Class { get; set; }

        public ScriptInfo()
        {
            Functions = new List<KeyValuePair<string, PyFunction>>();
            Class = new List<KeyValuePair<string, PyClass>>();
            SourceModule = "";
        }
    }

    public class ScriptExplorer
    {
        public virtual ScriptInfo ProcessFile(string file)
        {
            throw new NotImplementedException();
        }
    }
}
