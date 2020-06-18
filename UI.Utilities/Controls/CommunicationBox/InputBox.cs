using System;
using System.Windows.Forms;

namespace Bluebottle.Base.Controls.CommunicationBox
{
    public partial class InputBox : Form
    {
        string _txt;
        public string InputText
        {
            get { return _txt; }
            set 
            { 
                _textBox.Text = value;
                _txt = value;
            }
        }

        public InputBox(string title)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            if (!String.IsNullOrEmpty(title))
                this.Text = title;
        }

        private void onOk_Click(object sender, EventArgs e)
        {
            _txt = _textBox.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void __onKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();

            if (e.KeyChar == '\r')
                onOk_Click(sender, EventArgs.Empty);
        }
    }
}
