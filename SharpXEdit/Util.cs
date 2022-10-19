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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace SharpXEdit
{
    internal static class Util
    {
        public static class Shared
        {
            public static readonly TextFormatFlags TextFormatFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;
            public static readonly int LeftMargin = 10;
            public static readonly int LineFeedSelectionWidth = 10;
            public static readonly HashSet<char> WorkBreaks = new HashSet<char>()
            {
                ',', '.', '/', '(', ')', '!', '"', '\'', '#', '$', '&', '=', '-', '^', '~', '|', '\\',
                '[', ']', '{', '}', ';', '+', '*', ':', '<', '>', '?', '_', ' ', '\t'
            };
        }

        public static unsafe char PtrChar( this string str, int index )
        {
            if (index < 0 || index >= str.Length)
                throw new IndexOutOfRangeException("Index must be in the bounds of the string");

            fixed (char* ptr = str)
            {
                return ptr[index];
            }
        }

        public static bool IsInRange( int value, int begin, int end )
        {
            return value >= begin && value <= end;
        }

        public static string GetIndentValue( string line )
        {
            unsafe
            {
                int tabs = 0;
                int spaces = 0;
                fixed(char* ptr = line)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (ptr[i] == '\t')
                        {
                            tabs++;
                            continue;
                        }
                        if (ptr[i] == ' ')
                        {
                            spaces++;
                            continue;
                        }

                        break;
                    }
                }

                return new string(' ', spaces) + new string('\t', tabs);
            }
        }

        public static bool IsWordBreak( char c )
        {
            return Shared.WorkBreaks.Contains(c);
        }
    }
}
