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

namespace SharpXEdit
{
    /// <summary>
    /// Metadata of a source code
    /// </summary>
    public static class SrcCodeMeta
    {
        /// <summary>
        /// NO METADATA
        /// </summary>
        public static int NONE => 0;
        /// <summary>
        /// DEFINED BY A ENCLOSURE
        /// </summary>
        public static int ENCLOSURE_DEFINED => 1;
    }

    /// <summary>
    /// Represents a object which contains colors of a source code
    /// </summary>
    public sealed class SourceCodeColor
    {
        private Dictionary<int, CharStyle> _colors = new Dictionary<int, CharStyle>();
        private Dictionary<int, int> _metadata = new Dictionary<int, int>();

        /// <summary>
        /// Checks if the style of the specified character index exists
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <returns>value</returns>
        public bool ExistColor(int charIndex)
        {
            lock (_colors)
                return _colors.ContainsKey(charIndex);
        }

        /// <summary>
        /// Gets the color of the specified character index
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <returns>character style</returns>
        public CharStyle GetColor(int charIndex)
        {
            lock (_colors)
            {
                if (_colors.TryGetValue(charIndex, out CharStyle CharStyle))
                    return CharStyle;
                else
                    return new CharStyle(Color.White, FontStyle.Normal);
            }
        }

        /// <summary>
        /// Sets the style of the specified character index
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <param name="CharStyle">character style</param>
        public void SetColor(int charIndex, CharStyle CharStyle)
        {
            lock (_colors)
            {
                if (_colors.ContainsKey(charIndex))
                    _colors[charIndex] = CharStyle;
                else
                    _colors.Add(charIndex, CharStyle);
            }
        }

        /// <summary>
        /// Sets the style of the specified character index if it is undefined
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <param name="charColor">character style</param>
        public void SetColorIfUndef(int charIndex, CharStyle charColor)
        {
            lock (_colors)
            {
                if (!_colors.ContainsKey(charIndex))
                    _colors.Add(charIndex, charColor);
            }
        }

        /// <summary>
        /// Sets the specified range of the styles
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <param name="length">length of the range</param>
        /// <param name="charColor">character style</param>
        public void SetRangeColor(int charIndex, int length, CharStyle charColor)
        {
            for (int i = charIndex; i < charIndex + length; i++)
            {
                SetColor(i, charColor);
            }
        }

        /// <summary>
        /// Sets the specified range of the styles if they are undefined
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <param name="length">length of the range</param>
        /// <param name="charColor">character style</param>
        public void SetRangeColorIfUndef(int charIndex, int length, CharStyle charColor)
        {
            for (int i = charIndex; i < charIndex + length; i++)
            {
                SetColorIfUndef(i, charColor);
            }
        }

        /// <summary>
        /// Sets the specified range of the metadata if they are undefined
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <param name="length">length of the range</param>
        /// <param name="metadata">metadata</param>
        public void SetRangeMetaIfUndef(int charIndex, int length, int metadata)
        {
            for (int i = charIndex; i < charIndex + length; i++)
            {
                SetMetaIfUndef(i, metadata);
            }
        }

        /// <summary>
        /// Sets the metadata of the specified character index
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <param name="metadata">metadata</param>
        public void SetMetaIfUndef(int charIndex, int metadata)
        {
            lock (_metadata)
            {
                if (!_metadata.ContainsKey(charIndex))
                    _metadata.Add(charIndex, metadata);
            }
        }

        /// <summary>
        /// Clears all data
        /// </summary>
        public void ClearColor()
        {
            lock (_colors)
            {
                _colors.Clear();
            }
            lock (_metadata)
            {
                _metadata.Clear();
            }
        }

        /// <summary>
        /// Gets the metadata of the specified character index
        /// </summary>
        /// <param name="charIndex">character index</param>
        /// <returns>metadata</returns>
        public int GetMeta(int charIndex)
        {
            lock (_metadata)
            {
                if (_metadata.TryGetValue(charIndex, out int meta))
                    return meta;
                else
                    return SrcCodeMeta.NONE;
            }
        }
    }
}
