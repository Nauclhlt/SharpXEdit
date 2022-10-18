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
    internal struct CodeRange
    {
        private string _text;
        private CharStyle _style;
        private int _xOffset;
        private int _yOffset;
        private Rectangle _drawingBounds;

        public string Text => _text;
        public CharStyle Style => _style;
        public int XOffset => _xOffset;
        public int YOffset => _yOffset;
        public Rectangle DrawingBounds => _drawingBounds;

        public CodeRange( string text, CharStyle style, int xOffset, int yOffset, int bwidth, int bheight )
        {
            _text = text;
            _style = style;
            _xOffset = xOffset;
            _yOffset = yOffset;
            _drawingBounds = new Rectangle(xOffset, yOffset, bwidth, bheight);
        }

        public void ShiftY( int offset )
        {
            _yOffset += offset;
        }
    }
}
