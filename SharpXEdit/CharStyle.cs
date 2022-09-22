using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.CodeDom;

namespace SharpXEdit
{
    /// <summary>
    /// Represents a character style
    /// </summary>
    public struct CharStyle
    {
        private FontStyle _fontStyle;
        private Color _color;

        /// <summary>
        /// Gets the color
        /// </summary>
        public Color Color => _color;
        /// <summary>
        /// Gets the font style
        /// </summary>
        public FontStyle FontStyle => _fontStyle;

        /// <summary>
        /// Initializes new structure with the specified color and font style
        /// </summary>
        /// <param name="color">color</param>
        /// <param name="style">font style</param>
        public CharStyle( Color color, FontStyle style )
        {
            _color = color;
            _fontStyle = style;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator CharStyle(Color source)
        {
            return new CharStyle(source, FontStyle.Normal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator Color(CharStyle source)
        {
            return source.Color;
        }

        /// <summary>
        /// Compares the equality between this object and the specified object
        /// </summary>
        /// <param name="obj">object to compare with</param>
        /// <returns>value</returns>
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is null)
                return false;

            if (obj is CharStyle other)
            {
                return _color.R == other.Color.R && _color.G == other.Color.G && _color.B == other.Color.B && _color.A == other.Color.A &&
                       _fontStyle == other.FontStyle;
            }

            return false;
        }

        /// <summary>
        /// Gets the unique hash code for the object
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator ==( CharStyle obj1, CharStyle obj2 )
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator !=( CharStyle obj1, CharStyle obj2 )
        {
            return !(obj1 == obj2);
        }
    }
}
