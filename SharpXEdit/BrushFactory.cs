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
    /// Provides a factory for brush objects
    /// </summary>
    internal static class BrushFactory
    {
        private static Dictionary<int, SolidBrush> _cache = new Dictionary<int, SolidBrush>();

        public static SolidBrush GetInstance( Color color )
        {
            int argb = color.ToArgb();

            if (_cache.TryGetValue(argb, out SolidBrush brush))
                return brush;
            else
            {
                SolidBrush obj = new SolidBrush(color);
                _cache.Add(argb, obj);
                return obj;
            }
        }

        public static void Clear()
        {
            foreach ( SolidBrush item in _cache.Values )
            {
                item.Dispose();
            }
            _cache.Clear();
        }
    }
}
