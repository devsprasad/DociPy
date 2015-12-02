using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DociPy
{
    public partial class mainForm : Form
    {

        public mainForm()
        {
            InitializeComponent();
            Program.PyInit(new CustomStream(ipyEdit));
            setupEditor();
        }


        private void setupEditor()
        {
            ipyEdit.Settings.EnableIntegers = false;
            ipyEdit.AppendText("IronPython Interactive Interpreter v" + IronPython.CurrentVersion.DisplayVersion);
            ipyEdit.init();
        }


        private string[] splitLine(string line)
        {
            List<string> s = new List<string>();
            string c_word = "";
            for (int i = 0 ; i < line.Length ; i++)
            {
                char c = line[i];
                if (!char.IsWhiteSpace(c))
                {
                    c_word += c;
                }
                else
                {
                    s.Add(c_word);
                    System.Diagnostics.Debug.Print(c_word);
                    c_word = "";
                }
            }
            return s.ToArray();
        }

        private string template_path = @".\data\";
        private void loadThemes()
        {
            string path = System.IO.Path.Combine(template_path, "Themes");
            string[] files = System.IO.Directory.GetFiles(path, "*.css", System.IO.SearchOption.AllDirectories);
            foreach (string template in files)
            {
                System.IO.FileInfo f = new System.IO.FileInfo(template);
                string rel_template_path = template.Replace(template_path, "");
                string template_name = f.Name;
                cmbThemes.Items.Add(rel_template_path);
            }
            if (cmbThemes.Items.Count > 0) cmbThemes.SelectedIndex = 0;
        }


        private List<string> engines;
        private void LoadEngines()
        {
            engines = new List<string>();
            foreach (string name in Program.ipy.EngineScope.GetVariableNames())
            {
                object obj = Program.ipy.EngineScope.GetVariable(name);
                if (obj != null)
                {
                    if (obj.GetType() == typeof(IronPython.Runtime.Types.PythonType))
                    {
                        IronPython.Runtime.Types.PythonType pt = (IronPython.Runtime.Types.PythonType)obj;
                        if (name.StartsWith("Engine"))
                        {
                            engines.Add(name);
                            cmbDocEngines.Items.Add(name.Replace("Engine",""));
                        }
                    }
                }
            }
            cmbDocEngines.SelectedIndex = 0;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            Program.ipy.LoadLibs(@".\data\processors");
            LoadEngines();
            loadThemes();
        }

        private void log(string msg, bool newline = true)
        {
            txtLogs.AppendText(msg);
            if (newline) txtLogs.AppendText("\n");
            txtLogs.SelectionStart = txtLogs.TextLength;
            txtLogs.ScrollToCaret();
        }

        private void ipyEdit_CommandEntered(object sender, SimbedEnvisionCodeEditor.CommandEnteredEventArgs e)
        {
            if (e.Command == "exit")
            {

            }
            else if (e.Command == "clear")
            {
                ipyEdit.Clear();
            }
            else
            {
                e.SuppressEnter = true;
                ipyEdit.AppendText("\n");
                if (e.Command.Trim() != "")
                {
                    try
                    {
                        Program.ipy.PyExecute(e.Command, Microsoft.Scripting.SourceCodeKind.InteractiveCode);
                    } catch (Exception ex)
                    {
                        if (Program.ipy_bridge.ShortErrorMessage)
                        {
                            ipyEdit.AppendText("E: " + ex.Message);
                        }
                        else
                        {
                            ipyEdit.AppendText("E: " + Program.ipy.getTraceback(ex));
                        }
                    }
                }
            }
        }

        private void ipyEdit_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void updateList()
        {
            lstFiles.Items.Clear();
            foreach (var item in paths)
            {
                lstFiles.Items.Add(item);
            }
            lblCount.Text = lstFiles.Items.Count.ToString() + " Files";
        }
        private void AddFile(string file, string root = null)
        {
            string p = file;
            if (root != null) p = file.Replace(root, "{root}");
            else p = file.Split('\\').Last();
            if (!full_paths.Contains(file))
            {
                full_paths.Add(file);
                paths.Add(p);
            }
        }
        private void DelFile(int index)
        {
            full_paths.RemoveAt(index);
            paths.RemoveAt(index);
        }

        private void filesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private List<string> full_paths = new List<string>();
        private List<string> paths = new List<string>();


        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFiles.SelectedIndex >= 0)
            {
                txtInfo.Text = "File (Double click to open): " + full_paths[lstFiles.SelectedIndex];
            }
        }

        private void LockControls(bool unlock = false)
        {
            panelFileControl.Enabled = unlock;
        }

        private void UnlockControls()
        {
            LockControls(true);
        }

        TreeNode BrowserRoot = new TreeNode();
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                LockControls();
                txtLogs.Clear();
                _docEngine.Generate(full_paths.ToArray());
                UnlockControls();
            } catch (Exception ex)
            {
                log(Program.ipy.getTraceback(ex).ToString());
            }
            
        }

        private void lstFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lstFiles.SelectedIndex >= 0)
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "explorer";
                p.StartInfo.Arguments = full_paths[lstFiles.SelectedIndex];
                p.Start();
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            var x = lstFiles.SelectedItems;
            int i = 0;
            foreach (int item in lstFiles.SelectedIndices)
            {
                DelFile(item);
                i = item;
            }
            updateList();
            if (i >= lstFiles.Items.Count) i = 0;
            if (lstFiles.Items.Count > 0) lstFiles.SelectedIndex = i;
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            full_paths.Clear();
            paths.Clear();
            updateList();
        }





        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
#if DEBUG
            fd.SelectedPath = @"F:\Circuit\Projects\UWAC\UWACSim\DociPy\DociPy\bin\Debug\test";
#endif
            fd.RootFolder = Environment.SpecialFolder.MyComputer;
            fd.Description = "Select a folder to look for python scripts";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DialogResult d = MessageBox.Show("Search inside subdirectories also?", "Recurse?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                string[] files = new string[] { };
                if (d == System.Windows.Forms.DialogResult.Yes)
                {
                    files = System.IO.Directory.GetFiles(fd.SelectedPath, "*.py", System.IO.SearchOption.AllDirectories);
                }
                else
                {
                    files = System.IO.Directory.GetFiles(fd.SelectedPath, "*.py", System.IO.SearchOption.TopDirectoryOnly);
                }
                lstFiles.Items.Clear();
                full_paths.Clear();
                paths.Clear();
                foreach (string item in files)
                {
                    AddFile(item, fd.SelectedPath);
                }

                updateList();
            }
            _docEngine.RootDirectory = fd.SelectedPath;
            propGrid.Refresh();
        }

        private void cmbThemes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }






        private AbstractDocEngine _docEngine; 
        private void cmbDocEngines_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDocEngines.SelectedIndex >= 0)
            {
                string engine_name =  "Engine" + cmbDocEngines.Text;
                object engine_obj = Program.ipy.EngineScope .GetVariable(engine_name);
                object instance = Program.ipy.Engine.Runtime.Operations.CreateInstance(engine_obj);
                if (typeof(AbstractDocEngine).IsAssignableFrom(instance.GetType()))
                {
                    _docEngine = (AbstractDocEngine)instance;
                    _docEngine.NewLogMessage += _docEngine_NewLogMessage;
                    propGrid.SelectedObject = _docEngine;
                }                
            }
        }

        void _docEngine_NewLogMessage(object sender, string msg)
        {
            log(msg,false);
        }

        private void cmbThemes_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbThemes.SelectedIndex >= 0)
            {
                _docEngine.Theme = System.IO.Path.Combine(template_path, cmbThemes.SelectedItem.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }




    }



}
