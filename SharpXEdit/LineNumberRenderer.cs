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
    /// Provides the base class for line numbers renderer
    /// </summary>
    public class LineNumberRenderer
    {
        /// <summary>
        /// Renders the line numbers
        /// </summary>
        /// <param name="lineStart">line start</param>
        /// <param name="lineCount">line count</param>
        /// <param name="theme">theme</param>
        /// <param name="sender">sender</param>
        /// <param name="g">graphics context</param>
        public virtual void Render( int lineStart, int lineCount, TextAreaTheme theme, XTextEditor sender, Graphics g )
        {
            SolidBrush backBrush = BrushFactory.GetInstance(theme.LineNumberBackgroundColor);
            g.FillRectangle(backBrush, 0, 0, sender.LineNumberWidth, sender.Height);

            for (int i = lineStart; i < lineStart + lineCount; i++)
            {
                if (i < 0 || i >= sender.Document.Cache.LineCount)
                    return;

                string text = (i + 1).ToString();
                int w = sender.Document.SourceCodeManager.GetWidth(text);
                TextRenderer.DrawText(g, text, sender.Font, new Point(sender.LineNumberWidth - w - 8, sender.FontHeight * i - sender.Document.Scroll.Vertical), i == sender.Document.Caret.Line ? theme.CaretLineNumberColor : theme.LineNumberColor);
            }
        }
    }
}
