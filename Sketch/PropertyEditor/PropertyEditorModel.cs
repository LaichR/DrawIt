using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sketch.PropertyEditor
{
    public class PropertyEditorModel: BindableBase, ITemplateProvider
    {
        ObservableCollection<PropertyValueModel> _properties = new ObservableCollection<PropertyValueModel>();

        DataTemplateSelector _cellTemplateSelector = new PropertyEditTemplateSelector();
        DataTemplateSelector _cellEditingTemplateSelector = new PropertyEditTemplateSelector();

        const string NoObjSelectedLabel = "No Object Selected";

        object _object = null;
        string _objectTypeName = NoObjSelectedLabel;

        public PropertyEditorModel() { }


        public void Wrap(object obj)
        {
            
            _object = obj;
            foreach( var m in _properties) { m.ReleaseBinding(); } // avoid memory leaks
            _properties.Clear();
            _objectTypeName = NoObjSelectedLabel;
            if (_object != null)
            {
                ObjectTypeName = _object.GetType().Name;
                foreach (var pi in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var attrs = pi.GetCustomAttributes<BrowsableAttribute>(true);
                    if (attrs.Any())
                    {
                        var attr = attrs.First();
                        if (attr.Browsable)
                        {
                            var pvModel = new PropertyValueModel(this, obj, pi);
                            _properties.Add(pvModel);
                        }
                    }
                }
            }
            else
            {
                RaisePropertyChanged("ObjectTypeName");
            }
        }

        public string ObjectTypeName
        {
            get => _objectTypeName;
            private set
            {
                SetProperty<string>(ref _objectTypeName, value);
            }
        }

        public ObservableCollection<PropertyValueModel> ObjectProperties
        {
            get => _properties;
        }

        public DataTemplateSelector CellTemplateSelector
        {
            get => _cellTemplateSelector;
        }

        public DataTemplateSelector CellEditingTemplateSelector
        {
            get => _cellTemplateSelector;
        }
    }
}
