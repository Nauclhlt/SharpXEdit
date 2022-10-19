using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace SharpXEdit
{
    /// <summary>
    /// Provides a highlighter class for keywords
    /// </summary>
    public sealed class KeywordsHighlighter : SourceCodeHighlighter
    {
        private CharStyle _style;
        private Regex _regex;
        
        /// <summary>
        /// Creates new object
        /// </summary>
        /// <param name="style"></param>
        /// <param name="wordUnit"></param>
        /// <param name="keywords"></param>
        /// <exception cref="ArgumentException"></exception>
        public KeywordsHighlighter( CharStyle style, bool wordUnit, params string[] keywords )
        {
            StringBuilder regexBuilder = new StringBuilder();
            
            if (keywords.Length == 0)
            {
                throw new ArgumentException("The specified keywords array is empty");
            }

            regexBuilder.Append('(');
            regexBuilder.Append(keywords[0]);
            for (int i = 1; i < keywords.Length; i++)
            {
                regexBuilder.Append('|');
                regexBuilder.Append(Regex.Escape(keywords[i]));
            }
            regexBuilder.Append(')');

            regexBuilder.Insert(0, "\\b");
            regexBuilder.Append("\\b");

            _style = style;
            _regex = new Regex(regexBuilder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEndPlus1"></param>
        public override void Run(Document doc, int lineStart, int lineEndPlus1)
        {
            for (int i = lineStart; i < lineEndPlus1; i++)
            {
                int head = doc.Cache.GetLineHead(i);
                string line = doc.Cache.GetLineText(i);
                MatchCollection matches = _regex.Matches(line);
                for (int m = 0; m < matches.Count; m++)
                {
                    Match match = matches[m];

                    doc.SourceCodeColor.SetRangeColorIfUndefMetaIsNot(head + match.Index, match.Length, SrcCodeMeta.ENCLOSURE_DEFINED, _style);
                }
            }
        }
    }
}
