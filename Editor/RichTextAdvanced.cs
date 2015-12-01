using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SimbedEnvisionCodeEditor
{



    public class IsIntelliTriggerEventArgs : EventArgs
    {
        public bool IntelliTrigger { get; set; }
        public KeyEventArgs KeyE { get; set; }
        public IsIntelliTriggerEventArgs(KeyEventArgs e, bool trigger = false)
        {
            this.KeyE = e;
            this.IntelliTrigger = trigger;
        }
    }
    public class IntelliRequestEventArgs : EventArgs
    {
        public List<string> Words = new List<string>();
        private KeyEventArgs _e;
        public KeyEventArgs KeyE
        {
            get { return _e; }
        }
        public IntelliRequestEventArgs(KeyEventArgs KeyE, ref List<string> words)
        {
            this.Words = words;
            _e = KeyE;
        }
    }

    public class IntelliFromatCompletionEventArgs : EventArgs
    {
        public string CompletionString { get; set; }
        public IntelliFromatCompletionEventArgs(string completion)
        {
            CompletionString = completion;
        }
    }

    public class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern void GetCaretPos(ref System.Drawing.Point pt);

        public static System.Drawing.Point GetCaretPos()
        {
            System.Drawing.Point pt = new System.Drawing.Point();
            GetCaretPos(ref pt);
            return pt;
        }

    }

    public class RichTextAdvanced : SyntaxHighlighter.SyntaxRichTextBox
    {


        #region "CodeCompletion"
        private ListBox _intelliBox = new ListBox();
        private bool intelli_en = true;
        private int trigger_pos = -1;
        private string search_key = "";
        private Keys _intelliKey = Keys.Tab;

        public event EventHandler<IsIntelliTriggerEventArgs> IsIntelliTrigger;
        public event EventHandler<IntelliRequestEventArgs> IntelliRequest;
        public event EventHandler<IntelliFromatCompletionEventArgs> IntelliFromatCompletion;
        private void _posIntelliBox()
        {
            System.Drawing.Point p = NativeMethods.GetCaretPos();
            this._intelliBox.Top = p.Y + 75;
            this._intelliBox.Left = p.X + 5;
        }

        private List<string> intelli_words = new List<string>();
        private void _handleNewIntelliRequest(KeyEventArgs e)
        {
            IsIntelliTriggerEventArgs _trigger = new IsIntelliTriggerEventArgs(e);
            if (IsIntelliTrigger != null)
            {
                IsIntelliTrigger(this, _trigger);
                if (_trigger.IntelliTrigger)
                {
                    e = _trigger.KeyE;
                    if (IntelliRequest != null)
                    {
                        intelli_words.Clear();
                        IntelliRequestEventArgs ex = new IntelliRequestEventArgs(e, ref intelli_words);
                        IntelliRequest(this, ex);
                        if (ex.Words.Count > 0)
                        {
                            trigger_pos = this.SelectionStart;
                            search_key = "";
                            _intelliBox.Items.Clear();
                            _intelliBox.Items.AddRange(ex.Words.ToArray());
                            _posIntelliBox();
                            _intelliBox.Show();
                        }
                        else
                        {
                            _intelliBox.Hide();
                        }
                    }
                }
            }

        }
        private void _handleKeyDown(object sender, KeyEventArgs e)
        {
            if (!_intelliBox.Visible && e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                this.SelectedText.Insert(0, "    ");
            }
            if (intelli_en)
            {
                if (e.KeyCode == _intelliKey && _intelliBox.Visible)
                {
                    if (_intelliBox.SelectedItem == null) _intelliBox.SelectedIndex = 0;
                    string p_string = _intelliBox.SelectedItem.ToString();
                    if (search_key.Length >= 0)
                    {
                        p_string = p_string.Substring(search_key.Length);
                    }
                    if (IntelliFromatCompletion != null)
                    {
                        IntelliFromatCompletionEventArgs _e = new IntelliFromatCompletionEventArgs(p_string);
                        IntelliFromatCompletion(this, _e);
                        p_string = _e.CompletionString;
                    }
                    this.SelectedText = this.SelectedText.Insert(0, p_string);
                    _intelliBox.Hide();
                    search_key = "";
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    _intelliBox.Hide();
                }
                else if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) ||
                            (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) ||
                               (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
                {
                    if (_intelliBox.Visible)
                    {
                        string c = ((char)e.KeyValue).ToString().ToLower();
                        if (e.Shift) c = c.ToUpper();
                        search_key = this.Text.Substring(trigger_pos + 1, this.SelectionStart - trigger_pos - 1) + c;
                        _intelliBox.Items.Clear();
                        _intelliBox.Items.AddRange((from object word in intelli_words
                                                    where word.ToString().StartsWith(search_key)
                                                    select word).ToArray());
                        if (_intelliBox.Items.Count > 0) _intelliBox.SelectedIndex = 0;
                        else _intelliBox.Hide();
                    }
                }
                else if (_intelliBox.Visible && e.KeyCode == Keys.Up)
                {
                    if (_intelliBox.SelectedIndex > 0)
                        _intelliBox.SelectedIndex -= 1;
                    else
                        _intelliBox.SelectedIndex = _intelliBox.Items.Count - 1;
                    e.Handled = true;
                }
                else if (_intelliBox.Visible && e.KeyCode == Keys.Down)
                {
                    if (_intelliBox.SelectedIndex < _intelliBox.Items.Count - 1)
                        _intelliBox.SelectedIndex += 1;
                    else
                        _intelliBox.SelectedIndex = 0;
                    e.Handled = true;
                }
                else if (this.SelectionStart < trigger_pos + 1)
                {
                    _intelliBox.Hide();
                }
                _handleNewIntelliRequest(e);
            }


        }

        public bool EnableCodeCompletion
        {
            get { return intelli_en; }
            set { intelli_en = value; }
        }

        public ListBox IntelliBox
        {
            get { return _intelliBox; }
            set { this._intelliBox = value; }
        }

        public Keys IntelliCompletionKey
        {
            get { return _intelliKey; }
            set { _intelliKey = value; }
        }

        #endregion

        public RichTextAdvanced()
        {
            this.KeyDown += this._handleKeyDown;
        }


    }
}
