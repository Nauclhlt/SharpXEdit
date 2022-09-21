using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Security.Policy;

namespace SharpXEdit
{
    /// <summary>
    /// Represents line-feed codes
    /// </summary>
    public enum LineFeedCode
    {
        /// <summary>
        /// Carriage return
        /// </summary>
        CR,
        /// <summary>
        /// Line feed
        /// </summary>
        LF,
        /// <summary>
        /// Carriage return and line feed
        /// </summary>
        CRLF
    }

    internal static class LineFeedCodeExtensionMethods
    {
        public static string GetCode( this LineFeedCode code )
        {
            if (code == LineFeedCode.CR)
                return "\r";
            if (code == LineFeedCode.LF)
                return "\n";
            if (code == LineFeedCode.CRLF)
                return "\r\n";

            return "NODATA";
        }
    }
}
