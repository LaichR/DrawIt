using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bluebottle.Base.Controls.CommunicationBox
{
    public partial class HtmlViewer : Form
    {
        Size _size;
        Size _browserOriginalSize;
        Func<object> _closeAction;
        IAsyncResult _closeActionHandle;

        public HtmlViewer(string title, string htmlText) : this(title, htmlText, null, new Size(-1, -1))
        {
        }

        public HtmlViewer(string title, string htmlText, Func<object> closeAction) : this(title, htmlText, closeAction, new Size(-1, -1))
        {
        }

        public HtmlViewer(string title, string htmlText, Size size) : this(title, htmlText, null, size)
        {
        }

        public HtmlViewer(string title, string htmlText, Func<object> closeAction, Size size)
        {
            InitializeComponent();

            _size = size;
            _closeAction = closeAction;

            DialogResult = DialogResult.Cancel;
            Text = title;
            KeyUp += OnKeyUp;
            FormClosing += OnFormClosing;

            _browserOriginalSize = new Size(_browser.Size.Width, _browser.Size.Height);

            _browser.DocumentText = htmlText;

            if (_closeAction == null)
            {
                _cancel.Click += OnCancelClick;
                _buttonOk.Click += OnOkClick;
                _buttonOk.KeyUp += OnKeyUp;
                _buttonOk.Focus();
            }
            else
            {
                _cancel.Visible = false;
                _buttonOk.Visible = false;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_closeActionHandle != null && !_closeActionHandle.IsCompleted)
            {
                e.Cancel = true;
                BasicGuiOperations.Notify("Please perform the requested action to close this window.");
            }
        }

        public object ActionResult
        {
            get; private set;
        }

        public Exception ActionException
        {
            get; private set;
        }

        void OnCloseActionDone(IAsyncResult ar)
        {
            var func = (Func<object>)ar.AsyncState;
            var closeDialogAction = new Action<object, EventArgs>(OnOkClick);

            try
            {
                ActionResult = func.EndInvoke(_closeActionHandle);
            }
            catch (Exception e)
            {
                ActionException = e;
                closeDialogAction = new Action<object, EventArgs>(OnActionException);
            }
            _closeAction = null;

            if (this.InvokeRequired)
            {
                Invoke(closeDialogAction, this, EventArgs.Empty);
            }
            else
            {
                closeDialogAction(this, EventArgs.Empty);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                OnCancelClick(sender, EventArgs.Empty);

            if ((char)e.KeyValue == '\r' && _closeAction == null)
                OnOkClick(sender, EventArgs.Empty);
        }

        private void OnActionException(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnOkClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnDocumentCompleated(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Size newSize = _browser.Document.Body.ScrollRectangle.Size;
            
            if (_size.Width > 0)
            {
                newSize.Width = _size.Width;
            }
            if (_size.Height > 0)
            {
                newSize.Height = _size.Height;
            }
            Size scrollBars = new Size(System.Windows.Forms.SystemInformation.VerticalScrollBarWidth, System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight);
            Size += newSize - _browserOriginalSize + scrollBars;
            _browser.Size = newSize + scrollBars;

            if (_closeAction != null)
            {
                _closeActionHandle = _closeAction.BeginInvoke(OnCloseActionDone, _closeAction);
            }
        }
    }
}
