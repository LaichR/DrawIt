using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using Bluebottle.Base.Interfaces;
using Bluebottle.Base.Behaviors;

namespace Bluebottle.Base.Controls.InfoChannelViewer.ViewModel
{
    public enum Level
    {
        Off,
        Fatal,
        Error,
        Warning,
        Info,
        Debug,
        Trace
    };


    public class InfoItemModel
    {

        public bool IsEven
        {
            get;
            internal set;
        }

        public Level Level
        {
            get;
            set;
        }

        public int LevelOrdinal
        {
            get
            {
                return (int)Level;
            }
        }

        public string Origin
        {
            get;
            set;
        }

        public DateTime TimeStamp
        {
            get;
            set;
        }

        public string FirstLineOfMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    var msg = Message.Split('\n').First();
                    if( msg.Length > 80)
                    {
                        msg = string.Format("{0} ..", msg.Substring(0, 80));
                    }
                    return msg;
                }
                return "";
            }
        }

        public string Message
        {
            get;
            set;
        }

    }

    public class InfoChannelViewerViewModel: BindableBase
    {
        ObservableCollection<InfoItemModel> _infoItemCollection = new ObservableCollection<InfoItemModel>();
        List<ICommandDescriptor> _contextMenuCommands;
        int _maxNrOfItems;

        public InfoChannelViewerViewModel(int maxNrOfItems):base()
        {
            _maxNrOfItems = maxNrOfItems;
            _contextMenuCommands = new List<ICommandDescriptor>
            {
                new CommandDescriptor {Name = "Clear", Command = new DelegateCommand( ()=>_infoItemCollection.Clear()), ToolTip="Clear message log!" }
            };
        }

        public IList<ICommandDescriptor> ContextMenu
        {
            get
            {
               return _contextMenuCommands;

            }
        }

        public InfoItemModel SelectedItem
        {
            get;
            set;
        }

        public ObservableCollection<InfoItemModel> InfoItemCollection
        {
            get
            {
                return _infoItemCollection;
            }
        }

        public void AddInfoItem( InfoItemModel item )
        {
            
            if(_infoItemCollection.Count() >= _maxNrOfItems)
            {
                InfoItemModel oldest = null;
                foreach( var infoItem in _infoItemCollection)
                {
                    if( infoItem.Level > Level.Warning)
                    {
                        oldest = infoItem;
                        break;
                    }
                }
                if( oldest != null)
                    _infoItemCollection.Remove(oldest);
            }
            bool isEven = true;
            if( _infoItemCollection.Count > 0)
            {
                isEven = !_infoItemCollection.Last().IsEven;
            }
            item.IsEven = isEven;
            _infoItemCollection.Add(item);
        }


    }
}
