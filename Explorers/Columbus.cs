using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DociPy.Explorers
{

    public class PyArg
    {
        public string Name { get; set; }
        public string Type {get;set;}
        public string Default {get;set;}
        public string Description {get;set;}
    }
    public class PyFunction
    {
        public string Name { get; set; }
        public List<PyArg> Args { get; set; }
        public string[] ReturnTypes {get;set;}
        public string Description{get;set;}
        public PyFunction()
        {
            this.Name = this.Description = "";
            this.Args = new List<PyArg>();
        }
    }

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

    public class PyModule
    {
        public string Name { get; set; }
        public List<PyFunction> Functions { get; set; }
        public List<PyClass> Classes { get; set; }
        public PyModule()
        {
            Functions = new List<PyFunction>();
            Classes = new List<PyClass>();
        }
    }

    public class PyColumbus
    {
        private string _fileName = "";
        private string _file_data = "";
        private string[] _lines = new string[] { };

        private void readData()
        {
            System.IO.StreamReader fs = new System.IO.StreamReader(this.FileName);
            this._file_data = fs.ReadToEnd();
            this._lines = this._file_data.Split('\n');
        }
        public string FileName
        {
            get { return _fileName; }
            set { 
                if (!System.IO.File.Exists(value))
                {
                    throw new System.IO.FileNotFoundException(value);
                }
                if (_fileName != value)
                {
                    _fileName = value;
                    readData();
                }
            }
        }

        public PyColumbus()
        {
        }

        public void Process(string file = "")
        {
            this.FileName = file;
           PyModule pm = _process();
        }

        public PyModule _process()
        {
            PyModule pm = new PyModule();
            int i = 0;
            List<string> current_doc = new List<string>();
            while (i < this._lines.Length)
            {
                string line = _lines[i].Trim().Trim('\t').Trim();
                if (line.StartsWith("#"))
                {
                    Debug.Print("comment start");
                    line = line.Trim().Trim('#').Trim();
                    current_doc.Add(line);
                }
                else if (line.StartsWith("def "))
                {
                    // a normal function starts here
                    PyFunction pf = _processPyFunction(ref i, current_doc);
                    if (_inClassDef && pc != null)
                    {
                        pc.Methods.Add(pf);
                    }
                    else
                    {
                        pm.Functions.Add(pf);
                    }
                    current_doc.Clear();
                }
                else if (line.StartsWith("class "))
                {
                    PyClass pc = _processPyClass(ref i, current_doc);
                    current_doc.Clear();
                    pm.Classes.Add(pc);
                    pc = null;
                }
                else
                {
                    current_doc.Clear();
                }
                i++;
            }
            return pm;
        }


        private int _indentLevel(string line)
        {
            line = line.Replace("\t", "    ");
            int count = line.Length - line.TrimStart(' ').Length;
            return count;
            
        }
        private bool _inClassDef = false;
        private PyClass pc = null;
        private PyClass _processPyClass(ref int i, List<string> doc)
        {
            Debug.Print("class starts");
            pc = new PyClass();
            _inClassDef = true;
            // **********************
            string header = _lines[i];
            header = header.Replace("class ", "");
            string name = header.Substring(0, header.Contains("(")? header.IndexOf("(") : header.IndexOf(":") );
            i++;
            int start = i;
            while (i < _lines.Length)
            {
                string line = _lines[i];
                int Indent = _indentLevel(line);
                if (Indent == 0)
                {
                    break; // class ends here
                }
                else
                {
                    i++;
                }
            }
            string[] _tmp_line = _lines;
            this._lines = new ArraySegment<string>(_lines, start,i-start).ToArray();
            _process();
            this._lines = _tmp_line;
            Debug.Print("class ends");
            // **********************
            _inClassDef = false;
            return pc;
        }


        public PyFunction _processPyFunction(ref int i, List<string> doc)
        {
            PyFunction pf = new PyFunction();
            string header = _lines[i].Trim();
            header = header.Replace("def ", "");
            string name = header.Substring(0, header.IndexOf("("));
            if (_inClassDef)
            {
                Debug.Print("method starts" + header);
            }
            else
            {
                Debug.Print("function starts: " + header);
            }
            pf.Args= _processPyArgs(header);
            return pf;
        }



        public List<PyArg> _processPyArgs(string header)
        {
            List<PyArg> pargs = new List<PyArg>();
            int start = header.IndexOf("(")+1;
            int end = header.IndexOf("):");
            string arg_header = header.Substring(start, end - start);
            string[] args = arg_header.Split(',');
            if (_inClassDef) args = args.SkipWhile (x => { return (x.Trim() == "self"); }).ToArray();
            foreach (var item in args)
            {
                PyArg parg = new PyArg();
                parg.Name = item.Trim();
                string[] parts = item.Split('=');
                if (parts.Length == 2)
                {
                    parg.Name = parts[0].Trim();
                    parg.Default = parts[1].Trim();
                }
                pargs.Add(parg);
            }
            return pargs;
        }
    }
}
