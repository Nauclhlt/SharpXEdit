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
    /// Represents a scroll data for a document
    /// </summary>
    public class DocumentScroll
    {
        private int _vertical;
        private int _horizontal;
        private Document _document;

        /// <summary>
        /// Raised when the vertical scroll value is changed
        /// </summary>
        public event EventHandler VerticalChanged;
        /// <summary>
        /// Raised when the horizontal scroll value is changed
        /// </summary>
        public event EventHandler HorizontalChanged;

        internal DocumentScroll( Document document ) 
        {
            _vertical = 0;
            _horizontal = 0;
            _document = document;
        }

        /// <summary>
        /// Gets or sets the vertical scroll value
        /// </summary>
        public int Vertical
        {
            get => _vertical;
            set
            {
                int max = _document.Parent.FontHeight * (_document.Cache.LineCount - 1);

                _vertical = Math.Clamp(value, 0, max);
                //_vertical = value;
                VerticalChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll value
        /// </summary>
        public int Horizontal
        {
            get => _horizontal;
            set
            {
                int max = Math.Max(0, _document.SourceCodeManager.GetWidth(_document.Cache.GetLineText(_document.Cache.LongestLine)) - _document.SourceCodeManager.GetWidth("A"));

                _horizontal = Math.Clamp(value, 0, max);
                HorizontalChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
