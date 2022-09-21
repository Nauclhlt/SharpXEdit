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

namespace SharpXEdit
{
    public class TextArea : Control
    {
        private Document _document;

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
    }
}
