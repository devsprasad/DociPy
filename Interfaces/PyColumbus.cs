using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DociPy.PyTypes;



namespace DociPy
{


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
            fs.Close();
            fs.Dispose();
        }
        public string FileName
        {
            get { return _fileName; }
            set
            {
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

        public PyModule Process(string file = "")
        {
            this.FileName = file;
            try
            {
                PyModule pm = _process();
                return pm;
            } catch (Exception ex)
            {
                throw new Exception("Error while processing " + file + " ( " + ex.Message.Trim() + ")");
            }
        }

        public PyModule _process()
        {
            PyModule pm = new PyModule();
            pm.Name = this.FileName;
            int i = 0;
            List<string> current_doc = new List<string>();
            while (i < this._lines.Length)
            {
                string line = _lines[i];
                if (line.Trim().Trim('\t').Trim().StartsWith("#"))
                {
                    Debug.Print("comment start");
                    line = line.Trim().Trim('#').Trim();
                    current_doc.Add(line);
                }
                else if (line.Trim().Trim('\t').Trim().StartsWith("def "))
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
                else if (line.Trim().Trim('\t').Trim().StartsWith("import "))
                {
                    string[] imports = line.Replace("import ", "").Split(',');
                    Debug.Print("Dependency found");
                    pm.Imports.AddRange(imports);
                }
                else
                {
                    if (current_doc.Count == i)
                    {
                        pm.Description = string.Join("\n", current_doc.ToArray());
                        pm.RawDocLine = current_doc;
                    }
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

        private string[] _parseClassHead(string header)
        {
            int name_end = header.Contains("(") ? header.IndexOf("(") : header.IndexOf(":");
            string name = header.Substring(0, name_end);
            string str_base = "";
            if (header.Contains("("))
            {
                string[] bases = header.Substring(name_end + 1, header.IndexOf(")") - name_end - 1).Split(',');
                for (int i = 0 ; i < bases.Length ; i++)
                {
                    bases[i] = bases[i].Trim();
                }
                str_base = string.Join(",", bases);
            }
            return new string[] { name, str_base };
        }
        private PyClass _processPyClass(ref int i, List<string> doc)
        {
            Debug.Print("class starts");
            pc = new PyClass();
            pc.RawDocLine = doc;
            _inClassDef = true;
            // **********************
            string header = _lines[i];
            header = header.Replace("class ", "");
            string[] parts = _parseClassHead(header);
            string name = parts[0];
            pc.Name = name;
            pc.BaseClasses = parts[1].Split(',');
            i++;
            int start = i;
            while (i < _lines.Length)
            {
                string line = _lines[i];
                int Indent = _indentLevel(line);
                if (Indent == 0 && line != "\r")
                {
                    break; // class ends here
                }
                else
                {
                    i++;
                }
            }
            string[] _tmp_line = _lines;
            this._lines = new ArraySegment<string>(_lines, start, i - start).ToArray();
            _process();
            this._lines = _tmp_line;
            foreach (string item in doc)
            {
                if (item.StartsWith("@"))
                {
                    if (item.StartsWith("@ref"))
                    {
                        string[] refernce = _splitRefTag(item);
                        pc.Description += "<a href=\"" + refernce[0] + "\">" + refernce[1] + "</a>";
                    }
                }
                else
                {
                    pc.Description += item + " ";
                }
            }
            Debug.Print("class ends");
            // **********************
            _inClassDef = false;
            return pc;
        }

        private string[] _splitParamComm(string header)
        {
            int arg_start = header.IndexOf(":") + 1;
            int arg_end = header.IndexOf(":", arg_start);
            string arg_name = header.Substring(arg_start, arg_end - arg_start);
            string arg_info = header.Substring(arg_end + 1);
            return new string[] { arg_name, arg_info };
        }

        private string[] _splitRefTag(string header)
        {
            int ref_start = header.IndexOf(":") + 1;
            int ref_end = header.IndexOf(":", ref_start);
            string ref_url, ref_title;
            if (ref_end == -1)
            {
                ref_end = header.Length;
                ref_url = header.Substring(ref_start, ref_end - ref_start);
                ref_title = ref_url;
            }
            else
            {
                ref_url = header.Substring(ref_start, ref_end - ref_start);
                ref_title = header.Substring(ref_end + 1);
            }
            return new string[] { ref_url, ref_title };
        }

        public PyFunction _processPyFunction(ref int i, List<string> doc)
        {
            PyFunction pf = new PyFunction();
            pf.RawDocLine = doc;
            string header = _lines[i].Trim();
            header = header.Replace("def ", "");
            string name = header.Substring(0, header.IndexOf("("));
            pf.Name = name;
            var _args = _processPyArgs(header);
            string func_doc = "";
            foreach (string item in doc)
            {
                if (item.StartsWith("@"))
                {
                    if (item.StartsWith("@param"))
                    {
                        string[] _param = _splitParamComm(item);
                        if (_args.ContainsKey(_param[0]))
                        {
                            _args[_param[0]].Description = _param[1];
                        }
                        else
                        {
                            throw new InvalidOperationException("no argument named '" + _param[0] + "'");
                        }
                    }
                    //else if (item.StartsWith("@ref"))
                    //{
                    //    string[] refernce = _splitRefTag(item);
                    //    pf.Description += "<a href=\"" + refernce[0] + "\">" + refernce[1] + "</a>";
                    //}
                }
                else
                {
                    func_doc += item + " ";
                }
            }
            pf.Args = _args.Values.ToList();
            pf.Description = func_doc;
            return pf;
        }



        public Dictionary<string, PyArg> _processPyArgs(string header)
        {
            Dictionary<string, PyArg> pargs = new Dictionary<string, PyArg>();
            int start = header.IndexOf("(") + 1;
            int end = header.IndexOf("):");
            string arg_header = header.Substring(start, end - start);
            string[] args = arg_header.Split(',');
            if (_inClassDef) args = args.SkipWhile(x => { return (x.Trim() == "self"); }).ToArray();
            foreach (var item in args)
            {
                string arg_name = item.Trim();
                if (arg_name != "")
                {
                    PyArg parg = new PyArg();
                    parg.Name = arg_name;
                    string[] parts = item.Split('=');
                    if (parts.Length == 2)
                    {
                        parg.Name = parts[0].Trim();
                        parg.Default = parts[1].Trim();
                    }
                    pargs.Add(parg.Name, parg);
                }
            }
            return pargs;
        }
    }
}
