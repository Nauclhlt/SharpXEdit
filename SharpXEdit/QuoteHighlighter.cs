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
using System.DirectoryServices.ActiveDirectory;

namespace SharpXEdit
{
    /// <summary>
    /// Provides a highlighting process for ranges enclosed by 2 strings
    /// </summary>
    public sealed class QuoteHighlighter : SourceCodeHighlighter
    {
        private Regex _regex;
        private CharStyle _style;
        private bool _multiLine = false;
        private bool _self = true;
        private int _openLen = 0;
        private int _closeLen = 0;
        private CharStyle _outStyle;

        /// <summary>
        /// Creates new object
        /// </summary>
        /// <param name="style"></param>
        /// <param name="multiLine"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="self"></param>
        /// <param name="replace"></param>
        /// <param name="outStyle"></param>
        public QuoteHighlighter( CharStyle style, bool multiLine, string open, string close, bool self = true, bool replace = true, CharStyle outStyle = default )
        {
            _style = style;
            _multiLine = multiLine;

            _regex = new Regex(Regex.Escape(open) + ".*?" + Regex.Escape(close), RegexOptions.Singleline);

            _openLen = open.Length;
            _closeLen = close.Length;

            _outStyle = outStyle;
            _self = self;
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
            if (_multiLine)
            {
                MatchCollection matches = _regex.Matches(doc.Text);
                for (int i = lineStart; i < lineEndPlus1; i++)
                {
                    int head = doc.Cache.GetLineHead(i);
                    int lineLen = doc.Cache.GetLineText(i).Length;
                 
                    for (int c = 0; c < lineLen; c++)
                    {
                        for (int j = 0; j < matches.Count; j++)
                        {
                            Match match = matches[j];

                            int idx = head + c;
                            if (idx >= match.Index && idx < match.Index + match.Length)
                            {
                                doc.SourceCodeColor.SetColor(idx, _style);
                                break;
                            }
                        }
                    }
                }
            }
            else
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
                            doc.SourceCodeColor.SetRangeMetaIfUndef(head + match.Index, match.Length, SrcCodeMeta.ENCLOSURE_DEFINED);
                            doc.SourceCodeColor.SetRangeColor(head + match.Index, match.Length, _style);
                        }
                        else
                        {
                            if (!_outStyle.Equals(default(CharStyle)))
                            {
                                doc.SourceCodeColor.SetRangeColorIfUndef(head + match.Index, _openLen, _outStyle);
                                doc.SourceCodeColor.SetRangeColorIfUndef(head + match.Index + match.Length - _closeLen, _closeLen, _outStyle);
                            }
                            doc.SourceCodeColor.SetRangeMetaIfUndef(head + match.Index, match.Length, SrcCodeMeta.ENCLOSURE_DEFINED);
                            doc.SourceCodeColor.SetRangeColor(head + match.Index + _openLen, match.Length - _openLen - _closeLen, _style);
                        }
                    }
                }
            }
        }
    }
}
