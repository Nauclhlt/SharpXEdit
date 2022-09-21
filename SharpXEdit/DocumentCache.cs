using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.ComponentModel;

namespace SharpXEdit
{
    /// <summary>
    /// Represents a cache data for a document
    /// </summary>
    public sealed class DocumentCache
    {
        private string[] _lines;
        private Document _parent;
        private int _lineCount;
        private string _text;
        private Dictionary<int, int> _lineHeadMap = new Dictionary<int, int>();
        private int _longestLine;

        internal DocumentCache( Document document )
        {
            _parent = document;

            string code = document.LineFeedCode.GetCode();
            string[] lines = document.Text.Split(code);

            _lines = lines;
            _lineCount = lines.Length;

            int max = 0;
            int maxIndex = 0;
            int pos = 0;
            for (int i = 0; i < lines.Length + 1; i++)
            {
                _lineHeadMap.Add(i, pos);
                if (i == lines.Length)
                    continue;
                if (_lines[i].Length > max)
                {
                    max = _lines[i].Length;
                    maxIndex = i;
                }
                pos += lines[i].Length + code.Length;
            }
            _text = document.Text;
            _longestLine = maxIndex;
        }

        /// <summary>
        /// Gets the parent document
        /// </summary>
        public Document Parent => _parent;
        /// <summary>
        /// Gets the number of the lines
        /// </summary>
        public int LineCount => _lineCount;
        /// <summary>
        /// Gets the index of the longest line
        /// </summary>
        public int LongestLine => _longestLine;

        /// <summary>
        /// Gets the text content of the specified line index
        /// </summary>
        /// <param name="line">line index</param>
        /// <returns>line text</returns>
        public string GetLineText( int line )
        {
            if (line < 0 || line >= _lineCount)
                return "";
            else
                return _lines[line];
        }

        /// <summary>
        /// Gets the first character index of the specified line
        /// </summary>
        /// <param name="line">line index</param>
        /// <returns>line head index</returns>
        public int GetLineHead( int line )
        {
            if (line < 0 || line >= _lineCount)
                return 0;
            else
                return _lineHeadMap[line];
        }

        /// <summary>
        /// Gets the line index of the line which contains the specified character index
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <returns>line index</returns>
        public int GetLineIndex( int charIndex )
        {
            if (charIndex < 0 || charIndex >= _text.Length)
                return 0;
            string code = _parent.LineFeedCode.GetCode();
            int pos = 0;
            for (int i = 0; i < _lines.Length; i++)
            {
                pos += _lines[i].Length + code.Length;
                if (charIndex < pos)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets the column index of the column which contains the specified character index
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <returns>column index</returns>
        public int GetColumnIndex( int charIndex )
        {
            if (charIndex < 0 || charIndex >= _text.Length)
                return 0;
            int line = GetLineIndex(charIndex);
            return charIndex - _lineHeadMap[line];
        }

        /// <summary>
        /// Gets a <see cref="TextPoint"/> structure which contains the line and column index of the specified character index
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <returns>text point</returns>
        public TextPoint GetPoint( int charIndex )
        {
            int line = GetLineIndex(charIndex);
            int col = charIndex - _lineHeadMap[line];
            return new TextPoint(line, col);
        }

        /// <summary>
        /// Gets the character index from the line and column index
        /// </summary>
        /// <param name="line">line index</param>
        /// <param name="column">column index</param>
        /// <returns>character index</returns>
        public int GetCharIndex( int line, int column )
        {
            return GetLineHead(line) + column;
        }

        /// <summary>
        /// Gets the character index from the specified text point
        /// </summary>
        /// <param name="point">text point</param>
        /// <returns>character index</returns>
        public int GetCharIndex( TextPoint point )
        {
            return GetCharIndex(point.Line, point.Column);
        }
    }
}
