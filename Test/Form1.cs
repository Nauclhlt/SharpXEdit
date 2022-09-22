using SharpXEdit;

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
            Controls.Add(c);
        }
    }
}