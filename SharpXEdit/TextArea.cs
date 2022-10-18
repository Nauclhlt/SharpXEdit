using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms.Design;
using static SharpXEdit.TextArea;
using System.Windows.Forms.VisualStyles;

#pragma warning disable CS1591

namespace SharpXEdit
{
    public class TextArea : Control
    {
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_RESULTREADSTR = 0x0200;
        private const int WM_IME_STARTCOMPOSITION = 0x10D;
        private const int WM_IME_ENDCOMPOSITION = 0x10E;
        private const int WM_IME_NOTIFY = 0x0282;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const uint CFS_POINT = 0x0002;
        private const int WM_CHAR = 0x0102;
        private const int WM_UNICHAR = 0x0109;



        private Document _document;
        private int _lineNumberWidth = 80;
        private Dictionary<int, List<CodeRange>> _codeRanges = new Dictionary<int, List<CodeRange>>();
        private TextAreaTheme _theme = new TextAreaTheme();
        private IntPtr _hImc = IntPtr.Zero;
        private bool _mouseDown = false;
        private int _scrollSpeed = 3;
        

        public Document Document
        {
            get => _document;
            set
            {
                if (value is null)
                    return;

                if (value.Parent != this)
                    throw new ArgumentException("new document must have this object as its parent", nameof(value));

                RemoveDocumentEvents(_document);
                _document = value;
                AddDocumentEvents(_document);
            }
        }

        public int LineNumberWidth
        {
            get => _lineNumberWidth;
            set
            {
                if (value <= 0)
                    return;
                _lineNumberWidth = value;
            }
        }

        public TextAreaTheme Theme
        {
            get => _theme;
            set
            {
                if (value is object)
                {
                    _theme = value;
                }
            }
        }

        public int ScrollSpeed
        {
            get => _scrollSpeed;
            set
            {
                _scrollSpeed = Math.Max(1, value);
            }
        }

        public new int FontHeight => base.FontHeight;

        public TextArea()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.UserMouse, true);


            _document = new Document(this);
            _document.SourceCodeColor.SetRangeColorIfUndef(0, 5, Color.Blue);

            Cursor = Cursors.IBeam;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.FillRectangle(BrushFactory.GetInstance(_theme.BackgroundColor), ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle clipBounds = e.ClipRectangle;
            bool redrawAll = e.ClipRectangle.Left == 0 && e.ClipRectangle.Top == 0 && e.ClipRectangle.Width == Width && e.ClipRectangle.Height == Height;

            Rectangle caretBounds = new Rectangle(_document.SourceCodeManager.GetCaretPoint(_document.Caret), new Size(2, FontHeight));
            bool caretInScreen = new Rectangle(0, 0, Width, Height).IntersectsWith(caretBounds);

            if (caretInScreen && !clipBounds.Contains(caretBounds))
            {
                Invalidate();
                return;
            }

            if (_document.IsUserModified)
            {
                CreateAllVisibleCodeRanges();
                _document.RemoveModifiedFlag();
            }

            int lineStart = _document.SourceCodeManager.GetLineStart();
            int lineCount = _document.SourceCodeManager.GetVisibleLineCount();
            int charIndex1 = _document.SelectionStart.GetCharIndex(), charIndex2 = _document.Caret.GetCharIndex();
            int selectionBegin = Math.Min(charIndex1, charIndex2);
            int selectionEnd = Math.Max(charIndex1, charIndex2);

            if (_document.Caret.HasSamePosition(_document.SelectionStart))
            {
                selectionBegin = -1;
                selectionEnd = -1;
            }

            //Console.WriteLine(selectionBegin + "," + selectionEnd);

            using SolidBrush selectionBrush = new SolidBrush(Color.Aquamarine);

            for (int i = lineStart; i < lineStart + lineCount; i++)
            {
                if (i < 0 || i >= _document.Cache.LineCount)
                    continue;

                int lineHead = _document.Cache.GetLineHead(i);
                string line = _document.Cache.GetLineText(i);
                int lineLength = line.Length;
                int lineEnd = lineHead + lineLength;
                bool headFlag = Util.IsInRange(lineHead, selectionBegin, selectionEnd);
                bool endFlag = Util.IsInRange(lineEnd, selectionBegin, selectionEnd);

                //if (!headFlag && !endFlag)
                //    goto selectionRenderingEnd;
                if ( headFlag && endFlag )
                {
                    // The line is completely in the range of selection

                    int lineWidth = _document.SourceCodeManager.GetWidth(line);
                    lineWidth = Math.Clamp(lineWidth, Util.Shared.LineFeedSelectionWidth, Width - LineNumberWidth - Util.Shared.LeftMargin);

                    g.FillRectangle(selectionBrush, LineNumberWidth + Util.Shared.LeftMargin, FontHeight * i - _document.Scroll.Vertical, lineWidth, FontHeight);
                }
                else
                {
                    // The line is partly in the range of selection
                    int begin = 0;
                    int len = 0;
                    for (int j = 0; j < lineLength; j++)
                    {
                        int idx = lineHead + j;
                        if (idx >= selectionBegin && idx <= selectionEnd)
                        {
                            begin = j;
                            len = 0;
                            for (int k = j + 1; k < lineLength + 1; k++)
                            {
                                int sidx = lineHead + k;
                                if (sidx < selectionBegin || sidx > selectionEnd)
                                {
                                    break;
                                }
                                len++;
                            }
                            break;
                        }
                    }

                    g.FillRectangle(selectionBrush, LineNumberWidth + Util.Shared.LeftMargin + _document.SourceCodeManager.GetWidth(line.Substring(0, begin)), FontHeight * i - _document.Scroll.Vertical, _document.SourceCodeManager.GetWidth(line.Substring(begin, len)), FontHeight);
                }

                selectionRenderingEnd:


                if (_codeRanges.TryGetValue(i, out List<CodeRange> lineCodeRanges))
                {
                    for (int j = 0; j < lineCodeRanges.Count; j++)
                    {
                        CodeRange codeRange = lineCodeRanges[j];
                        Rectangle rect = codeRange.DrawingBounds;
                        // Scroll conversion
                        rect = new Rectangle(rect.X - _document.Scroll.Horizontal, rect.Y - _document.Scroll.Vertical, rect.Width, rect.Height);
                        if (clipBounds.IntersectsWith(rect))
                            TextRenderer.DrawText(g, codeRange.Text, Font, new Point(codeRange.XOffset - _document.Scroll.Horizontal, codeRange.YOffset - _document.Scroll.Vertical), codeRange.Style.Color, Util.Shared.TextFormatFlags);
                    }
                }
            }

            //Draw caret
            {
                SolidBrush brush = BrushFactory.GetInstance(_theme.CaretColor);
                g.FillRectangle(brush, caretBounds);
            }

            if (redrawAll)
                RenderLineNumbers(g);
        }

        private void RenderLineNumbers( Graphics g )
        {
            SolidBrush backBrush = BrushFactory.GetInstance(_theme.LineNumberBackgroundColor);
            g.FillRectangle(backBrush, 0, 0, _lineNumberWidth, Height);

            int lineStart = _document.SourceCodeManager.GetLineStart();
            int lineCount = _document.SourceCodeManager.GetVisibleLineCount();

            for (int i = lineStart; i < lineStart + lineCount; i++)
            {
                if (i < 0 || i >= _document.Cache.LineCount)
                    return;

                string text = (i + 1).ToString();
                int w = _document.SourceCodeManager.GetWidth(text);
                TextRenderer.DrawText(g, text, Font, new Point(LineNumberWidth - w - 8, FontHeight * i - _document.Scroll.Vertical), i == _document.Caret.Line ? _theme.CaretLineNumberColor : _theme.LineNumberColor);
            }
        }

        private unsafe void CreateLineCodeRanges( int lineIndex )
        {
            if (lineIndex < 0 || lineIndex >= _document.Cache.LineCount)
                return;

            List<CodeRange> ranges = new List<CodeRange>();

            int head = _document.Cache.GetLineHead(lineIndex);
            string line = _document.Cache.GetLineText(lineIndex);
            int length = line.Length;


            int left = _document.SourceCodeManager.GetTextLeft();
            int leftOffset = 0;
            for (int i = 0; i < length; i++)
            {
                CharStyle style = _document.SourceCodeColor.GetColor(head + i);
                bool exist = _document.SourceCodeColor.ExistColor(head + i);
                int len = 1;
                for (int j = i + 1; j < length; j++)
                {
                    fixed(char* ptr = line)
                    {
                        if (line[j] == ' ')
                            goto end;
                    }
                    if (exist != _document.SourceCodeColor.ExistColor(head + j) || _document.SourceCodeColor.GetColor(head + j) != style)
                        break;

                    end:
                    len++;
                }

                string range = line.Substring(i, len);
                int w = _document.SourceCodeManager.GetWidth(range);

                ranges.Add(new CodeRange(range, exist ? style : new CharStyle(_theme.TextColor, FontStyle.Normal), left + leftOffset, FontHeight * lineIndex, w, FontHeight));

                leftOffset += w;

                i += len - 1;
            }

            RegisterCodeRanges(lineIndex, ranges);
        }

        private void CreateAllVisibleCodeRanges()
        {
            int start = _document.SourceCodeManager.GetLineStart();
            int count = _document.SourceCodeManager.GetVisibleLineCount();

            for (int i = start; i < start + count; i++)
            {
                CreateLineCodeRanges(i);
            }
        }

        private void FillEmptyCodeRanges()
        {
            int start = _document.SourceCodeManager.GetLineStart();
            int count = _document.SourceCodeManager.GetVisibleLineCount();

            for (int i = start; i < start + count; i++)
            {
                if (!_codeRanges.ContainsKey(i))
                    CreateLineCodeRanges(i);
            }
        }

        private void ShiftCodeRangesV( int offset )
        {
            foreach ( List<CodeRange> list in _codeRanges.Values )
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].ShiftY(offset);
                }
            }
        }

        private void RegisterCodeRanges( int line, List<CodeRange> codeRanges )
        {
            if (_codeRanges.ContainsKey(line))
            {
                _codeRanges[line] = codeRanges;
            }
            else
            {
                _codeRanges.Add(line, codeRanges);
            }
        }

        private void ProcessIMECharInput( char character )
        {
            Console.Write(character);
            if (character == '\r')
            {
                BreakLine();
                _document.Caret.Set(_document.Caret.Line + 1, 0);
                _document.RemoveSelection();
                CreateAllVisibleCodeRanges();
                Invalidate();
                return;
            }

            int charIndex = _document.Caret.GetCharIndex();

            if (character == '\b')
            {
                if (_document.Caret.Column == 0)
                {
                    // Delete line
                    if (_document.Caret.Line > 0)
                    {
                        string code = _document.LineFeedCode.GetCode();
                        string prevLine = _document.Cache.GetLineText(_document.Caret.Line - 1);
                        _document.IDA.Remove(charIndex - code.Length, code.Length);
                        _document.Caret.Set(_document.Caret.Line - 1, prevLine.Length);
                        _document.RemoveSelection();

                        CreateAllVisibleCodeRanges();
                        Invalidate();
                    }
                }
                else
                {
                    _document.IDA.Remove(charIndex - 1, 1);
                    _document.Caret.Offset(0, -1);
                    _document.RemoveSelection();

                    UpdateLine(_document.Caret.Line);
                }

                return;
            }

            if (character == '\t')
            {
                Console.Write("TAB");
                return;
            }
            
            _document.IDA.Insert(charIndex, character);

            _document.Caret.Offset(0, 1);
            _document.RemoveSelection();

            UpdateLine(_document.Caret.Line);
        }

        private void InvisualInput( char value )
        {
            InvisualInput(value.ToString());
        }

        private void InvisualInput( string value )
        {
            string[] lines = value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                _document.IDA.Insert(_document.Caret.GetCharIndex(), lines[i]);
                _document.Caret.Offset(0, lines[i].Length);
                CreateLineCodeRanges(_document.Caret.Line);

                if (i != lines.Length - 1)
                    BreakLine();
            }

            Invalidate();
        }

        private void BreakLine()
        {
            _document.IDA.BreakLine();
        }

        private void UpdateLine( int lineIndex )
        {
            CreateLineCodeRanges(lineIndex);
            InvalidateLine(lineIndex);
        }

        private void InvalidateLine( int lineIndex )
        {
            if (lineIndex < 0 || lineIndex >= _document.Cache.LineCount)
                return;

            Invalidate(_document.SourceCodeManager.GetLineRefreshBounds(lineIndex));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.Delete )
            {
                int charIndex = _document.Caret.GetCharIndex();
                if (charIndex < _document.Length - 1)
                {
                    _document.IDA.Remove(charIndex, 1);
                    CreateAllVisibleCodeRanges();
                    Invalidate();
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int delta = e.Delta;

            if (delta != 0)
            {
                if (delta < 0)
                {
                    //Scroll down

                    _document.Scroll.Vertical += FontHeight * ScrollSpeed;
                }
                else
                {
                    //Scroll up
                    _document.Scroll.Vertical -= FontHeight * ScrollSpeed;
                }

                FillEmptyCodeRanges();

                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!_mouseDown && e.Button == MouseButtons.Left)
            {
                _document.SelectionManager.MouseSelect(e.Location);
                _mouseDown = true;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_mouseDown)
            {
                _document.SelectionManager.MouseDrag(e.Location);
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                _mouseDown = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData & Keys.Control) == Keys.Control)
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys keyWithoutModifier = keyData & ~Keys.Modifiers;
            Keys modifiers = keyData & Keys.Modifiers;
            
            if (keyWithoutModifier == Keys.Right)
            {
                string line = _document.Cache.GetLineText(_document.Caret.Line);
                if (_document.Caret.Column == line.Length)
                {
                    if (_document.Caret.Line < _document.Cache.LineCount - 1)
                    {
                        _document.Caret.Set(_document.Caret.Line + 1, 0);
                        Invalidate();
                    }
                }
                else
                {
                    _document.Caret.Offset(0, 1);
                    InvalidateLine(_document.Caret.Line);
                }

                if ((modifiers & Keys.Shift) != Keys.Shift)
                {
                    _document.RemoveSelection();
                }

                _document.SavedCaretColumn = _document.Caret.Column;
                
                return true;
            }

            if (keyWithoutModifier == Keys.Left)
            {
                if (_document.Caret.Column == 0)
                {
                    if (_document.Caret.Line > 0)
                    {
                        _document.Caret.Set(_document.Caret.Line - 1, _document.Cache.GetLineText(_document.Caret.Line - 1).Length);
                        Invalidate();
                    }
                }
                else
                {
                    _document.Caret.Offset(0, -1);
                    InvalidateLine(_document.Caret.Line);
                }

                _document.SavedCaretColumn = _document.Caret.Column;

                if ((modifiers & Keys.Shift) != Keys.Shift)
                {
                    _document.RemoveSelection();
                }

                return true;
            }

            if (keyWithoutModifier == Keys.Down)
            {
                if (_document.Caret.Line < _document.Cache.LineCount - 1)
                {
                    string nextLine = _document.Cache.GetLineText(_document.Caret.Line + 1);
                    if (_document.Caret.Column > _document.SavedCaretColumn)
                    {
                        _document.SavedCaretColumn = _document.Caret.Column;
                    }
                    if (nextLine.Length < _document.Caret.Column)
                        _document.Caret.Set(_document.Caret.Line + 1, nextLine.Length);
                    else
                        _document.Caret.Set(_document.Caret.Line + 1, Math.Min(nextLine.Length, _document.SavedCaretColumn));

                    Invalidate();
                }

                if ((modifiers & Keys.Shift) != Keys.Shift)
                {
                    _document.RemoveSelection();
                }

                return true;
            }

            if (keyWithoutModifier == Keys.Up)
            {
                if (_document.Caret.Line > 0)
                {
                    string prevLine = _document.Cache.GetLineText(_document.Caret.Line - 1);
                    if (_document.Caret.Column > _document.SavedCaretColumn)
                    {
                        _document.SavedCaretColumn = _document.Caret.Column;
                    }
                    if (prevLine.Length < _document.Caret.Column)
                        _document.Caret.Set(_document.Caret.Line - 1, prevLine.Length);
                    else
                        _document.Caret.Set(_document.Caret.Line - 1, Math.Min(prevLine.Length, _document.SavedCaretColumn));

                    Invalidate();
                }

                if ((modifiers & Keys.Shift) != Keys.Shift)
                {
                    _document.RemoveSelection();
                }

                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void OnVerticalScrollChanged( object sender, EventArgs e )
        {
            CreateAllVisibleCodeRanges();
            Invalidate();
        }

        private void OnHorizontalScrollChanged( object sender, EventArgs e )
        {

        }


        private void AddDocumentEvents( Document doc )
        {
            doc.Scroll.VerticalChanged += OnVerticalScrollChanged;
            doc.Scroll.HorizontalChanged += OnHorizontalScrollChanged;
        }

        private void RemoveDocumentEvents( Document doc )
        {
            doc.Scroll.VerticalChanged -= OnVerticalScrollChanged;
            doc.Scroll.HorizontalChanged -= OnHorizontalScrollChanged;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            if (_hImc != IntPtr.Zero)
            {
                NativeMethods.ImmReleaseContext(Handle, _hImc);
                _hImc = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            CreateAllVisibleCodeRanges();
            Invalidate();
        }

        #region IME


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_IME_SETCONTEXT)
            {
                _hImc = NativeMethods.ImmCreateContext();
                NativeMethods.ImmAssociateContextEx(Handle, _hImc, ImmAssociateContextExFlags.IACE_DEFAULT);
                base.WndProc(ref m);
                return;
            }
            if (m.Msg == WM_IME_STARTCOMPOSITION)
            {
                IntPtr hImc = NativeMethods.ImmGetContext(Handle);

                COMPOSITIONFORM info = new COMPOSITIONFORM();
                info.dwStyle = CFS_POINT;
                info.ptCurrentPos.x = 10;
                info.ptCurrentPos.y = 10;
                NativeMethods.ImmSetCompositionWindow(hImc, ref info);

                NativeMethods.ImmReleaseContext(Handle, hImc);

                base.WndProc(ref m);
                return;
            }
            if (m.Msg == WM_CHAR || m.Msg == WM_UNICHAR)
            {
                char c = (char)(m.WParam);
                ProcessIMECharInput(c);
                return;
            }

            base.WndProc(ref m);
        }

        #endregion



        #region WinAPI definition


        private static class NativeMethods
        {
            [DllImport("Imm32.dll")]
            public static extern IntPtr ImmGetContext(IntPtr hWnd);
            [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
            public static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lpBuf, int dwBufLen);
            [DllImport("Imm32.dll")]
            public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
            [DllImport("imm32.dll")]
            public static extern IntPtr ImmCreateContext();
            [DllImport("imm32.dll")]
            public static extern bool ImmAssociateContextEx(IntPtr hWnd, IntPtr hIMC, ImmAssociateContextExFlags dwFlags);
            [DllImport("imm32.dll")]
            public static extern int ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompositionForm);
            [DllImport("gdi32.dll")]
            public static extern int SetTextColor(IntPtr hdc, int crColor);
            [DllImport("gdi32.dll")]
            public static extern int SetBkColor(IntPtr hdc, int crColor);
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int DrawText(IntPtr hdc, string lpStr, int nCount, ref Rect lpRect, int wFormat);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);
            [DllImport("gdi32.dll")]
            public static extern int SetBkMode(IntPtr hdc, int iBkMode);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr objectHandle);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectPalette(IntPtr hdc, IntPtr hpal, bool bForceBackground);
            [DllImport("gdi32.dll")]
            private static extern int RealizePalette(IntPtr hdc);
        }

        private enum ImmAssociateContextExFlags : uint
        {
            IACE_CHILDREN = 0x0001,
            IACE_DEFAULT = 0x0010,
            IACE_IGNORENOCONTEXT = 0x0020
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct C_RECT
        {
            public int _Left;
            public int _Top;
            public int _Right;
            public int _Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct C_POINT
        {
            public int x;
            public int y;
        }

        internal struct COMPOSITIONFORM
        {
            public uint dwStyle;
            public C_POINT ptCurrentPos;
            public C_RECT rcArea;
        }

        internal struct Rect
        {
            public int Left, Top, Right, Bottom;
            public Rect(Rectangle r)
            {
                this.Left = r.Left;
                this.Top = r.Top;
                this.Bottom = r.Bottom;
                this.Right = r.Right;
            }
        }
        #endregion
    }
}
