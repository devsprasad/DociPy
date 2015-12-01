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

        private string template_path = "./data/";
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

        private void loadTemplates()
        {
            string path = System.IO.Path.Combine(template_path, "Templates");
            string[] files = System.IO.Directory.GetFiles(path, "*.html", System.IO.SearchOption.AllDirectories);
            foreach (string template in files)
            {
                System.IO.FileInfo f = new System.IO.FileInfo(template);
                string rel_template_path = template.Replace(template_path, "");
                string template_name = f.Name;
                cmbTemplates.Items.Add(rel_template_path);
            }
            if (cmbTemplates.Items.Count > 0) cmbTemplates.SelectedIndex = 0;
        }


        private DocumentationEngine docEngine = new DocumentationEngine();
        private void mainForm_Load(object sender, EventArgs e)
        {
            loadThemes(); loadTemplates();
            propGrid.SelectedObject = docEngine;
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
            if (root != null) p = file.Replace(root, ".");
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
            LockControls();
            txtLogs.Clear();
            BrowserRoot.Nodes.Clear();
            int fileCount = full_paths.Count;
            int methodCount = 0;
            int classCount = 0;
            Columbus cExplorer = new Columbus();
            //
            // start
            //            
            List<ScriptInfo> scripts = new List<ScriptInfo>();
            for (int i = 0 ; i < full_paths.Count ; i++)
            {
                lstFiles.SelectedIndex = i;
                string file_name = lstFiles.SelectedItem.ToString();
                string file_path = full_paths[i];
                TreeNode trvNode = BrowserRoot.Nodes.Add(file_name);
                log("processing file " + file_name + "...       ", chkDetReport.Checked);
                ScriptInfo scrinf = cExplorer.ProcessFile(file_path);
                TreeNode clsNode = trvNode.Nodes.Add("Classes");
                foreach (var item in scrinf.Class)
                {
                    clsNode.Nodes.Add(item.Value.Name);

                }
                TreeNode funcNode = trvNode.Nodes.Add("Functions");
                foreach (var item in scrinf.Functions)
                {
                    funcNode.Nodes.Add(item.Value.Name);

                }
                methodCount += scrinf.Functions.Count;
                classCount += scrinf.Class.Count;
                log("done");
                if (chkDetReport.Checked)
                {
                    log("===> Classes   : " + scrinf.Class.Count.ToString());
                    log("===> Functions : " + scrinf.Functions.Count.ToString());
                }
                scripts.Add(scrinf);
            }
            button2.Enabled = true;
            log("=====================================");
            log("Report: ");
            log(string.Format("Processed {0} methods from {1} classes from {2} files", methodCount, classCount, fileCount));
            docEngine.WriteHTML(scripts,_html,_css);
            UnlockControls();
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
            lstFiles.Items.Clear();
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
                foreach (string item in files)
                {
                    AddFile(item, fd.SelectedPath);
                }

                updateList();
            }
            docEngine.RootDirectory = fd.SelectedPath;

        }

        private void cmbThemes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (lstFiles.SelectedIndex > 0 )
            {
                string path = full_paths[lstFiles.SelectedIndex];
                string p = path.Replace(docEngine.RootDirectory, docEngine.OutputDirectory) + ".html";
                p = System.IO.Path.GetFullPath (p.Replace('\\', '/'));
                if (System.IO.File.Exists(p))
                {
                    p = "file:///" + p;
                    if (sender == button3)
                    {
                        //preview with browser;
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = "explorer";
                        proc.StartInfo.Arguments = p;
                        proc.Start();
                    }
                    else
                    {
                        frmPreview fp = new frmPreview(p);
                        fp.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("File not found. Have you forgotten to generate the documentation first?","Error",
                        MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        private string _css = "";
        private string _html = "";
        private void cmbTemplates_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbTemplates.SelectedIndex >= 0 && cmbThemes.SelectedIndex >= 0)
            {
                _css = System.IO.Path.Combine(Application.StartupPath, template_path, cmbThemes.SelectedItem.ToString());
                _html = System.IO.Path.Combine(template_path, cmbTemplates.SelectedItem.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DociPy.Explorers.PyColumbus pc = new Explorers.PyColumbus();
            pc.Process("./test/lib/builtins.py");
        }




    }



}
