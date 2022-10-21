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
using System.Diagnostics;

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

        public static bool IsOpenBracket( char c )
        {
            return c == '(' || c == '{' || c == '[';
        }

        public static bool IsCloseBracket( char c )
        {
            return c == ')' || c == '}' || c == ']';
        }

        public static char GetPairBracket(char c)
        {
            if (c == '[')
                return ']';
            if (c == '(')
                return ')';
            if (c == '{')
                return '}';

            if (c == ']')
                return '[';
            if (c == ')')
                return '(';
            if (c == '}')
                return '{';

            return ' ';
        }

        public static int SearchOpenBracket(string text, int index, string open, string close)
        {
            int pos = index;
            Stack<int> closeIndices = new Stack<int>();
            closeIndices.Push(index);
            for (; ; )
            {
                if (pos < 0)
                    return -1;

                int openIndex = text.LastIndexOf(open, pos);
                int closeIndex = text.LastIndexOf(close, pos);
                int closest = Math.Max(openIndex, closeIndex);

                if (openIndex == -1 && closeIndex == -1)
                {
                    if (closeIndices.Count > 0)
                        return -1;
                    else
                        return index;
                }

                if (openIndex != -1 && closest == openIndex)
                {
                    int stackIndex = closeIndices.Any() ? closeIndices.Pop() : -1;
                    pos = openIndex - 1;

                    if (stackIndex == index)
                    {
                        return openIndex;
                    }
                }

                if (closeIndex != -1 && closest == closeIndex)
                {
                    closeIndices.Push(closeIndex);
                    pos = closeIndex - 1;
                }
            }
        }

        public static int SearchCloseBracket(string text, int index, string open, string close)
        {
            int pos = index + 1;
            Stack<int> openIndices = new Stack<int>();
            openIndices.Push(index);
            for (; ; )
            {
                if (pos >= text.Length)
                    return -1;

                int openIndex = text.IndexOf(open, pos);
                int closeIndex = text.IndexOf(close, pos);
                int closest = Math.Min(openIndex == -1 ? int.MaxValue : openIndex, closeIndex == -1 ? int.MaxValue : closeIndex);

                if (openIndex == -1 && closeIndex == -1)
                {
                    if (openIndices.Count > 0)
                        return -1;
                    else
                        return index;
                }

                if (openIndex != -1 && closest == openIndex)
                {
                    openIndices.Push(openIndex);
                    pos = openIndex + 1;
                }

                if (closeIndex != -1 && closest == closeIndex)
                {
                    int stackIndex = openIndices.Any() ? openIndices.Pop() : -1;
                    pos = closeIndex + 1;

                    if (stackIndex == index)
                    {
                        return closeIndex;
                    }
                }
            }
        }
    }
}
