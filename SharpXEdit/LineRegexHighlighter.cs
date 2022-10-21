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

namespace SharpXEdit
{
    /// <summary>
    /// Provides a regex highlighting process
    /// </summary>
    public sealed class LineRegexHighlighter : SourceCodeHighlighter
    {
        private CharStyle _style;
        private Regex _regex;

        /// <summary>
        /// Creates new object
        /// </summary>
        /// <param name="style"></param>
        /// <param name="regex"></param>
        public LineRegexHighlighter( CharStyle style, Regex regex )
        {
            _style = style;
            _regex = regex;
        }

        /// <summary>
        /// Runs highlighting
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
