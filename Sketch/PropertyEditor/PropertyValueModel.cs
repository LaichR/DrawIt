
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UI.Utilities;

namespace Sketch.PropertyEditor
{
    public class PropertyValueModel:BindableModel
    {
        readonly PropertyInfo _propertyInfo;
        readonly object _parent;
        readonly string[] _valueNames;
        readonly ITemplateProvider _templateProvider;
        string _displayName;
        
        bool _getValue = false;
        readonly RectangleWrapper _rectangleWrapper;
        public PropertyValueModel(ITemplateProvider templateProvider, object parent, PropertyInfo propertyInfo)
        {
            _parent = parent;
            _templateProvider = templateProvider;
            _propertyInfo = propertyInfo;
            _displayName = propertyInfo.Name;
            if( parent is  INotifyPropertyChanged bindable)
            {
                bindable.PropertyChanged += Bindable_PropertyChanged;
            }
            if (_propertyInfo.PropertyType == typeof(Rect))
            {
                _rectangleWrapper = new RectangleWrapper((Rect)_propertyInfo.GetValue(_parent));
                _rectangleWrapper.PropertyChanged += Wrapper_PropertyChanged;
            }
            else if(_propertyInfo.PropertyType == typeof(FontWeight))
            {
                _valueNames = typeof(FontWeights).GetProperties(BindingFlags.Static|BindingFlags.Public).Select<PropertyInfo, string>((x) => x.Name).ToArray();
            }
            else if(_propertyInfo.PropertyType.IsEnum)
            {
                _valueNames = Enum.GetNames(_propertyInfo.PropertyType);
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


        public string[] ValueNames => _valueNames;

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
                        _rectangleWrapper.Assign((Rect)value);
                        value = _rectangleWrapper;
                    }
                    else if( _propertyInfo.PropertyType == typeof(FontWeight))
                    {
                        value = new FontWeightConverter().ConvertToInvariantString(
                            _propertyInfo.GetValue(_parent));
                    }
                    else if( _propertyInfo.PropertyType.IsEnum)
                    {
                        value = value.ToString();
                    }
                    else if(_propertyInfo.PropertyType.IsGenericType )
                    {
                        if( _propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
                        {
                            var args = _propertyInfo.PropertyType.GetGenericArguments();
                            if( args.Any())
                            {
                                //var collectionMemberType = args.First();

                                //if( collectionMemberType.GetInterface(typeof(INotifyPropertyChanged).FullName) != null )
                                //{
                                    
                                //}
                            }
                        }
                    }
                    _getValue = false;
                }
                value.GetType();
                return value;
            }

            set
            {
                if( _propertyInfo.PropertyType.IsEnum )
                {
                    value = Enum.Parse(_propertyInfo.PropertyType, value?.ToString(), true);
                }
                else if( _propertyInfo.PropertyType == typeof(FontWeight))
                {
                    value = new FontWeightConverter().ConvertFromString(value?.ToString());
                }
                _propertyInfo.SetValue(_parent, value);
            }
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
            if( _parent is INotifyPropertyChanged bindable)
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
