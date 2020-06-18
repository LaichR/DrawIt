using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Bluebottle.Base.Controls.CommunicationBox;

namespace Bluebottle.Base
{
    public static class BasicGuiOperations
    {
        public enum UserChoice { Yes, No, Cancel }

        public static string GetUserInput()
        {
            return GetUserInput("");
        }

        public static string GetUserInput(string title)
        {
            return GetUserInput(title, "");
        }

        public static string GetUserInput(string title, string defaultText)
        {
            var box = new InputBox(title);
            box.InputText = defaultText;
            if (box.ShowDialog() == DialogResult.OK)
            {
                return box.InputText;
            }
            return null;
        }

        public static UserChoice GetUserChoice(string question)
        {
            var box = new ChoiceBox(question);
            var res = box.ShowDialog();
            if (res == DialogResult.Yes || res == DialogResult.OK)
                return UserChoice.Yes;
            if (res == DialogResult.No)
                return UserChoice.No;
            return UserChoice.Cancel;
        }

        public static void Notify ( string message )
        {
            new NotificationBox ( message ).ShowDialog ();
        }

        /// <summary>
        /// Shows a window to the user containing the content of a HTML file or HTML text.
        /// </summary>
        /// <param name="title">The title of the displayed window</param>
        /// <param name="html">A HTML string or a path to a HTM/HTML file.</param>
        /// <param name="width">If bigger than 0, the HTML content viewer width will be set to this value</param>
        /// <param name="height">If bigger than 0, the HTML content viewer height will be set to this value</param>
        /// <returns>True, if the user pressed the OK button to close the window. False otherwise</returns>
        public static bool Notify(string title, string html, int width = -1, int height=-1)
        {
            return (bool)Notify(title, html, null, width, height)[0];
        }


        /// <summary>
        /// Shows a window to the user containing the content of a HTML file or HTML text.
        /// </summary>
        /// <param name="title">The title of the displayed window</param>
        /// <param name="html">A HTML string or a path to a HTM/HTML file.</param>
        /// <param name="action">If not null, this action is performed and when it's done, the windows closes with result "OK".</param>
        /// <param name="width">If bigger than 0, the HTML content viewer width will be set to this value</param>
        /// <param name="height">If bigger than 0, the HTML content viewer height will be set to this value</param>
        /// <returns>an object array containing:
        /// at element 0: True, if the user pressed the OK button to close the window. False otherwise
        /// at element 1: the result of the provided action, if any
        /// at element 2: the exception caused by the action, if any
        /// </returns>
        public static object[] Notify(string title, string html, Func<object> action, int width = -1, int height = -1)
        {
            var htmlText = html.Trim();
            object result = null;
            bool success = false;
            Exception error = null;

            if (htmlText.EndsWith(".html") || htmlText.EndsWith(".htm"))
            {
                HIMS.Services.Core.Assert.FileExists(htmlText);
                htmlText = File.ReadAllText(html);
            }

            //HIMS.Services.Threading.GuiOperations.Run(() =>
            //{
            //    var htmlViewer = new Controls.CommunicationBox.HtmlViewer(title, htmlText, action, new System.Drawing.Size(width, height));
            //    htmlViewer.ShowDialog();
            //    success = htmlViewer.DialogResult == DialogResult.OK;
            //    result = htmlViewer.ActionResult;
                //error = htmlViewer.ActionException;
            //}, true);

            return new object[] { success, result, error } ;
        }
    }
}
