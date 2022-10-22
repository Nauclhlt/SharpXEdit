namespace SharpXEdit
{
    partial class XTextEditor
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            SharpXEdit.TextAreaTheme textAreaTheme2 = new SharpXEdit.TextAreaTheme();
            this._textArea = new SharpXEdit.TextArea();
            this.SuspendLayout();
            // 
            // _textArea
            // 
            this._textArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textArea.LineNumberWidth = 80;
            this._textArea.Location = new System.Drawing.Point(0, 0);
            this._textArea.Name = "_textArea";
            this._textArea.ScrollSpeed = 3;
            this._textArea.Size = new System.Drawing.Size(500, 400);
            this._textArea.TabIndex = 0;
            this._textArea.Text = "textArea1";
            textAreaTheme2.BackgroundColor = System.Drawing.Color.White;
            textAreaTheme2.BracketHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            textAreaTheme2.CaretColor = System.Drawing.Color.Black;
            textAreaTheme2.CaretLineNumberBackgroundColor = System.Drawing.Color.White;
            textAreaTheme2.CaretLineNumberColor = System.Drawing.Color.Gray;
            textAreaTheme2.LineNumberBackgroundColor = System.Drawing.Color.White;
            textAreaTheme2.LineNumberColor = System.Drawing.Color.Gray;
            textAreaTheme2.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(135)))), ((int)(((byte)(206)))), ((int)(((byte)(235)))));
            textAreaTheme2.TextColor = System.Drawing.Color.Black;
            this._textArea.Theme = textAreaTheme2;
            // 
            // XTextEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Blue;
            this.Controls.Add(this._textArea);
            this.Font = new System.Drawing.Font("Courier New", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "XTextEditor";
            this.Size = new System.Drawing.Size(500, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private TextArea _textArea;
    }
}
