using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;

namespace SyntaxHighlighter
{
    public class SyntaxRichTextBox : System.Windows.Forms.RichTextBox
    {
        private SyntaxSettings m_settings = new SyntaxSettings();
        private static bool m_bPaint = true;
        private string m_strLine = "";
        private int m_nContentLength = 0;
        private int m_nLineLength = 0;
        private int m_nLineStart = 0;
        private int m_nLineEnd = 0;
        private string m_strKeywords = "";
        private string m_strBlockWords = "";
        private int m_nCurSelection = 0;




        private int _cur_pos = 1;
        private int _cur_lin = 1;
        private string _selWord = "";

        public event EventHandler<EventArgs> CursorPositionChanged;
        private string updateCurrentWord()
        {
            int CurrentPos = this.SelectionStart;
            int StartPos = CurrentPos;
            int EndPos = this.Text.ToString().IndexOf(" ", StartPos);
            if (EndPos < 0) EndPos = this.Text.Length;
            StartPos = this.Text.LastIndexOf(" ", CurrentPos);
            if (StartPos < 0) StartPos = 0;
            _selWord = this.Text.Substring(StartPos, EndPos - StartPos).Trim();
            if (_selWord.Contains("\n"))
            {
                int nl_pos = _selWord.IndexOf('\n');
                int word_pos = this.SelectionStart - StartPos - 1;
                if (nl_pos >= word_pos)
                {
                    _selWord = this.Text.Substring(StartPos, nl_pos + 1).Trim();
                }
                else
                {
                    _selWord = _selWord.Substring(this.Text.IndexOf('\n', StartPos) - StartPos).Trim();
                }
            }
            else if (this.SelectionStart == this.TextLength)
            {
                StartPos = this.Text.LastIndexOf(" ", this.TextLength);
                if (StartPos > 0)
                {
                    _selWord = this.Text.Substring(StartPos).Trim();
                }
            }
            return _selWord;

        }
        private void lineIndexUpdate(object s, EventArgs e)
        {
            _cur_lin = this.GetLineFromCharIndex(this.SelectionStart) + 1;
            _cur_pos = this.SelectionStart + 1;
            updateCurrentWord();
            if (CursorPositionChanged != null) CursorPositionChanged(s, new EventArgs());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SyntaxRichTextBox()
        {
            this.Click += lineIndexUpdate;
            this.DoubleClick += lineIndexUpdate;
            this.KeyDown += lineIndexUpdate;
        }

        public bool isModifyingKey(KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) ||
                  (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) ||
                    (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) ||
                       (e.Control && e.KeyCode == Keys.V) || (e.Control && e.KeyCode == Keys.X) ||
                       e.KeyCode == Keys.Space || (e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
            {
                return true;
            }
            return false;
        }

        public int CharIndex
        {
            get { return _cur_pos; }
            set { this.SelectionStart = (int)value; }
        }
        public int LineIndex
        {
            get { return _cur_lin; }
            set { this.SelectionStart = this.GetFirstCharIndexFromLine(_cur_lin - 1); }
        }

        public string SelectedLine
        {
            get
            {
                if (LineIndex >= 1 && Lines.Length > 0)
                    return this.Lines[LineIndex - 1];
                return "";
            }
        }

        public string SelectedWord { get { return _selWord; } }


        /// <summary>
        /// The settings.
        /// </summary>
        public SyntaxSettings Settings
        {
            get { return m_settings; }
        }

        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x00f)
            {
                if (m_bPaint)
                    base.WndProc(ref m);
                else
                    m.Result = IntPtr.Zero;
            }
            else
                base.WndProc(ref m);
        }
        /// <summary>
        /// OnTextChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            // Calculate shit here.
            m_nContentLength = this.TextLength;

            int nCurrentSelectionStart = SelectionStart;
            int nCurrentSelectionLength = SelectionLength;

            m_bPaint = false;

            // Find the start of the current line.
            m_nLineStart = nCurrentSelectionStart;
            while ((m_nLineStart > 0) && (Text[m_nLineStart - 1] != '\n'))
                m_nLineStart--;
            // Find the end of the current line.
            m_nLineEnd = nCurrentSelectionStart;
            while ((m_nLineEnd < Text.Length) && (Text[m_nLineEnd] != '\n'))
                m_nLineEnd++;
            // Calculate the length of the line.
            m_nLineLength = m_nLineEnd - m_nLineStart;
            // Get the current line.
            m_strLine = Text.Substring(m_nLineStart, m_nLineLength);

            // Process this line.
            ProcessLine();

            m_bPaint = true;
        }
        /// <summary>
        /// Process a line.
        /// </summary>
        private void ProcessLine()
        {
            // Save the position and make the whole line black
            int nPosition = SelectionStart;
            SelectionStart = m_nLineStart;
            SelectionLength = m_nLineLength;
            SelectionColor = Color.Black;

            // Process the keywords
            ProcessRegex(m_strKeywords, Settings.KeywordColor);
            //ProcessRegex(m_strBlockWords, Settings.BlockWordColor); 
            // Process numbers
            if (Settings.EnableIntegers)
                ProcessRegex("\\b(?:[0-9]*\\.)?[0-9]+\\b", Settings.IntegerColor);
            // Process strings
            if (Settings.EnableStrings)
                ProcessRegex("\"[^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*\"", Settings.StringColor);
            // Process comments
            if (Settings.EnableComments && !string.IsNullOrEmpty(Settings.Comment))
                ProcessRegex(Settings.Comment + ".*$", Settings.CommentColor);

            SelectionStart = nPosition;
            SelectionLength = 0;
            SelectionColor = Color.Black;

            m_nCurSelection = nPosition;
        }
        /// <summary>
        /// Process a regular expression.
        /// </summary>
        /// <param name="strRegex">The regular expression.</param>
        /// <param name="color">The color.</param>
        private void ProcessRegex(string strRegex, Color color)
        {
            Regex regKeywords = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match regMatch;

            for (regMatch = regKeywords.Match(m_strLine) ; regMatch.Success ; regMatch = regMatch.NextMatch())
            {
                // Process the words
                int nStart = m_nLineStart + regMatch.Index;
                int nLenght = regMatch.Length;
                SelectionStart = nStart;
                SelectionLength = nLenght;
                SelectionColor = color;
            }
        }
        /// <summary>
        /// Compiles the keywords as a regular expression.
        /// </summary>
        public void CompileKeywords()
        {
            for (int i = 0 ; i < Settings.Keywords.Count ; i++)
            {
                string strKeyword = Settings.Keywords[i];

                if (i == Settings.Keywords.Count - 1)
                    m_strKeywords += "\\b" + strKeyword + "\\b";
                else
                    m_strKeywords += "\\b" + strKeyword + "\\b|";
            }
            for (int i = 0 ; i < Settings.BlockWords.Count ; i++)
            {
                string strKeyword = Settings.BlockWords[i];

                if (i == Settings.BlockWords.Count - 1)
                    m_strBlockWords += "\\b" + strKeyword + "\\b";
                else
                    m_strBlockWords += "\\b" + strKeyword + "\\b|";
            }
        }





        public void ProcessAllLines()
        {
            m_bPaint = false;

            int nStartPos = 0;
            int i = 0;
            int nOriginalPos = SelectionStart;
            while (i < Lines.Length)
            {
                m_strLine = Lines[i];
                m_nLineStart = nStartPos;
                m_nLineEnd = m_nLineStart + m_strLine.Length;

                ProcessLine();
                i++;

                nStartPos += m_strLine.Length + 1;
            }

            m_bPaint = true;
        }
    }

    /// <summary>
    /// Class to store syntax objects in.
    /// </summary>
    public class SyntaxList
    {
        public List<string> m_rgList = new List<string>();
        public Color m_color = new Color();
    }

    /// <summary>
    /// Settings for the keywords and colors.
    /// </summary>
    public class SyntaxSettings
    {
        SyntaxList m_rgKeywords = new SyntaxList();
        SyntaxList m_rgBlockwords = new SyntaxList();
        string m_strComment = "";
        Color m_colorComment = Color.Green;
        Color m_colorString = Color.Gray;
        Color m_colorInteger = Color.Red;
        bool m_bEnableComments = true;
        bool m_bEnableIntegers = true;
        bool m_bEnableStrings = true;

        #region Properties
        /// <summary>
        /// A list containing all keywords.
        /// </summary>
        public List<string> Keywords
        {
            get { return m_rgKeywords.m_rgList; }
        }

        public List<string> BlockWords
        {
            get { return m_rgBlockwords.m_rgList; }
        }

        public Color BlockWordColor
        {
            get { return m_rgBlockwords.m_color; }
            set { m_rgBlockwords.m_color = value; }
        }
        /// <summary>
        /// The color of keywords.
        /// </summary>
        public Color KeywordColor
        {
            get { return m_rgKeywords.m_color; }
            set { m_rgKeywords.m_color = value; }
        }
        /// <summary>
        /// A string containing the comment identifier.
        /// </summary>
        public string Comment
        {
            get { return m_strComment; }
            set { m_strComment = value; }
        }
        /// <summary>
        /// The color of comments.
        /// </summary>
        public Color CommentColor
        {
            get { return m_colorComment; }
            set { m_colorComment = value; }
        }
        /// <summary>
        /// Enables processing of comments if set to true.
        /// </summary>
        public bool EnableComments
        {
            get { return m_bEnableComments; }
            set { m_bEnableComments = value; }
        }
        /// <summary>
        /// Enables processing of integers if set to true.
        /// </summary>
        public bool EnableIntegers
        {
            get { return m_bEnableIntegers; }
            set { m_bEnableIntegers = value; }
        }
        /// <summary>
        /// Enables processing of strings if set to true.
        /// </summary>
        public bool EnableStrings
        {
            get { return m_bEnableStrings; }
            set { m_bEnableStrings = value; }
        }
        /// <summary>
        /// The color of strings.
        /// </summary>
        public Color StringColor
        {
            get { return m_colorString; }
            set { m_colorString = value; }
        }
        /// <summary>
        /// The color of integers.
        /// </summary>
        public Color IntegerColor
        {
            get { return m_colorInteger; }
            set { m_colorInteger = value; }
        }
        #endregion
    }
}
