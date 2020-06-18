using System;
using System.Windows.Forms;

namespace Bluebottle.Base.Controls.CommunicationBox
{
    public partial class NotificationBox : Form
    {
        public NotificationBox(string text)
        {
            InitializeComponent();
            MessageLabel.Text = text;
            Text = "Notification";

            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.AcceptButton = okButton;
            this.CancelButton = okButton;
            okButton.Focus();
        }

        public NotificationBox(string title, string text)
        {
            InitializeComponent();
            MessageLabel.Text = text;
            Text = title;
        }

        private void okClick ( object sender, EventArgs e )
        {
            Close ();
        }
    }
}
