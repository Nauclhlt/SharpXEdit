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
            TextArea a = new TextArea();
            a.BackColor = Color.White;
            a.Bounds = new Rectangle(120, 120, 500, 500);
            a.Font = new Font("Consolas", 22);
            a.Document.Text = "using System;\r\nusing System.Linq;";
            Controls.Add(a);
        }
    }
}