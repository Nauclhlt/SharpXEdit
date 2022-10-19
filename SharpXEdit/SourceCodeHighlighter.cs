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
    /// Provides a base class for source code highlighters
    /// </summary>
    public abstract class SourceCodeHighlighter
    {
        /// <summary>
        /// Whether the highlighting process is final execution
        /// </summary>
        public bool Final
        {
            get;
            protected set;
        }

        /// <summary>
        /// Runs the highlighting process
        /// </summary>
        /// <param name="doc">document</param>
        /// <param name="lineStart">line start index</param>
        /// <param name="lineEndPlus1">line end index plus 1</param>
        public abstract void Run(Document doc, int lineStart, int lineEndPlus1);
    }
}
