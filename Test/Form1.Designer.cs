namespace Test
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SharpXEdit.TextAreaTheme textAreaTheme1 = new SharpXEdit.TextAreaTheme();
            SharpXEdit.TextAreaTheme textAreaTheme2 = new SharpXEdit.TextAreaTheme();
            this.xTextEditor1 = new SharpXEdit.XTextEditor();
            this.SuspendLayout();
            // 
            // xTextEditor1
            // 
            textAreaTheme1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            textAreaTheme1.BracketHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            textAreaTheme1.CaretColor = System.Drawing.Color.Black;
            textAreaTheme1.CaretLineNumberBackgroundColor = System.Drawing.Color.White;
            textAreaTheme1.CaretLineNumberColor = System.Drawing.Color.Gray;
            textAreaTheme1.LineNumberBackgroundColor = System.Drawing.Color.White;
            textAreaTheme1.LineNumberColor = System.Drawing.Color.White;
            textAreaTheme1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(255)))));
            textAreaTheme1.TextColor = System.Drawing.Color.Black;
            this.xTextEditor1.AutoCompleteTheme = textAreaTheme1;
            this.xTextEditor1.BackColor = System.Drawing.Color.Blue;
            this.xTextEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xTextEditor1.HScrollBar = true;
            this.xTextEditor1.LineNumberWidth = 80;
            this.xTextEditor1.Location = new System.Drawing.Point(0, 0);
            this.xTextEditor1.Name = "xTextEditor1";
            this.xTextEditor1.ScrollSpeed = 3;
            this.xTextEditor1.Size = new System.Drawing.Size(800, 450);
            this.xTextEditor1.TabIndex = 0;
            textAreaTheme2.BackgroundColor = System.Drawing.Color.White;
            textAreaTheme2.BracketHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            textAreaTheme2.CaretColor = System.Drawing.Color.Black;
            textAreaTheme2.CaretLineNumberBackgroundColor = System.Drawing.Color.White;
            textAreaTheme2.CaretLineNumberColor = System.Drawing.Color.Gray;
            textAreaTheme2.LineNumberBackgroundColor = System.Drawing.Color.White;
            textAreaTheme2.LineNumberColor = System.Drawing.Color.Gray;
            textAreaTheme2.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(135)))), ((int)(((byte)(206)))), ((int)(((byte)(235)))));
            textAreaTheme2.TextColor = System.Drawing.Color.Black;
            this.xTextEditor1.Theme = textAreaTheme2;
            this.xTextEditor1.VScrollBar = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.xTextEditor1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SharpXEdit.XTextEditor xTextEditor1;
    }
}