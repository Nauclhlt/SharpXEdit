using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace SharpXEdit
{
    /// <summary>
    /// Provides source code rendering features
    /// </summary>
    public sealed class SourceCodeManager : IDisposable
    {
        private TextArea _textArea;
        private Document _document;
        private bool _disposed = false;

        internal SourceCodeManager( Document document )
        {
            _textArea = document.Parent;
            _document = document;

            //_graphics = _textArea.CreateGraphics();
        }

        /// <summary>
        /// Gets the width of the specified string
        /// </summary>
        /// <param name="value">value to measure</param>
        /// <returns>width</returns>
        public int GetWidth( string value )
        {
            value = value.Replace("\t", "    ");
            return TextRenderer.MeasureText(value, _textArea.Font, new Size(int.MaxValue, int.MaxValue), Util.Shared.TextFormatFlags).Width;
        }

        /// <summary>
        /// Gets the width of the specified string
        /// </summary>
        /// <param name="span">value to measure</param>
        /// <returns>width</returns>
        public int GetWidth( ReadOnlySpan<char> span )
        {
            return GetWidth(new string(span));
        }

        /// <summary>
        /// Gets the top y coordinate of the specified line
        /// </summary>
        /// <param name="lineIndex">line index</param>
        /// <returns>top</returns>
        public int GetLineTop( int lineIndex )
        {
            return lineIndex * _document.Parent.FontHeight - _document.Scroll.Vertical;
        }

        /// <summary>
        /// Gets the control point of the caret
        /// </summary>
        /// <param name="caret">caret</param>
        /// <returns>point</returns>
        public Point GetCaretPoint( Caret caret )
        {
            return GetScreenPoint(new TextPoint(caret.Line, caret.Column));
        }

        /// <summary>
        /// Gets the screen point of the specified text point
        /// </summary>
        /// <param name="point">text point</param>
        /// <returns>point</returns>
        public Point GetScreenPoint( TextPoint point )
        {
            string line = _document.Cache.GetLineText(point.Line);
            int width = GetWidth(line.Substring(0, point.Column));

            return new Point(GetTextLeft() + width - _document.Scroll.Horizontal, GetLineTop(point.Line));
        }

        /// <summary>
        /// Gets the top line
        /// </summary>
        /// <returns>top line</returns>
        public int GetLineStart()
        {
            return _document.Scroll.Vertical / _textArea.FontHeight;
        }

        /// <summary>
        /// Gets the line count
        /// </summary>
        /// <returns>line count</returns>
        public int GetVisibleLineCount()
        {
            return _textArea.Height / _textArea.FontHeight + 1;
        }

        /// <summary>
        /// Gets the text left x coordinate
        /// </summary>
        /// <returns>x</returns>
        public int GetTextLeft()
        {
            return _textArea.LineNumberWidth + Util.Shared.LeftMargin;
        }

        internal Rectangle GetLineRefreshBounds(int line)
        {
            return new Rectangle(_textArea.LineNumberWidth + Util.Shared.LeftMargin, line * _textArea.FontHeight - _document.Scroll.Vertical, _textArea.Width - _textArea.LineNumberWidth - Util.Shared.LeftMargin, _textArea.FontHeight);
        }

        /// <summary>
        /// Releases all resources used by this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose( bool disposing )
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                _disposed = true;
            }
        }
    }
}
