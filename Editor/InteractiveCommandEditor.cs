using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SimbedEnvisionCodeEditor
{
    public class CommandEnteredEventArgs : EventArgs
    {
        public string Command { get; set; }
        public bool Handled { get; set; }
        public bool SuppressEnter { get; set; }
        public CommandEnteredEventArgs(string command)
        {
            Command = command;
            Handled = false;
            SuppressEnter = true;
        }
    }
    public class InteractiveCommandEditor : SyntaxHighlighter.SyntaxRichTextBox
    {
        private int _lastEditablePos = 0;




        public void insertTermination(string txt = "")
        {
            if (txt != "")
            {
                this.AppendText(txt);
            }
            if (!this.Text.EndsWith("\n") && !(this.Text == ""))
            {
                this.AppendText("\n");
            }
            this.AppendText(this.Prompt);
        }

        private void _handleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (_lastEditablePos > this.SelectionStart)
                {
                    this.SelectionStart = this.TextLength;
                }
                string word = this.Text.Substring(_lastEditablePos, this.SelectionStart - _lastEditablePos);
                if (CommandEntered != null)
                {
                    CommandEnteredEventArgs _e = new CommandEnteredEventArgs(word);
                    CommandEntered(this, _e);
                    if (!_e.Handled)
                    {
                        insertTermination();
                    }
                    e.Handled = _e.SuppressEnter;
                }
                else
                {
                    insertTermination();
                }
                _lastEditablePos = this.TextLength;

            }
            else if (isModifyingKey(e) && (_lastEditablePos > this.SelectionStart))
            {
                e.SuppressKeyPress = true;
            }
        }



        public event EventHandler<CommandEnteredEventArgs> CommandEntered;

        public InteractiveCommandEditor()
        {
            this.KeyDown += _handleKeyDown;
        }


        public void init()
        {
            insertTermination();
            _lastEditablePos = this.TextLength;
            this.SelectionStart = _lastEditablePos;
        }

        public string Prompt { get; set; }

        public void EmulateCommandEntry(string word)
        {
            if (CommandEntered != null)
            {
                AppendText(word);
                CommandEnteredEventArgs _e = new CommandEnteredEventArgs(word);
                CommandEntered(this, _e);
                if (!_e.Handled)
                {
                    insertTermination();
                }
            }
        }

        public void _Cut()
        {
            if (_lastEditablePos <= this.SelectionStart)
            {
                this.Cut();
            }
        }
        public void _Paste()
        {
            if (_lastEditablePos <= this.SelectionStart)
            {
                this.Paste(DataFormats.GetFormat(DataFormats.Text));
            }
        }
    }
}
