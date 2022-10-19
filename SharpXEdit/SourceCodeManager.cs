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
    internal sealed class SourceCodeManager : IDisposable
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

        public int GetWidth( string value )
        {
            value = value.Replace("\t", "    ");
            return TextRenderer.MeasureText(value, _textArea.Font, new Size(int.MaxValue, int.MaxValue), Util.Shared.TextFormatFlags).Width;
        }

        public int GetWidth( ReadOnlySpan<char> span )
        {
            return GetWidth(new string(span));
        }

        public int GetLineTop( int lineIndex )
        {
            return lineIndex * _document.Parent.FontHeight - _document.Scroll.Vertical;
        }

        public Point GetCaretPoint( Caret caret )
        {
            string line = _document.Cache.GetLineText(caret.Line);
            int width = GetWidth(line.AsSpan(0, caret.Column));

            return new Point(GetTextLeft() + width, GetLineTop(caret.Line));
        }

        public int GetLineStart()
        {
            return _document.Scroll.Vertical / _textArea.FontHeight;
        }

        public int GetVisibleLineCount()
        {
            return _textArea.Height / _textArea.FontHeight + 2;
        }

        public int GetTextLeft()
        {
            return _textArea.LineNumberWidth + Util.Shared.LeftMargin;
        }

        public Rectangle GetLineRefreshBounds(int line)
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
