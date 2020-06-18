using System;
using System.Windows.Forms;

namespace Bluebottle.Base.Controls.CommunicationBox
{
    public partial class ChoiceBox : Form
    {
        public ChoiceBox(string text)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            MessageLabel.Text = text;
            Text = "Notification";

            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            AcceptButton = yesButton;
            CancelButton = cancelButton;
            cancelButton.Focus();
        }

        public ChoiceBox(string title, string text)
        {
            InitializeComponent();
            MessageLabel.Text = text;
            Text = title;
        }

        private void __onKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private void OnYesClick(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void OnNoClick(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
