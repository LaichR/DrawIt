using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows.Media;

namespace Bluebottle.Base.Controls.AboutBox.ViewModel
{
    public class ViewModel: BindableBase
    {
        string _processName;
        string _version;
        string _product;
        string _originator;

        ImageSource _icon;
        DelegateCommand _onClose;

        public ViewModel( Action closeAction )
        {
            _onClose = new DelegateCommand(closeAction);
            var main = System.Reflection.Assembly.GetEntryAssembly();

            var attr1 = main.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)[0]
                as System.Reflection.AssemblyInformationalVersionAttribute;
            _version = attr1.InformationalVersion;

            var attr2 = main.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false)[0]
                as System.Reflection.AssemblyProductAttribute;
            _product = attr2.Product;

            var attr3 = main.GetCustomAttributes(typeof(System.Reflection.AssemblyCompanyAttribute), false)[0]
                as System.Reflection.AssemblyCompanyAttribute;
            _originator = attr3.Company;

            var runtime = main.ImageRuntimeVersion;
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            _processName = proc.ProcessName;
            _icon = ToBitmapSource.Icon2BitmapSource(System.Drawing.Icon.ExtractAssociatedIcon(main.Location));
        }

        public DelegateCommand CloseDialog
        {
            get
            {
                return _onClose;
            }
        }

        public ImageSource ApplicationIcon
        {
            get
            {
                return _icon;
            }
        }

        public string Title
        {
            get
            {
                return string.Format("About {0}", _processName);
            }
        }

        public string Product
        {
            get
            {
                return _product;
            }
        }

        public string Originator
        {
            get
            {
                return _originator;
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
        }


    }
}
