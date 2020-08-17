using Prism.Mvvm;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sketch.PropertyEditor
{
    public class RectangleWrapper:BindableBase
    {
        Rect _rectangle;
        string _xValue;
        string _yValue;
        string _widthValue;
        string _heightValue;
        public RectangleWrapper(Rect r)
        {
            _rectangle = r;
            _xValue = r.X.ToString();
            _yValue = r.Y.ToString();
            _widthValue = r.Width.ToString();
            _heightValue = r.Height.ToString();
        }

        public void Assign(RectangleWrapper wrapper, bool notify)
        {
            _rectangle = wrapper._rectangle;
            //if (notify)
            //{
            //    RaisePropertyChanged("XValue");
            //    RaisePropertyChanged("YValue");
            //    RaisePropertyChanged("WidthValue");
            //    RaisePropertyChanged("HeightValue");
            //}
        }

        public void Assign(Rect other, bool notify)
        {
            _rectangle = other;
            XValue = _rectangle.X.ToString();
            YValue = _rectangle.Y.ToString();
            WidthValue = _rectangle.Width.ToString();
            HeightValue = _rectangle.Height.ToString();
        }

        public static implicit operator Rect(RectangleWrapper wrapper)
        {
            return wrapper.Wrapped;
        }

        public Rect Wrapped
        {
            get => _rectangle;
        }

        public string XValue
        {
            get => _xValue;
            set
            {
                _rectangle.X = double.Parse(value);
                SetProperty<string>(ref _xValue, value);
                
            }
        }

        public string YValue
        {
            get => _yValue;
            set
            {
                _rectangle.Y = double.Parse(value);
                SetProperty<string>(ref _yValue, value);
                
            }
        }

        public string WidthValue
        {
            get => _widthValue;
            set
            {
                _rectangle.Width = double.Parse(value);
                SetProperty<string>(ref _widthValue, value);
               
            }
        }

        public string HeightValue
        {
            get => _heightValue;
            set
            {
                _rectangle.Height = double.Parse(value);
                SetProperty<string>(ref _heightValue, value);
                
            }
        }

    }
}
