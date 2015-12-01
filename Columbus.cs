using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy
{



    public class Columbus : ScriptExplorer
    {

        #region [    Columbus : The Script Explorer    ]

        delegate void logFunction(string msg);

        private string[] _lines;
        private void log(string msg)
        {
            System.Diagnostics.Debug.Print(msg);
        }

        private string _currentFile = "";
        public new ScriptInfo ProcessFile(string file)
        {
            _currentFile = file;
            var fop = new System.IO.StreamReader(file);
            string data = fop.ReadToEnd();
            this._lines = data.Split('\n');
            ScriptInfo scrInf = ProcessLines();
            fop.Close();
            return scrInf;
        }
        private ScriptInfo ProcessLines()
        {
            ScriptInfo scr = new ScriptInfo();
            scr.SourceModule = _currentFile;
            int i = 0;
            string current_doc = "";
            while (i < _lines.Length)
            {
                string line = _lines[i];
                if (line.StartsWith("#") && i==0)
                {
                    while (line.StartsWith("#") && i < _lines.Length)
                    {
                        scr.Description += line.Trim('#');
                        i++;
                        if (i >= _lines.Length || line.Trim() == "") break;
                        line = _lines[i];
                    }
                }
                if (line.StartsWith("def"))
                {
                    log("==> function:");
                    PyFunction pf = ProcessFunctionHeader(line);
                    log("  |==> Name: " + pf.Name);
                    log("  |==> Args: " + string.Join(" , ", pf.args));
                    if (current_doc != "")
                    {
                        pf.Description = current_doc;
                        current_doc = "";
                    }
                    scr.Functions.Add(new KeyValuePair<string,PyFunction>( pf.Name, pf));                    
                }
                else if (line.StartsWith("class"))
                {
                    log("==> Class : " + line.Substring(6));
                    PyClass pc = ProcessClassHeader(i);
                    if (current_doc != "")
                    {
                        pc.Description = current_doc;
                        current_doc = "";
                    }
                    scr.Class.Add(new KeyValuePair<string,PyClass>( pc.Name,pc));
                }
                if (line.Trim() != "" && line.StartsWith("#")) current_doc += line.Trim().Trim('#').Trim();
                else { current_doc = ""; }
                i++;
            }
            return scr;
        }

        private PyClass ProcessClassHeader(int start) {
            PyClass pc = new PyClass();
            string header = _lines[start];
            pc.Name = header.Substring(6);
            return pc;
        }
        private PyFunction ProcessFunctionHeader(string line)
        {
            PyFunction func = new PyFunction();
            string func_header = line.Substring(4);
            int i = 0;
            string tmp = "";
            while (i < func_header.Length)
            {
                if (func_header[i] == '(')
                {
                    func.Name = tmp;
                    i++;
                    break;
                }
                tmp += func_header[i];
                i++;
            }
            List<string> args = new List<string>();
            tmp = "";
            while (i < func_header.Length)
            {
                if (func_header[i] != ',')
                {
                    if (func_header[i] == ')')
                    {
                        if (tmp.Trim() != "") args.Add(tmp);
                        func.args = args.ToArray();
                        break;
                    }
                    tmp += func_header[i];
                }
                else
                {
                    args.Add(tmp);
                    tmp = "";
                }
                i++;
            }
            return func;
        }
        #endregion
    }

    public class PyClass
    {
        public string Name { get; set; }
        public string[] Constructors { get; set; }
        public string Description { get; set; }
        public PyClass()
        {
            this.Name = "N/A";
            this.Constructors = new string[] { };
            this.Description = "";
        }
    }
    public class PyFunction
    {
        public string Name { get; set; }
        public string[] args { get; set; }
        public string Description { get; set; }

        public PyFunction()
        {
            this.Name = "N/A";
            this.args = new string[] { };
            this.Description = "";
        }
    }
}
