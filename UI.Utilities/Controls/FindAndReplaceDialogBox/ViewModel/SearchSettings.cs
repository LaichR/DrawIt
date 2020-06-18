using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace Bluebottle.Base.Controls.FindAndReplaceDialogBox.ViewModel
{
    public class SearchSettings: BindableBase, IXmlSerializable, IEnumerable<Setting>
    {
        string _name;
        List<Setting> _children;

        public SearchSettings() //Needed by the ConfigurableProperty mechanism
        {
            _children = new List<Setting>();
        }

        public SearchSettings(string name, List<string> settingNames) 
        {
            _name = name;
            _children = new List<Setting>();
            foreach (var v in settingNames) 
            {
                _children.Add(new Setting(v));
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public List<Setting> Children
        {
            get { return _children; }
            set
            { 
                _children = value;
                OnPropertyChanged("Children");
            }
        } 
        
        public void SetChildrenState(List<Setting> newChildren)
        {
            foreach(var child in _children)
            {
                var newChild = newChildren.Where((x) => x.Name == child.Name);
                if (newChild.Count() > 0)
                    child.IsChecked = newChild.First().IsChecked;
            }
        }

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadToFollowing("SearchSettingsList");
            _name = reader.GetAttribute("Name");
            _children.Clear();

            while (reader.ReadToFollowing("SettingItem"))
            {
                _children.Add(new Setting(reader.GetAttribute("Name"), Convert.ToBoolean(reader.GetAttribute("Checked"))));
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlDocument doc = new XmlDocument();
            var root = doc.CreateElement("SearchSettingsList");
            doc.AppendChild(root);
            var rootNameAttr = doc.CreateAttribute("Name");
            rootNameAttr.Value = Name.ToString();
            root.Attributes.Append(rootNameAttr);

            foreach(var setting in Children)
            {
                var settingNode = doc.CreateElement("SettingItem");
                
                var checkedAttr = doc.CreateAttribute("Checked");
                checkedAttr.Value = setting.IsChecked.ToString();
                settingNode.Attributes.Append(checkedAttr);
             
                var nameAttr = doc.CreateAttribute("Name");
                nameAttr.Value = setting.Name.ToString();
                settingNode.Attributes.Append(nameAttr);

                root.AppendChild(settingNode);
            }
            doc.WriteContentTo(writer);
        }

        #endregion

        #region IEnumerator

        public IEnumerator<Setting> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        #endregion
    }
}
