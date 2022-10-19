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
    /// Provides a header highlighting process
    /// </summary>
    public sealed class HeaderHighlighter : SourceCodeHighlighter
    {
        private CharStyle _style;
        private string _header;
        private Regex _regex;
        private bool _self = true;

        /// <summary>
        /// Creates new object
        /// </summary>
        /// <param name="style"></param>
        /// <param name="header"></param>
        /// <param name="self"></param>
        /// <param name="replace"></param>
        public HeaderHighlighter( CharStyle style, string header, bool self = true, bool replace = true )
        {
            _style = style;
            _header = header;
            _self = self;

            _regex = new Regex(Regex.Escape(header) + ".*");

            Final = replace;
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

                    if (_self)
                    {
                        doc.SourceCodeColor.SetRangeColor(head + match.Index, match.Length, _style);
                        doc.SourceCodeColor.SetRangeMetaIfUndef(head + match.Index, match.Length, SrcCodeMeta.ENCLOSURE_DEFINED);
                    }
                    else
                    {
                        doc.SourceCodeColor.SetRangeColor(head + match.Index + _header.Length, match.Length - _header.Length, _style);
                        doc.SourceCodeColor.SetRangeMetaIfUndef(head + match.Index, match.Length, SrcCodeMeta.ENCLOSURE_DEFINED);
                    }
                }
            }
        }
    }
}
