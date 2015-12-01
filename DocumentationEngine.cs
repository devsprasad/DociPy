using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DociPy
{


    public class DocumentationEngine
    {
        public string ProjectTitle { get; set; }
        public string Version { get; set; }
        public string[] Contributors { get; set; }
        public bool IncludeModulePath { get; set; }
        public string OutputDirectory { get; set; }
        public string RootDirectory { get; set; }

        public DocumentationEngine()
        {
            this.ProjectTitle = "MyProject";
            this.Version = "1.0.0";
            this.Contributors = new string[] { System.Windows.Forms.SystemInformation.UserName };
            this.OutputDirectory = "./docs";
        }

        private string find_relative_path(string target, string exact_file)
        {
            string rel_path = "";
            System.IO.FileInfo fi = new System.IO.FileInfo(exact_file);
            target = target.Replace('\\', '/').Replace(@"\\","/").Trim('/');
            int count = target.Split('/').Length - 2;
            for (int i = 0 ; i < count ; i++)
            {
                rel_path += "../";
            }
            rel_path = System.IO.Path.Combine(rel_path, "styles",fi.Name);
            rel_path = rel_path.Replace("\\", "/");
            return rel_path;
        }

        private string sanitize(string path)
        {
            path = path.Replace(@"//", @"/");
            path = path.Replace(@"/", @"\");
            return path;
        }


        public string _ProjectInfo(string html)
        {
            html = html.Replace("[project_title]", this.ProjectTitle);
            html = html.Replace("[project_version]", this.Version);
            return html;
        }
        public void WriteHTML(List<ScriptInfo> scrInf, string template, string theme)
        {
            System.IO.Directory.CreateDirectory(this.OutputDirectory);
            System.IO.Directory.CreateDirectory(this.OutputDirectory + "./styles/");
            System.IO.File.Copy(theme, this.OutputDirectory + "./styles/" + theme.Split('\\').Last(),true);

            var html_file_in = new System.IO.StreamReader(template);
            string html_data = html_file_in.ReadToEnd();
            html_file_in.Close();
            
            foreach (ScriptInfo scr in scrInf)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(scr.SourceModule);

                string source_dir = fi.Directory.ToString().Replace(this.RootDirectory, "").Replace('\\','/');
                string output_dir = this.OutputDirectory + "/" + source_dir;

                output_dir = sanitize(output_dir);
                var dir = System.IO.Directory.CreateDirectory( output_dir);

                string path = System.IO.Path.Combine(output_dir, scr.SourceModule.Split('\\').Last() + ".html");

                path = sanitize(path);
                var html_file_out = new System.IO.StreamWriter(path);
                theme = find_relative_path(output_dir, theme);

                string html = _ProjectInfo(html_data);
                html = html.Replace("[style]", theme);
                html = html.Replace("[body]", start(scr));
                html_file_out.Write(html);
                html_file_out.Close();
            }

        }

        public string start(ScriptInfo scrInf)
        {
            string body = "<div class='module'>";
            body += "<label class='module-name'>" + scrInf.SourceModule.Split('\\').Last() + "</label>";
            body += "<p class='module-info'>" + scrInf.Description + "</p>";
            foreach (var item in scrInf.Functions)
            {
                PyFunction pf = item.Value;
                body += _processPyF(pf);
            }
            body += "</div>";
            return body;
        }

        private string _processPyF(PyFunction pf)
        {
            string html = "<div class='function'>";
            html += "<label class='function-name'>" + pf.Name + "</label>";
            string args = "";
            foreach (var item in pf.args)
            {
                args += "<span class='function-arg'>" + item + "</span>" + "  ,  ";
            }
            html += " (" + args.Trim().Trim(',') + ") ";
            html += "<label class='function-info'>" + pf.Description + "</label>";

            html += "</div>";
            return html;
        }
    }
}
