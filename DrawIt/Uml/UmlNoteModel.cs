using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.ComponentModel;
using Sketch.Models;
using Sketch.Interface;

namespace DrawIt.Uml
{
    [Serializable]
    public class UmlNoteModel:ConnectableBase
    {
        //IList<Bluebottle.Base.Interfaces.ICommandDescriptor> _tools;

        new public const double DefaultHeight = 50;
        new public const double DefaultWidth = 150;

       
        static readonly double DogEarSize = 30;
       
        [PersistentField((int)ModelVersion.V_0_1,"Note")]
        string _note;
        [PersistentField((int)ModelVersion.V_0_1,"IsDecisionInput")]
        bool _isDecistionInput;

        
        
        //readonly PathFigure[] _notePath = new PathFigure[1];
        

        public UmlNoteModel(Point p, ISketchItemContainer container )
            :base(p, container, new Size(DefaultWidth, DefaultHeight), 
                 "a note",
                 Colors.White)
        {
            CanChangeSize = true;
            CanEditLabel = true;
            Note = "a note";

        }

        protected UmlNoteModel(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            
            UpdateGeometry();
        }

        public override string LabelPropertyName
        {
            get
            {
                return "Note";
            }
        }

        [Browsable(true)]
        public bool IsDecisionInput
        {
            get => _isDecistionInput;
            set
            {
                _isDecistionInput = value;
                RaisePropertyChanged("StereotypeVisibility");
            }
        }

        [System.ComponentModel.Browsable(false)]
        public override string Label { get => base.Label; set => base.Label = value; }

        [System.ComponentModel.Browsable(true)]
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                AdjustBounds();
                
                RaisePropertyChanged("Note");
            }
        }

        


        public Visibility StereotypeVisibility
        {
            get => IsDecisionInput ? Visibility.Visible : Visibility.Hidden;
        }
        public override void UpdateGeometry()
        {

            var myGeometry = Geometry as GeometryGroup;
            myGeometry.Children.Clear();


            var pathFigure = GeometryHelper.GetPathFigureFromPoint(new Point[]
            {
                        new Point(0,0),
                        new Point( Bounds.Width - DogEarSize, 0),
                        //new Point( Bounds.Width - DogEarSize, DogEarSize),
                        new Point( Bounds.Width, DogEarSize),
                        new Point( Bounds.Width, Bounds.Height),
                        new Point( 0, Bounds.Height)
            });

            pathFigure.IsClosed = true;
            pathFigure.IsFilled = true;

            myGeometry.Children.Add(GeometryHelper.GetGeometryFromPath(pathFigure));
            myGeometry.Children.Add(new LineGeometry(new Point(Bounds.Width - DogEarSize, 0),
                new Point(Bounds.Width-DogEarSize, DogEarSize)));
            myGeometry.Children.Add(new LineGeometry(new Point(Bounds.Width - DogEarSize, DogEarSize),
                new Point(Bounds.Width, DogEarSize)));

            myGeometry.Transform = Rotation;
        }

        protected override Rect ComputeLabelArea(string label)
        {   
            var labelAreaLocation = new Point(5, DogEarSize );
            var size = new Size(DefaultWidth - 20, DefaultHeight - DogEarSize -
                ConnectableBase.MinimalTextMarginY*2);
            if (!string.IsNullOrEmpty(label))
            {
                size = ComputeFormattedTextSize(label, ConnectableBase.DefaultFont, ConnectableBase.DefaultFontSize,
                ConnectableBase.MinimalTextMarginX, ConnectableBase.MinimalTextMarginY);
            }
            return new Rect(labelAreaLocation, size); // the entier shape may contain text!
        }

        protected override string DisplayedLabel()
        {
            return Note;
        }

        void AdjustBounds()
        {
            if (Bounds.Left != 0) // the bounds where not yet initialized
            {
                LabelArea = ComputeLabelArea(DisplayedLabel());
                var w = Math.Max(DefaultWidth, LabelArea.Width + 20);
                var h = Math.Max(Bounds.Height, LabelArea.Height + DogEarSize + 20);
                Bounds = ComputeBounds(Bounds.TopLeft, new Size(w, h), LabelArea);
            }
        }
    }
}
