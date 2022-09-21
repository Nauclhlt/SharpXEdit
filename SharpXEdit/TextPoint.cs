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
    /// Represents a position which is expressed by a line index and column index
    /// </summary>
    public struct TextPoint
    {
        private int _line;
        private int _column;

        /// <summary>
        /// Gets the line index
        /// </summary>
        public int Line => _line;
        /// <summary>
        /// Gets the column index
        /// </summary>
        public int Column => _column;

        /// <summary>
        /// Initializes new structure with the line and column index
        /// </summary>
        /// <param name="line">line index</param>
        /// <param name="column">column index</param>
        public TextPoint( int line, int column )
        {
            _line = line;
            _column = column;
        }
    }
}
