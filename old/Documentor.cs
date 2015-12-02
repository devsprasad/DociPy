using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DociPy.PyTypes;
namespace DociPy
{

    public class Theme
    {
        public string module { get; set; }
        public string module_name { get; set; }
        public string module_info { get; set; }
        public string module_classes { get; set; }
        public string module_functions { get; set; }
        public string Class { get; set; }
        public string class_name { get; set; }
        public string class_info { get; set; }
        public string class_methods { get; set; }
        public string class_method { get; set; }
        public string function { get; set; }
        public string function_name { get; set; }
        public string function_info { get; set; }
        public string function_args { get; set; }
        public string function_arg { get; set; }
        private List<string> styles = new List<string>();
        public List<string> Styles
        {
            get { return styles; }
        }
        public Theme(string dir)
        {
            string[] _styles = System.IO.Directory.GetFiles(dir, "*.css", System.IO.SearchOption.AllDirectories);
            this.Styles.AddRange(_styles);
            System.IO.StreamReader fs = new System.IO.StreamReader(dir + "\\");
        }

    }
    public class ColumbusDocumentor
    {
        private PyColumbus _parser = new PyColumbus();
        private Theme theme;
        private string[] files;
        // paths

        // themeing;
        

        private string _root = "";
        public string RootDirectory
        {
            get { return _root; }
            set { _root = System.IO.Path.GetFullPath(value); }
        }

        private string _out_dir = "";
        public string OutputDirectory
        {
            get { return _out_dir; }
            set { _out_dir = System.IO.Path.GetFullPath(value); }
        }

        public ColumbusDocumentor(string rootDir)
        {
            this.RootDirectory = rootDir;
            if (System.IO.Directory.Exists(this.RootDirectory))
            {
                files = System.IO.Directory.GetFiles(this.RootDirectory);
            }
            else
            {
                throw new System.IO.DirectoryNotFoundException(this.RootDirectory);
            }
        }

        public void Generate(string themeDIR)
        {
            List<PyModule> pms = new List<PyModule>();
            foreach (string item in this.files)
            {
                pms.Add(this._parser.Process(item));
            }
            if (pms.Count > 0)
            {
                if (System.IO.Directory.Exists(themeDIR))
                {
                    this.theme = new Theme(themeDIR);
                    System.IO.Directory.CreateDirectory(this._out_dir); // create output directory
                    System.IO.Directory.CreateDirectory(this._out_dir + "./styles/"); // create styles directory
                    foreach (string item in this.theme.Styles)
                    {
                        System.IO.File.Copy(item, this._out_dir + "./styles/" + item.Split('\\').Last(), true);
                    }
                }
                else
                {
                    throw new System.IO.DirectoryNotFoundException(themeDIR);
                }
            }
        }

    }
}
