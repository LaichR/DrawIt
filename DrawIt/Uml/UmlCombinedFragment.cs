using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Sketch;
using Sketch.Models;

namespace DrawIt.Uml
{
    public enum CombindedFragmentOperation
    {
        
        Loop,
        Alt,
        Opt
    };

    [Serializable]
    public class UmlCombinedFragment : ConnectableBase
    {
        static readonly CombindedFragmentOperation _default = CombindedFragmentOperation.Loop;

        CombindedFragmentOperation _operation = _default;
        FormattedText _formattedLabel;
        //Point _textPosition = new Point();
        //bool _boundsChanging;

        public UmlCombinedFragment(Point location) : base(location, new Size(150,50),  $"{_default}", DefaultColor)
        {
            CanChangeSize = true;
            CanEditLabel = false;
            _formattedLabel = ComputeFormattedText(Label, ConnectableBase.DefaultFont, ConnectableBase.DefaultFontSize);
        }

        protected UmlCombinedFragment(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        [Browsable(true)]
        public CombindedFragmentOperation Operation
        {
            get => _operation;
            set
            {
                SetProperty<CombindedFragmentOperation>(ref _operation, value);
                Label = Operation.ToString();
                _formattedLabel = ComputeFormattedText(Label, ConnectableBase.DefaultFont, ConnectableBase.DefaultFontSize);
                UpdateGeometry();
            }
        }

        public override bool CanChangeZorder => true;

        public override bool CanCopy => false;

        [Browsable(false)]
        public override string Label { get => base.Label; set => base.Label = value; }

        protected override Size ComputeFormattedTextSize(string label, Typeface typeface, double size, double marginX, double marginY)
        {
            var newSize = base.ComputeFormattedTextSize(label, typeface, size, marginX, marginY);
            newSize.Width += newSize.Height / 2;
            return newSize;
        }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
            var geometry = Geometry as GeometryGroup;
            geometry.Children.Clear();
            double lableWidth = LabelArea.Width;
            double lableHeight = LabelArea.Height;
            var points = new[]
            {
                new Point(0,0),
                new Point(lableWidth,0),
                new Point(lableWidth,lableHeight/2),
                new Point(lableWidth-lableHeight/2, lableHeight),
                new Point(0,lableHeight)
            };
            var pf1 = ConnectorUtilities.GetPathFigureFromPoints(points);
            pf1.IsClosed = true;
            pf1.IsFilled = true;

            points = new[]
            {
                new Point(0,0), new Point( Bounds.Width-1, 0),
                new Point(Bounds.Width-1, Bounds.Height-1),
                new Point(0, Bounds.Height-1)
            };
            
            var pf2 = ConnectorUtilities.GetPathFigureFromPoints(points);
            pf2.IsClosed = true;
            pf2.IsFilled = false;
            
            geometry.Children.Add(new PathGeometry(new[] { pf1 }));

            geometry.Children.Add(
                _formattedLabel.BuildGeometry(new Point(LabelArea.Width/2 - _formattedLabel.Width / 2, LabelArea.Top)));
            
            geometry.Transform = Rotation;
        }
    }
}
