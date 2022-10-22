using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SharpXEdit
{
    /// <summary>
    /// Represents a theme for a text area
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class TextAreaTheme
    {
        private Color _backgroundColor = Color.White;
        private Color _textColor = Color.Black;
        private Color _caretColor = Color.Black;
        private Color _lineNumberColor = Color.Gray;
        private Color _lineNumberBackgroundColor = Color.White;
        private Color _caretLineNumberColor = Color.Gray;
        private Color _caretLineNumberBackgroundColor = Color.White;
        private Color _bracketHighlightColor = Color.FromArgb(210, 210, 210);
        private Color _selectionColor = Color.FromArgb(128, Color.SkyBlue);

        /// <summary>
        /// Background color of a text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Background color of a text area")]
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        /// <summary>
        /// Text color of a text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Text color of a text area")]
        public Color TextColor
        {
            get => _textColor;
            set => _textColor = value;
        }

        /// <summary>
        /// Caret color of a text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Caret color of a text area")]
        public Color CaretColor
        {
            get => _caretColor;
            set => _caretColor = value;
        }

        /// <summary>
        /// Line number color of a text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Line number color of a text area")]
        public Color LineNumberColor
        {
            get => _lineNumberColor;
            set => _lineNumberColor = value;
        }

        /// <summary>
        /// Background color of line numbers of a text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Background color of line numbers of a text area")]
        public Color LineNumberBackgroundColor
        {
            get => _lineNumberBackgroundColor;
            set => _lineNumberBackgroundColor = value;
        }

        /// <summary>
        /// Line number color of a line which contains the caret of a text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Line number color of a line which contains the caret of a text area")]
        public Color CaretLineNumberColor
        {
            get => _caretLineNumberColor;
            set => _caretLineNumberColor = value;
        }

        /// <summary>
        /// Background color of line number of a line which contains the caret of text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Background color of line number of a line which contains the caret of a text area")]
        public Color CaretLineNumberBackgroundColor
        {
            get => _caretLineNumberBackgroundColor;
            set => _caretLineNumberBackgroundColor = value;
        }

        /// <summary>
        /// Bracket highlighting color of a text area
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Bracket highlighting color of a text area")]
        public Color BracketHighlightColor
        {
            get => _bracketHighlightColor;
            set => _bracketHighlightColor = value;
        }

        /// <summary>
        /// Background color of the range of selection
        /// </summary>
        [Category("SharpXEdit")]
        [Description("Background color of the range of selection")]
        public Color SelectionColor
        {
            get => _selectionColor;
            set => _selectionColor = value;
        }
    }
}
