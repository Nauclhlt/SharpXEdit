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
    /// Provides a collection of highlight objects
    /// </summary>
    public class HighlighterCollection : IList<SourceCodeHighlighter>, ICollection<SourceCodeHighlighter>, IEnumerable<SourceCodeHighlighter>, IReadOnlyCollection<SourceCodeHighlighter>
    {
        private List<SourceCodeHighlighter> _list = new List<SourceCodeHighlighter>();

        /// <summary>
        /// Gets or sets the item at the specified index
        /// </summary>
        /// <param name="index">target index</param>
        /// <returns>the item at the specified index</returns>
        public SourceCodeHighlighter this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        /// <summary>
        /// Gets if the collection is read-only
        /// </summary>
        public bool IsReadOnly => false;
        /// <summary>
        /// Gets the number of items
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// 
        /// </summary>
        public HighlighterCollection()
        {
            _list = new List<SourceCodeHighlighter>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HighlighterCollection(int capacity)
        {
            _list = new List<SourceCodeHighlighter>(capacity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceCollection"></param>
        public HighlighterCollection(IEnumerable<SourceCodeHighlighter> sourceCollection)
        {
            _list = new List<SourceCodeHighlighter>(sourceCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(SourceCodeHighlighter item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(SourceCodeHighlighter item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _list.Count)
                return;
            _list.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(SourceCodeHighlighter item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(SourceCodeHighlighter item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, SourceCodeHighlighter item)
        {
            if (index < 0 || index >= _list.Count)
                return;
            _list.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(SourceCodeHighlighter[] array, int index)
        {
            _list.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a iterator for the collection
        /// </summary>
        /// <returns>enumerator instance</returns>
        public IEnumerator<SourceCodeHighlighter> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}