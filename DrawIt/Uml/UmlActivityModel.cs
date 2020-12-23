using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UI.Utilities.Interfaces;
using Sketch.Helper;
using Sketch.Models;
using System.Runtime.Serialization;
using Sketch.Interface;
using System.Windows.Media.Imaging;
using UI.Utilities;
using UI.Utilities.Behaviors;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Specialized;

namespace DrawIt.Uml
{
    [Serializable]
    [AllowableConnector(typeof(UmlTransitionModel))]
    public class UmlActivityModel : ContainerModel
    {
        new static readonly double DefaultWidth = 150;
        new static readonly double DefaultHeight = 60;

        static readonly double DefaultRoundingEdgeRadius = DefaultHeight / 4;
        //static readonly BitmapSource CallActionIndicator = ToBitmapSource.Bitmap2BitmapSource(Properties.Resources.CallActionIndicator);

        [PersistentField((int)ModelVersion.V_0_1, "Precondition")]
        string _precondition = "";

        [PersistentField((int)ModelVersion.V_0_1, "Postcondition")]
        string _postcondition = "";

        public UmlActivityModel(Point p, ISketchItemContainer container)
            : base(p, container, new Size( DefaultWidth, DefaultHeight)) 
        {
            IsSelected = true;
            CanChangeSize = true;
            Label = "new action";
            RotationAngle = 0.0;
            Commands = GetUmlActivityModelCommands();
        }



        protected override Rect ComputeLabelArea(string label)
        {
            var labelAreaLocation = new Point(5, DefaultRoundingEdgeRadius);
            var size = new Size(DefaultWidth-20, DefaultHeight / 2);
            
            if (!string.IsNullOrEmpty(label))
            {
                size = ComputeFormattedTextSize(label, ConnectableBase.DefaultFont, ConnectableBase.DefaultFontSize,
                ConnectableBase.MinimalTextMarginX, ConnectableBase.MinimalTextMarginY);

            }
            if (!string.IsNullOrEmpty(Precondition))
            {
                var prSize = ComputeFormattedTextSize(GetPreconditionDisplayString(), ConnectableBase.DefaultFont, ConnectableBase.DefaultFontSize,
                ConnectableBase.MinimalTextMarginX, ConnectableBase.MinimalTextMarginY);
                size.Width = Math.Max(prSize.Width, size.Width);
            }
            if(!string.IsNullOrEmpty(Postcondition))
            {
                var poSize = ComputeFormattedTextSize(GetPostconditionDisplayString(), ConnectableBase.DefaultFont, ConnectableBase.DefaultFontSize,
                ConnectableBase.MinimalTextMarginX, ConnectableBase.MinimalTextMarginY);
                size.Width = Math.Max(poSize.Width, size.Width);
            }
            return new Rect(labelAreaLocation, size); // the entier shape may contain text!
        }

        protected UmlActivityModel(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {

            var myGeometry = Geometry as GeometryGroup;
            myGeometry.Children.Clear();
            
            myGeometry.Children.Add(new RectangleGeometry(
                new Rect(0,0, Bounds.Width, Bounds.Height), DefaultRoundingEdgeRadius, DefaultRoundingEdgeRadius));
            myGeometry.Transform = Rotation;
        }

        [Browsable(true)]
        public string Precondition 
        {
            get => _precondition;
            set
            {
                SetProperty<string>(ref _precondition, value);
                AdjustBounds();
                RaisePropertyChanged("PreconditionVisibility");
                
            }
        }

        [Browsable(true)]
        public string Postcondition
        {
            get => _postcondition;
            set
            {
                SetProperty<string>(ref _postcondition, value);
                AdjustBounds();
                RaisePropertyChanged("PostconditionVisibility");

            }
        }

        public string PreLabel => "<<pre>>";
        public string PostLabel => "<<post>>";

        public Visibility PreconditionVisibility
        {
            get => string.IsNullOrEmpty(Precondition) ? Visibility.Collapsed : Visibility.Visible;
        }

        public Visibility PostconditionVisibility
        {
            get => string.IsNullOrEmpty(Postcondition) ? Visibility.Collapsed : Visibility.Visible;
        }

        public override string Label
        {
            get => base.Label;
            set
            {
                base.Label = value;
                AdjustBounds();
            }
        }

        public double ImageAreaWidth
        {
            get 
            {
                if (!this.SketchItems.Any()) return 0;
                return 25;
            }
        }

        public Visibility ImageAreaVisibility
        {
            get => this.SketchItems.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        public override System.Windows.Media.RectangleGeometry Outline
        {
            get
            {
                var outline = new System.Windows.Media.RectangleGeometry(Bounds, DefaultRoundingEdgeRadius, DefaultRoundingEdgeRadius)
                {
                    Transform = Rotation
                };
                
                return outline;
            }
        }

        string GetPreconditionDisplayString()
        {
            return string.Format("{0} {1}", PreLabel, Precondition);
        }

        string GetPostconditionDisplayString()
        {
            return string.Format("{0} {1}", PostLabel, Postcondition);
        }

        void AdjustBounds()
        {
            if (Bounds.Left != 0) // the bounds where not yet initialized
            {
                LabelArea = ComputeLabelArea(DisplayedLabel());
                var w = Math.Max(DefaultWidth, LabelArea.Width + 20);
                var h = Math.Max(DefaultHeight, LabelArea.Height + DefaultRoundingEdgeRadius * 2);
                if( PostconditionVisibility == Visibility.Visible )
                {
                    h += 15;
                }
                if( PreconditionVisibility == Visibility.Visible )
                {
                    h += 15;
                }
                Bounds = ComputeBounds(Bounds.TopLeft, new Size(w, h), LabelArea);
            }
        }

        protected override void SketchItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.SketchItemsChanged(sender, e);
            RaisePropertyChanged("ImageAreaWidth");
            RaisePropertyChanged("ImageAreaVisibility");
        }

        void AddInputPort()
        {
            var leftSideConnectors = Decorators.Where((x) => x.Side == ConnectorDocking.Left);
            double relPos = 1.0 / (leftSideConnectors.Count() + 2);
            double delta = relPos;
            var R = new Rect(0, 0, Bounds.Width, Bounds.Height);
            foreach (var d in leftSideConnectors)
            {
                d.Location = ConnectorUtilities.ComputePoint(R, ConnectorDocking.Left, relPos);
                d.RelativePosition = relPos;
                relPos += delta;
            }
            ulong id = 1;
            if (Decorators.Any())
            {
                id = Decorators.Select<DecoratorModel, ulong>((x) => x.Id).Max() + 1;
            }
            var p = new Sketch.Models.BasicItems.SqaredConnectorPort(ConnectorDocking.Left, id)
            {
                Location = ConnectorUtilities.ComputePoint(R, ConnectorDocking.Left, relPos),
                RelativePosition = relPos
            };
            
            Decorators.Add(p);
        }

        List<ICommandDescriptor> GetUmlActivityModelCommands()
        {
            List<ICommandDescriptor> commands = new List<ICommandDescriptor>
            {
                new CommandDescriptor()
                {
                    Name = "Add Input Port",
                    Command = new DelegateCommand(()=>AddInputPort())
                }
            };
            return commands;
        }

    }
}
