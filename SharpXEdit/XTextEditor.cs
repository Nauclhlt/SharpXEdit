using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpXEdit
{
    /// <summary>
    /// Provides a main text editor control
    /// </summary>
    public partial class XTextEditor : UserControl
    {
        private VScrollBar _vScrollBar;
        private HScrollBar _hScrollBar;


        private static readonly int ScrollBarSize = 20;


        /// <summary>
        /// Whether the vertical scroll bar is enabled
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Whether the vertical scroll bar is enabled")]
        public bool VScrollBar
        {
            get => _vScrollBar.Visible;
            set
            {
                _vScrollBar.Visible = value;

                if (value)
                    _textArea.Width = Width - ScrollBarSize;
                else
                    _textArea.Width = Width;
            }
        }

        /// <summary>
        /// Whether the horizontal scroll bar is enabled
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Whether the horizontal scroll bar is enabled")]
        public bool HScrollBar
        {
            get => _hScrollBar.Visible;
            set
            {
                _hScrollBar.Visible = value;

                if (value)
                    _textArea.Height = Height - ScrollBarSize;
                else
                    _textArea.Height = Height;
            }
        }

        /// <summary>
        /// Gets or sets the line number renderer
        /// </summary>
        [Browsable(false)]
        public LineNumberRenderer LineNumberRenderer
        {
            get => _textArea.LineNumberRenderer;
            set
            {
                _textArea.LineNumberRenderer = value;
            }
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        public XTextEditor()
        {
            InitializeComponent();
            _textArea.SetContainer(this);


            _vScrollBar = new VScrollBar();
            _hScrollBar = new HScrollBar();
            Controls.Add(_vScrollBar);
            Controls.Add(_hScrollBar);
            _vScrollBar.Visible = false;
            _hScrollBar.Visible = false;

            int size = ScrollBarSize;

            _vScrollBar.Location = new Point(Width - size, 0);
            _vScrollBar.Width = size;
            _vScrollBar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _vScrollBar.Height = Height - size;
            _vScrollBar.Scroll += VScrollBarChanged;

            _hScrollBar.Location = new Point(0, Height - size);
            _hScrollBar.Height = size;
            _hScrollBar.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            _hScrollBar.Width = Width;
            _hScrollBar.Scroll += HScrollBarChanged;

            _textArea.DocumentChanged += OnDocumentChanged;
            _textArea.DocumentTextChanged += OnTextChanged;

            UpdateScrollBars();
        }

        private void OnTextChanged( object sender, EventArgs e )
        {
            UpdateScrollBars();
        }

        private void OnDocumentChanged( object sender, EventArgs e )
        {
            UpdateScrollBars();
        }

        private void UpdateScrollBars()
        {
            _vScrollBar.Maximum = _textArea.Document.Cache.LineCount + 9;
            _vScrollBar.Enabled = _textArea.Document.Cache.LineCount != 1;
            _hScrollBar.Maximum = _textArea.Document.Cache.GetLineText(_textArea.Document.Cache.LongestLine).Length + 9;
            _hScrollBar.Enabled = _hScrollBar.Maximum != 0;
        }

        private void VScrollBarChanged( object sender, ScrollEventArgs e )
        {
            int value = e.NewValue;
            _textArea.Document.Scroll.ScrollToLine(value);
        }

        private void HScrollBarChanged( object sender, ScrollEventArgs e )
        {
            int value = e.NewValue;
            string longest = _textArea.Document.Cache.GetLineText(_textArea.Document.Cache.LongestLine);
            _textArea.Document.Scroll.Horizontal = _textArea.Document.SourceCodeManager.GetWidth(longest.Substring(0, value));
        }



        /// <summary>
        /// Gets the collection of highlighters
        /// </summary>
        [Browsable(false)]
        public HighlighterCollection Highlighters
        {
            get => _textArea.Highlighters;
        }

        /// <summary>
        /// Gets or sets the width of the line number area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the width of the line number area")]
        public int LineNumberWidth
        {
            get => _textArea.LineNumberWidth;
            set => _textArea.LineNumberWidth = value;
        }

        /// <summary>
        /// Gets or sets the current document of the text editor
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the current document of the text editor")]
        public Document Document
        {
            get => _textArea.Document;
            set => _textArea.Document = value;
        }

        /// <summary>
        /// Gets or sets the theme of the text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the theme of the text area")]
        public TextAreaTheme Theme
        {
            get => _textArea.Theme;
            set => _textArea.Theme = value;
        }

        /// <summary>
        /// Gets or sets the speed of scrolling
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the speed of scrolling")]
        public int ScrollSpeed
        {
            get => _textArea.ScrollSpeed;
            set => _textArea.ScrollSpeed = value;
        }

        /// <summary>
        /// Gets or sets the rendering font of the text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the rendering font of the text area")]
        public new Font Font
        {
            get => _textArea.Font;
            set => _textArea.Font = value;
        }

        /// <summary>
        /// Gets the height of the font
        /// </summary>
        [Browsable(false)]
        public new int FontHeight => _textArea.FontHeight;

        /// <summary>
        /// Gets the auto-complete source
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the source of auto complete")]
        public StringCollection AutoCompleteSource
        {
            get => _textArea.AutoCompleteSource;
        }

        /// <summary>
        /// Gets or sets the theme of auto complete
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the theme of auto complete")]
        public TextAreaTheme AutoCompleteTheme
        {
            get => _textArea.AutoCompleteTheme;
            set => _textArea.AutoCompleteTheme = value;
        }

        /// <summary>
        /// Gets or sets the text of the text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Gets or sets the text of the text area")]
        public override string Text 
        { 
            get => _textArea.Text; 
            set => _textArea.Text = value; 
        }

        /// <summary>
        /// Gets the text content of the specified line
        /// </summary>
        /// <param name="index">line index</param>
        /// <returns>text</returns>
        public string GetLine( int index )
        {
            if (index < 0 || index >= _textArea.Document.Cache.LineCount)
            {
                return null;
            }

            return _textArea.Document.Cache.GetLineText(index);
        }

        /// <summary>
        /// Gets the array which contains all lines the text area
        /// </summary>
        /// <returns>array</returns>
        public string[] GetLinesArray()
        {
            string[] lines = new string[_textArea.Document.Cache.LineCount];
            for (int i = 0; i < _textArea.Document.Cache.LineCount; i++)
            {
                lines[i] = _textArea.Document.Cache.GetLineText(i);
            }

            return lines;
        }

        /// <summary>
        /// Requests the text area to redraw fully
        /// </summary>
        public void FullRedraw()
        {
            _textArea.FullRedraw();
        }

        /// <summary>
        /// Pastes the text content of the clipboard
        /// </summary>
        public void Paste()
        {
            _textArea.Paste();
        }

        /// <summary>
        /// Copies the selected text to the clipboard
        /// </summary>
        public void Copy()
        {
            _textArea.Copy();
        }

        /// <summary>
        /// Cuts the selected text to the clipboard
        /// </summary>
        public void Cut()
        {
            _textArea.Cut();
        }

        /// <summary>
        /// Writes the specified value to the caret
        /// </summary>
        /// <param name="value">value to write to</param>
        public void Paste( string value )
        {
            _textArea.Paste(value);
        }

        /// <summary>
        /// Gets the reference for the internal text area control object
        /// </summary>
        /// <returns>text area object</returns>
        public object GetInternalTextAreaObj()
        {
            return _textArea;
        }
    }
}
