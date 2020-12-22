using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Converters;
using Sketch.Interface;
using Sketch.Models;
using UI.Utilities;

namespace Sketch.Models.BasicItems
{


    public enum FontStyle
    {
        Normal,
        Oblique,
        Italic
    }

    [Serializable]
    [DoNotConsiderForConnectorRouting]
    [AllowableConnector(typeof(StrightLineConnector))]
    public class FreeTextModel: ConnectableBase
    {
        static readonly Dictionary<BasicItems.FontStyle, System.Windows.FontStyle> _fontStyleDictionary = new Dictionary<FontStyle, System.Windows.FontStyle>
        {
            {BasicItems.FontStyle.Italic, FontStyles.Italic},
            {BasicItems.FontStyle.Normal, FontStyles.Normal },
            {BasicItems.FontStyle.Oblique, FontStyles.Oblique }
        };

     

        [PersistentField((int)ModelVersion.V_1_1, "BorderThickness")]
        double _borderThickness = 0;

        [PersistentField((int)ModelVersion.V_1_1, "FontFamily")]
        string _fontFamily = "Arial";

        [PersistentField((int)ModelVersion.V_1_1, "FontSize")]
        double _fontSize = 12;

        [PersistentField((int)ModelVersion.V_1_1, "FontStyle")]
        FontStyle _fontStyle = FontStyle.Normal;

        [PersistentField((int)ModelVersion.V_1_1, "TextAlignement")]
        TextAlignment _textAlignment = TextAlignment.Center;

        [PersistentField((int)ModelVersion.V_1_1, "FontWeight")]
        int _fontWeightNum;

        FontWeight _fontWeight;

        [PersistentField((int)ModelVersion.V_1_1, "BorderColor")]
        readonly SerializableColor _borderColor = new SerializableColor() { Color = Colors.Black };
        //Brush _borderBrush;

        [PersistentField((int)ModelVersion.V_1_1, "TextColor")]
        readonly SerializableColor _textColor = new SerializableColor() { Color = Colors.Black };
        //Brush _textBrush;

        new const double DefaultHeight = 30;
        new const double DefaultWidth = 75;

        Typeface _typeface = DefaultFont;

        public FreeTextModel(Point p)
            : base(p, new Size(DefaultWidth, DefaultHeight), "a text",
                 Colors.White)
        {
            CanChangeSize = true;
        }

        protected FreeTextModel(SerializationInfo info, StreamingContext context):base(info, context) 
        { }

        [Browsable(true)]
        public double FontSize
        {
            get => _fontSize;
            set
            {
                SetProperty<double>(ref _fontSize, value);

                AdjustBounds();
            }
        }

        [Browsable(true)]
        public double BorderThickness
        {
            get => _borderThickness;
            set => SetProperty<double>(ref _borderThickness, value);
        }

        [Browsable(true)]
        
        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                SetProperty<string>(ref _fontFamily, value);
                _typeface = new Typeface(FontFamily);
                AdjustBounds();
            }
        }

        [Browsable(true)]
        [TypeConverter(typeof(FontStyleConverter))]
        public FontStyle FontStyle
        {
            get => _fontStyle;
            set
            {
                SetProperty<FontStyle>(ref _fontStyle, value);
                RaisePropertyChanged(nameof(ShownFontStyle));
                _typeface = new Typeface(
                    new FontFamily(FontFamily),
                    _fontStyleDictionary[_fontStyle],
                    FontWeight, FontStretches.Normal);
                AdjustBounds();
            }
        }

        public System.Windows.FontStyle ShownFontStyle
        {
            get => _fontStyleDictionary[FontStyle];
        }

        [Browsable(true)]
        public System.Windows.TextAlignment TextAlignment
        {
            get => _textAlignment;
            set => SetProperty<TextAlignment>(ref _textAlignment, value);

        }

        [Browsable(true)]
        public System.Windows.FontWeight FontWeight
        {
            get => _fontWeight;
            set => SetProperty<FontWeight>(ref _fontWeight, value);

        }

        [Browsable(true)]
        public Color BorderColor
        {
            get => _borderColor.Color;
            set
            {
                _borderColor.Color = value;
                RaisePropertyChanged(nameof(BorderColor));
                RaisePropertyChanged(nameof(BorderBrush));
            }
        }
        [Browsable(true)]
        public override string Label 
        { 
            get => base.Label;
            set
            {
                base.Label = value;
                AdjustBounds();
            }
        }

        public Brush BorderBrush
        {
            get => _borderColor.Brush; 
        }

        
        public Brush TextBrush
        {
            get => _textColor.Brush;
        }

        [Browsable(true)]
        public Color TextColor
        {
            get => _textColor.Color;
            set
            {
                _textColor.Color = value;
                RaisePropertyChanged(nameof(TextColor));
                RaisePropertyChanged(nameof(TextBrush));
            }
        }

        protected override Rect ComputeLabelArea(string label)
        {
            if (!string.IsNullOrEmpty(Label))
            {
                return new Rect(
                    new Point(5, 5),
                    ComputeFormattedTextSize(label, _typeface, FontSize, 10, 15));
            }
            return Rect.Empty;
        }

        protected override FormattedText ComputeFormattedText(
                string label,
                Typeface typeface, double size,
                FlowDirection flowDirection = System.Windows.FlowDirection.LeftToRight
        )
        {
            size = Math.Max(8, size);
            return new FormattedText(label, System.Globalization.CultureInfo.CurrentCulture,
                    flowDirection, typeface, size, Stroke,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);
        }

        protected override void PrepareFieldBackup()
        {
            base.PrepareFieldBackup();
            _fontWeightNum = _fontWeight.ToOpenTypeWeight();
        }

        protected override void FieldDataRestored()
        {
            
            _fontWeight = FontWeight.FromOpenTypeWeight(_fontWeightNum);
            
            base.FieldDataRestored();
        }

        void AdjustBounds()
        {
            if (Bounds.Left != 0) // the bounds where not yet initialized
            {
                LabelArea = ComputeLabelArea(DisplayedLabel());
                var w = Math.Max(DefaultWidth, LabelArea.Width + 20);
                var h = Math.Max(DefaultHeight, LabelArea.Height);
                
                Bounds = ComputeBounds(Bounds.TopLeft, new Size(w, h), LabelArea);
            }
        }

    }
}
