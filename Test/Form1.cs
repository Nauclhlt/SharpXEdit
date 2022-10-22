using SharpXEdit;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.CodeDom;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeywordsHighlighter highlighter = new KeywordsHighlighter(Color.Blue, true, "using", "public", "class", "static", "void", "string", "foreach", "else", "if", "continue", "break", "return", "while");
            QuoteHighlighter strHighlighter = new QuoteHighlighter(Color.Red, false, "\"", "\"");
            xTextEditor1.Highlighters.Add(highlighter);
            xTextEditor1.Highlighters.Add(strHighlighter);

            xTextEditor1.AutoCompleteSource.AddRange(new[] { "using", "public", "static", "void", "class", "string", "return", "while", "for", "foreach", "if", "else", "continue", "break" });

            
        }
    }
}