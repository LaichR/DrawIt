using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using UI.Utilities;
using Sketch.Helper;

namespace Sketch.View.ColorPicker
{
    internal class ColorPaletteModel: BindableModel
    {
        Color _customColor = Colors.Snow;
        Color _defaultColor = Colors.Wheat;
        readonly IColorSelectionTarget _selectionTarget = null;

        readonly DelegateCommand _selectRed;
        readonly DelegateCommand _selectYellow;
        readonly DelegateCommand _selectOrange;
        readonly DelegateCommand _selectBlue;
        readonly DelegateCommand _selectGreen;
        readonly DelegateCommand _selectCustomColor;
        readonly DelegateCommand _editCustomColor;
        readonly DelegateCommand _selectDefaultColor;
        readonly DelegateCommand _editDefaultColor;

        public ColorPaletteModel( IColorSelectionTarget selectionTarget )
        {
            _selectionTarget = selectionTarget;
            _selectRed = new DelegateCommand(() => _selectionTarget.SetColor(Colors.Red));
            _selectOrange = new DelegateCommand(() => _selectionTarget.SetColor(Colors.Orange));
            _selectYellow = new DelegateCommand(() => _selectionTarget.SetColor(Colors.Yellow));
            _selectBlue = new DelegateCommand(() => _selectionTarget.SetColor(Colors.Blue));
            _selectGreen = new DelegateCommand(() => _selectionTarget.SetColor(Colors.Green));
            _selectCustomColor = new DelegateCommand(() => _selectionTarget.SetColor(CustomColor));
            
            _selectDefaultColor = new DelegateCommand(() => _selectionTarget.SetColor(DefaultColor));

            _editCustomColor = new DelegateCommand(() =>
                {
                    EditColor(ref _customColor);
                    RaisePropertyChanged("CustomBrush");
                });

            _editDefaultColor = new DelegateCommand(() =>
            {
                EditColor(ref _defaultColor);
                RaisePropertyChanged("DefaultBrush");
            });
        }

        public ICommand SelectRed
        {
            get
            {
                return _selectRed;
            }
        }
        public ICommand SelectYellow
        {
            get
            {
                return _selectYellow;
            }
        }

        public ICommand SelectOrange
        {
            get
            {
                return _selectOrange;
            }
        }

        public ICommand SelectGreen
        {
            get
            {
                return _selectGreen;
            }
        }

        public ICommand SelectBlue
        {
            get
            {
                return _selectBlue;
            }
        }

        public ICommand SelectCustomColor
        {
            get
            {
                return _selectCustomColor;
            }
        }
        public ICommand SelectDefaultColor
        {
            get
            {
                return _selectDefaultColor;
            }
        }
        public ICommand EditCustomColor
        {
            get
            {
                return _editCustomColor;
            }
        }
        public ICommand EditDefaultColor
        {
            get
            {
                return _editDefaultColor;
            }
        }

        public Color CustomColor
        {
            get
            {
                return _customColor;
            }
            set
            {
                SetProperty<Color>(ref _customColor, value);
                RaisePropertyChanged("CustomBrush");
            }
        }

        public Brush CustomBrush
        {
            get
            {
                return new SolidColorBrush(_customColor);
            }
        }

        public Color DefaultColor
        {
            get
            {
                return _defaultColor;
            }
            set
            {
                SetProperty<Color>(ref _defaultColor, value);
                RaisePropertyChanged("DefaultBrush");
            }
        }

        public Brush DefaultBrush
        {
            get
            {
                return new SolidColorBrush(_defaultColor);
            }

        }

        void EditColor( ref Color brushToEdit )
        {
            var colorPicker = new ColorPickerDialog()
            {
                StartingColor = brushToEdit
            };
            
            bool? result = colorPicker.ShowDialog();

            if (result ?? true)
            {
                brushToEdit = colorPicker.SelectedColor;
            }
        }

        

    }
}
