namespace DociPy
{
    partial class mainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextAdd = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelFileControl = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lblCount = new System.Windows.Forms.ToolStripLabel();
            this.cmbThemes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.chkDetReport = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.propGrid = new System.Windows.Forms.PropertyGrid();
            this.cmbDocEngines = new System.Windows.Forms.ComboBox();
            this.ipyEdit = new SimbedEnvisionCodeEditor.InteractiveCommandEditor();
            this.contextAdd.SuspendLayout();
            this.panelFileControl.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextAdd
            // 
            this.contextAdd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFileToolStripMenuItem,
            this.addDirectoryToolStripMenuItem});
            this.contextAdd.Name = "contextAdd";
            this.contextAdd.Size = new System.Drawing.Size(148, 48);
            // 
            // addFileToolStripMenuItem
            // 
            this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
            this.addFileToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addFileToolStripMenuItem.Text = "Add &File";
            // 
            // addDirectoryToolStripMenuItem
            // 
            this.addDirectoryToolStripMenuItem.Name = "addDirectoryToolStripMenuItem";
            this.addDirectoryToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addDirectoryToolStripMenuItem.Text = "Add &Directory";
            // 
            // panelFileControl
            // 
            this.panelFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelFileControl.Controls.Add(this.panel2);
            this.panelFileControl.Controls.Add(this.toolStrip1);
            this.panelFileControl.Location = new System.Drawing.Point(12, 12);
            this.panelFileControl.Name = "panelFileControl";
            this.panelFileControl.Size = new System.Drawing.Size(201, 338);
            this.panelFileControl.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lstFiles);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(201, 313);
            this.panel2.TabIndex = 6;
            // 
            // lstFiles
            // 
            this.lstFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.HorizontalScrollbar = true;
            this.lstFiles.Location = new System.Drawing.Point(0, 0);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.ScrollAlwaysVisible = true;
            this.lstFiles.Size = new System.Drawing.Size(201, 316);
            this.lstFiles.TabIndex = 2;
            this.lstFiles.SelectedIndexChanged += new System.EventHandler(this.lstFiles_SelectedIndexChanged);
            this.lstFiles.DoubleClick += new System.EventHandler(this.lstFiles_DoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripSplitButton1,
            this.toolStripSeparator2,
            this.lblCount});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(201, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::DociPy.Properties.Resources.folder_classic_add_simple;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(77, 22);
            this.toolStripButton1.Text = "Add Root";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAllToolStripMenuItem});
            this.toolStripSplitButton1.Image = global::DociPy.Properties.Resources.remove_minus_sign;
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(82, 22);
            this.toolStripSplitButton1.Text = "Remove";
            this.toolStripSplitButton1.ButtonClick += new System.EventHandler(this.toolStripSplitButton1_ButtonClick);
            // 
            // removeAllToolStripMenuItem
            // 
            this.removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
            this.removeAllToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.removeAllToolStripMenuItem.Text = "Remove All";
            this.removeAllToolStripMenuItem.Click += new System.EventHandler(this.removeAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // lblCount
            // 
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(13, 22);
            this.lblCount.Text = "0";
            // 
            // cmbThemes
            // 
            this.cmbThemes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbThemes.FormattingEnabled = true;
            this.cmbThemes.Location = new System.Drawing.Point(216, 44);
            this.cmbThemes.Name = "cmbThemes";
            this.cmbThemes.Size = new System.Drawing.Size(185, 21);
            this.cmbThemes.TabIndex = 6;
            this.cmbThemes.SelectedIndexChanged += new System.EventHandler(this.cmbThemes_SelectedIndexChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(425, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Documentation Engines";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(544, 248);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 3;
            this.btnGo.Text = "&GO";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtLogs
            // 
            this.txtLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLogs.Location = new System.Drawing.Point(216, 277);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogs.Size = new System.Drawing.Size(403, 168);
            this.txtLogs.TabIndex = 2;
            // 
            // txtInfo
            // 
            this.txtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtInfo.Location = new System.Drawing.Point(12, 351);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(201, 94);
            this.txtInfo.TabIndex = 5;
            // 
            // chkDetReport
            // 
            this.chkDetReport.AutoSize = true;
            this.chkDetReport.Location = new System.Drawing.Point(216, 254);
            this.chkDetReport.Name = "chkDetReport";
            this.chkDetReport.Size = new System.Drawing.Size(95, 17);
            this.chkDetReport.TabIndex = 7;
            this.chkDetReport.Text = "Detailed report";
            this.chkDetReport.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(216, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Themes";
            // 
            // propGrid
            // 
            this.propGrid.HelpVisible = false;
            this.propGrid.Location = new System.Drawing.Point(216, 71);
            this.propGrid.Name = "propGrid";
            this.propGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propGrid.Size = new System.Drawing.Size(403, 168);
            this.propGrid.TabIndex = 10;
            this.propGrid.ToolbarVisible = false;
            // 
            // cmbDocEngines
            // 
            this.cmbDocEngines.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDocEngines.FormattingEnabled = true;
            this.cmbDocEngines.Location = new System.Drawing.Point(428, 44);
            this.cmbDocEngines.Name = "cmbDocEngines";
            this.cmbDocEngines.Size = new System.Drawing.Size(191, 21);
            this.cmbDocEngines.TabIndex = 6;
            this.cmbDocEngines.SelectedIndexChanged += new System.EventHandler(this.cmbDocEngines_SelectedIndexChanged);
            // 
            // ipyEdit
            // 
            this.ipyEdit.CharIndex = 1;
            this.ipyEdit.LineIndex = 1;
            this.ipyEdit.Location = new System.Drawing.Point(676, 71);
            this.ipyEdit.Name = "ipyEdit";
            this.ipyEdit.Prompt = ">> ";
            this.ipyEdit.Size = new System.Drawing.Size(229, 372);
            this.ipyEdit.TabIndex = 0;
            this.ipyEdit.Text = "";
            this.ipyEdit.Visible = false;
            this.ipyEdit.CommandEntered += new System.EventHandler<SimbedEnvisionCodeEditor.CommandEnteredEventArgs>(this.ipyEdit_CommandEntered);
            this.ipyEdit.TextChanged += new System.EventHandler(this.ipyEdit_TextChanged);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 455);
            this.Controls.Add(this.propGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbDocEngines);
            this.Controls.Add(this.cmbThemes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkDetReport);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtLogs);
            this.Controls.Add(this.panelFileControl);
            this.Controls.Add(this.ipyEdit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DociPy";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.contextAdd.ResumeLayout(false);
            this.panelFileControl.ResumeLayout(false);
            this.panelFileControl.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SimbedEnvisionCodeEditor.InteractiveCommandEditor ipyEdit;
        private System.Windows.Forms.ContextMenuStrip contextAdd;
        private System.Windows.Forms.ToolStripMenuItem addFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addDirectoryToolStripMenuItem;
        private System.Windows.Forms.Panel panelFileControl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem removeAllToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkDetReport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel lblCount;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ComboBox cmbThemes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PropertyGrid propGrid;
        private System.Windows.Forms.ComboBox cmbDocEngines;

    }
}

