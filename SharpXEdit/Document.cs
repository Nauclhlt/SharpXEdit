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
    /// Represents a document
    /// </summary>
    public sealed class Document
    {
        private static readonly int DefaultCapacity = 2 ^ 16;

        /// <summary>
        /// Raised when the text is changed
        /// </summary>
        public event EventHandler TextChanged;
        /// <summary>
        /// Raised when the cache is created
        /// </summary>
        public event EventHandler CacheCreated;


        private LineFeedCode _lineFeedCode = LineFeedCode.CRLF;
        private TextArea _parent;
        private StringBuilder _strBuilder;
        private string _lastText;
        private DocumentCache _cache;
        private Caret _caret;
        private Caret _selectionStart;
        private SourceCodeManager _sourceCodeManager;

        /// <summary>
        /// Gets or sets the line-feed code for the document
        /// </summary>
        public LineFeedCode LineFeedCode
        {
            get => _lineFeedCode;
            set
            {
                _lineFeedCode = value;
            }
        }

        /// <summary>
        /// Gets the internal <see cref="StringBuilder"/> object
        /// </summary>
        public StringBuilder Builder => _strBuilder;

        /// <summary>
        /// Gets the <see cref="DocumentCache"/> object of the document
        /// </summary>
        public DocumentCache Cache => _cache;

        /// <summary>
        /// Gets the length of the document
        /// </summary>
        public int Length => _strBuilder.Length;

        /// <summary>
        /// Gets the caret of the document
        /// </summary>
        public Caret Caret => _caret;

        /// <summary>
        /// Gets the selection start of the document
        /// </summary>
        public Caret SelectionStart => _selectionStart;

        /// <summary>
        /// Gets the <see cref="SourceCodeManager"/> object for the document
        /// </summary>
        public SourceCodeManager SourceCodeManager => _sourceCodeManager;
        
        /// <summary>
        /// Gets or sets the text of the document
        /// </summary>
        public string Text
        {
            get => _lastText;
            set
            {
                if (value is object)
                {
                    if (value.Length > DefaultCapacity)
                    {
                        _strBuilder = new StringBuilder(value);
                    }
                    else
                    {
                        _strBuilder = new StringBuilder(value, DefaultCapacity);
                    }

                    CreateCache();
                }
            }
        }

        internal Document( TextArea parent )
        {
            _parent = parent;

            _strBuilder = new StringBuilder(DefaultCapacity);

            CreateCache();
        }

        /// <summary>
        /// Inserts the specified string value in the specified index
        /// </summary>
        /// <param name="index">index to be inserted in</param>
        /// <param name="value">vakue to insert</param>
        public void Insert( int index, string value )
        {
            if (index < 0 || index > Length)
                return;

            _strBuilder.Insert(index, value);

            RaiseTextChanged();
            CreateCache();
        }

        /// <summary>
        /// Replaces the specified old string with the new string
        /// </summary>
        /// <param name="oldStr">old string</param>
        /// <param name="newStr">new string</param>
        public void Replace( string oldStr, string newStr )
        {
            _strBuilder.Replace(oldStr, newStr);

            RaiseTextChanged();
            CreateCache();
        }

        /// <summary>
        /// Replaces the specified old character with the new character
        /// </summary>
        /// <param name="oldChar">old character</param>
        /// <param name="newChar">new character</param>
        public void Replace( char oldChar, char newChar )
        {
            _strBuilder.Replace(oldChar, newChar);

            RaiseTextChanged();
            CreateCache();
        }

        /// <summary>
        /// Gets the substring of the document in the specified index with the specified length
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="length">length</param>
        /// <returns>substring</returns>
        public string Substring( int index, int length )
        {
            if (index < 0 || index >= Length ||
                index + length >= Length)
            {
                return string.Empty;
            }

            return _strBuilder.ToString(index, length);
        }

        private void CreateCache()
        {
            _lastText = _strBuilder.ToString();
            _cache = new DocumentCache(this);

            CacheCreated?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
