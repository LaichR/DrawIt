using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sketch.PropertyEditor
{
    public class PropertyValueModel:BindableBase
    {
        readonly PropertyInfo _propertyInfo;
        readonly object _parent;
        readonly ITemplateProvider _templateProvider;
        string _displayName;
        bool _getValue = false;
        RectangleWrapper _rectangleWrapper;
        public PropertyValueModel(ITemplateProvider templateProvider, object parent, PropertyInfo propertyInfo)
        {
            _parent = parent;
            _templateProvider = templateProvider;
            _propertyInfo = propertyInfo;
            _displayName = propertyInfo.Name;
            if( parent is BindableBase bindable)
            {
                bindable.PropertyChanged += Bindable_PropertyChanged;
            }
            if (_propertyInfo.PropertyType == typeof(Rect))
            {
                _rectangleWrapper = new RectangleWrapper((Rect)_propertyInfo.GetValue(_parent));
                _rectangleWrapper.PropertyChanged += Wrapper_PropertyChanged;
            }
            if( parent.GetType().IsEnum)
            {

            }
        }

        private void Bindable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName == this._propertyInfo.Name)
            {
                RaisePropertyChanged(nameof(Value));
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                SetProperty<string>(ref _displayName, value);
            }
        }

        public Type PropertyType => _propertyInfo.PropertyType;

   

        public object Value
        {
            get
            {
                object value = null;
                if (!_getValue)
                {
                    _getValue = true;

                    value = _propertyInfo.GetValue(_parent);
                    if (_propertyInfo.PropertyType == typeof(Rect))
                    {
                        _rectangleWrapper.Assign((Rect)value, true);
                        value = _rectangleWrapper;
                    }
                    _getValue = false;
                }
                return value;
            }

            set => _propertyInfo.SetValue(_parent, value);
        }

        private void Wrapper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( !_getValue && sender is RectangleWrapper wrapper && _propertyInfo.PropertyType == typeof(Rect))
            {
                _propertyInfo.SetValue(_parent, wrapper.Wrapped);
                RaisePropertyChanged("Value");
            }
        }

        public DataTemplateSelector CellTemplateSelector
        {
            get => _templateProvider.CellTemplateSelector;
        }

        public DataTemplateSelector CellEditingTemplateSelector
        {
            get => _templateProvider.CellEditingTemplateSelector;
        }

        public void ReleaseBinding()
        {
            if( _parent is BindableBase bindable)
            {
                bindable.PropertyChanged -= Bindable_PropertyChanged;
            }
            if (_rectangleWrapper != null)
            {
                _rectangleWrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }
        }


    }
}
