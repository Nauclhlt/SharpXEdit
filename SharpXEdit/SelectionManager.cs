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
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace SharpXEdit
{
    internal class SelectionManager
    {
        private Document _document;

        public SelectionManager( Document document )
        {
            _document = document;
        }

        private TextPoint GetPointFromMousePoint( Point mousePoint )
        {
            int line = (mousePoint.Y + _document.Scroll.Vertical) / _document.Parent.FontHeight;

            if (line >= _document.Cache.LineCount)
            {
                string lastLine = _document.Cache.GetLineText(_document.Cache.LineCount - 1);
                return new TextPoint(_document.Cache.LineCount - 1, lastLine.Length);
            }

            int left = _document.SourceCodeManager.GetTextLeft();
            if (mousePoint.X <= left)
                return new TextPoint(line, 0);
            string lineText = _document.Cache.GetLineText(line);
            int textX = mousePoint.X - left;
            for (int i = 1; i < lineText.Length + 1; i++)
            {
                int w = _document.SourceCodeManager.GetWidth(lineText.AsSpan(0, i));
                
                if (textX < w)
                {
                    int lastCharHalfWidth = _document.SourceCodeManager.GetWidth(lineText.AsSpan(i - 1, 1)) / 2;

                    if (textX < w - lastCharHalfWidth)
                        return new TextPoint(line, i - 1);
                    else
                        return new TextPoint(line, i);
                }
                else
                {
                    if (i == lineText.Length)
                        return new TextPoint(line, lineText.Length);
                }
            }

            return new TextPoint(line, 0);
        }

        public void MouseSelect( Point mousePoint )
        {
            mousePoint.Offset(_document.Scroll.Horizontal, 0);
            TextPoint pt = GetPointFromMousePoint(mousePoint);
            _document.Caret.Set(pt.Line, pt.Column);
            _document.RemoveSelection();
        }

        public void MouseDrag( Point mousePoint )
        {
            mousePoint.Offset(_document.Scroll.Horizontal, 0);
            TextPoint pt = GetPointFromMousePoint(mousePoint);
            _document.Caret.Set(pt.Line, pt.Column);
        }
    }
}
