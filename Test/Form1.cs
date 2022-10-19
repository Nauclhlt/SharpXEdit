using SharpXEdit;
using System.Configuration;
using System.Diagnostics;

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
            TextArea c = new TextArea();
            c.BackColor = Color.White;
            c.Bounds = new Rectangle(120, 120, 500, 500);
            c.Font = new Font("Consolas", 22);
            c.Document.Text = File.ReadAllText(@"C:\workspace\CSharp\VsProjects\XEdit\XEdit\CSharpCodeAnalyzer.cs");
            c.Dock = DockStyle.Fill;
            c.Theme = new TextAreaTheme() { TextColor = Color.White, BackgroundColor = Color.Black, CaretColor = Color.White, LineNumberBackgroundColor = Color.Black, CaretLineNumberBackgroundColor = Color.Black, CaretLineNumberColor = Color.Lime, LineNumberColor = Color.Lime };
            c.Highlighters.Add(new KeywordsHighlighter(new CharStyle(Color.Aqua, EditorFontStyle.Bold), true, "using", "public", "static", "void", "string", "int", "if", "else", "while", "for", "class"));
            c.Highlighters.Add(new QuoteHighlighter(Color.Yellow, false, "\"", "\""));
            c.Highlighters.Add(new HeaderHighlighter(Color.Green, "//"));
            c.Highlighters.Add(new QuoteHighlighter(Color.Magenta, false, "#include<", ">", self:false, outStyle: Color.Yellow));
            c.Highlighters.Add(new QuoteHighlighter(Color.Magenta, false, "using ", ";", self: false));
            Controls.Add(c);
        }
    }
}