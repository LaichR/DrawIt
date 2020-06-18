namespace Bluebottle.Base.Controls.CommunicationBox
{
    partial class InputBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._textBox = new System.Windows.Forms.TextBox();
            this._buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _textBox
            // 
            this._textBox.Location = new System.Drawing.Point(12, 12);
            this._textBox.Name = "_textBox";
            this._textBox.Size = new System.Drawing.Size(283, 20);
            this._textBox.TabIndex = 0;
            this._textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.@__onKeyPress);
            // 
            // _buttonOk
            // 
            this._buttonOk.Location = new System.Drawing.Point(220, 38);
            this._buttonOk.MaximumSize = new System.Drawing.Size(75, 23);
            this._buttonOk.MinimumSize = new System.Drawing.Size(75, 23);
            this._buttonOk.Name = "_buttonOk";
            this._buttonOk.Size = new System.Drawing.Size(75, 23);
            this._buttonOk.TabIndex = 3;
            this._buttonOk.Text = "Ok";
            this._buttonOk.UseVisualStyleBackColor = true;
            this._buttonOk.Click += new System.EventHandler(this.onOk_Click);
            // 
            // InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 70);
            this.Controls.Add(this._buttonOk);
            this.Controls.Add(this._textBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(323, 108);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(323, 108);
            this.Name = "InputBox";
            this.ShowIcon = false;
            this.Text = "Input box";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _textBox;
        private System.Windows.Forms.Button _buttonOk;
    }
}