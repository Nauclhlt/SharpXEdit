using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

namespace SharpXEdit
{
    /// <summary>
    /// Represents a caret
    /// </summary>
    public sealed class Caret
    {
        private int _line = 0;
        private int _column = 0;

        private TextArea _parent;

        /// <summary>
        /// Gets the line index of the caret
        /// </summary>
        public int Line
        {
            get => _line;
        }
        
        /// <summary>
        /// Gets the column index of the caret
        /// </summary>
        public int Column
        {
            get => _column;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Caret"/> with a parent text area
        /// </summary>
        /// <param name="parent"></param>
        public Caret( TextArea parent )
        {
            _parent = parent;
        }

        /// <summary>
        /// Gets the character index from the line and column index
        /// </summary>
        /// <returns>character index</returns>
        public int GetCharIndex()
        {
            return _parent.Document.Cache.GetCharIndex(_line, _column);
        }

        /// <summary>
        /// Sets the indices from the character index
        /// </summary>
        /// <param name="index">character index</param>
        public void SetCharIndex( int index )
        {
            TextPoint pt = _parent.Document.Cache.GetPoint(index);
            _line = pt.Line;
            _column = pt.Column;
        }

        /// <summary>
        /// Sets the line and column index of the instance
        /// </summary>
        /// <param name="line">line index</param>
        /// <param name="column">column index</param>
        public void Set( int line, int column )
        {
            _line = line;
            _column = column;
        }

        /// <summary>
        /// Offsets the line and column index of the instance
        /// </summary>
        /// <param name="line">line index offset</param>
        /// <param name="column">column index offset</param>
        public void Offset( int line, int column )
        {
            _line += line;
            _column += column;
        }

        /// <summary>
        /// Gets an object which has its properties with same values as this object
        /// </summary>
        /// <returns></returns>
        public Caret GetClone()
        {
            Caret clone = new Caret(_parent);
            clone.Set(_line, _column);
            return clone;
        }

        /// <summary>
        /// Checks if the specified object indicates the same position as this object
        /// </summary>
        /// <param name="obj">obj to compare with</param>
        /// <returns></returns>
        public bool HasSamePosition( Caret obj )
        {
            return obj.Line == _line && obj.Column == _column;
        }
    }
}
