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

        public Document Document
        {
            get => _document;
            set
            {
                if (value is null)
                    return;
                _document = value;
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

        public new int FontHeight => base.FontHeight;

        public TextArea()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.UserMouse, true);


            _document = new Document(this);

            Cursor = Cursors.IBeam;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            if (_document.IsUserModified)
            {
                CreateAllVisibleCodeRanges();
            }

            int lineStart = _document.SourceCodeManager.GetLineStart();
            int lineCount = _document.SourceCodeManager.GetVisibleLineCount();

            for (int i = lineStart; i < lineStart + lineCount; i++)
            {
                if (i < 0 || i >= _document.Cache.LineCount)
                    continue;
                //string lineText = _document.Cache.GetLineText(i);
                //int left = _document.SourceCodeManager.GetTextLeft();
                //TextRenderer.DrawText(g, lineText, Font, new Point(left, i * FontHeight), Color.Black);

                if (_codeRanges.TryGetValue(i, out List<CodeRange> lineCodeRanges))
                {
                    for (int j = 0; j < lineCodeRanges.Count; j++)
                    {
                        CodeRange codeRange = lineCodeRanges[j];
                        TextRenderer.DrawText(g, codeRange.Text, Font, new Point(codeRange.XOffset, codeRange.YOffset), codeRange.Style.Color);
                    }
                }
            }

            //Draw caret
            Point point = _document.SourceCodeManager.GetCaretPoint(_document.Caret);
            
        }

        private void CreateLineCodeRanges( int lineIndex )
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
                    if (exist != _document.SourceCodeColor.ExistColor(head + j) || _document.SourceCodeColor.GetColor(head + j) != style)
                        break;

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
            if (character == '\r')
            {
                BreakLine();
                Invalidate();
                return;
            }
        }

        private void InvisualInput( char value )
        {
            InvisualInput(value.ToString());
        }

        private void InvisualInput( string value )
        {

        }

        private void BreakLine()
        {
            _document.IDA.BreakLine();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData & Keys.Control) == Keys.Control)
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
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
