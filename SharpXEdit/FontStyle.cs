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
    /// Represents font styles
    /// </summary>
    public enum FontStyle
    {
        /// <summary>
        /// Normal font
        /// </summary>
        Normal,
        /// <summary>
        /// Bold font
        /// </summary>
        Bold,
        /// <summary>
        /// Italic font
        /// </summary>
        Italic,
        /// <summary>
        /// Underlined font
        /// </summary>
        Underline,
        /// <summary>
        /// Font striked out
        /// </summary>
        Strikeout
    }
}
