using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Controls;
using UI.Utilities.Interfaces;
using Sketch.Models;
using Sketch.Types;
using Sketch.Interface;
using Sketch.Utilities;
using System.IO;

namespace Sketch.Controls
{
    
    public partial class SketchPad: Canvas
    {
        public static readonly DependencyProperty SketchItemsPropery =
            DependencyProperty.Register("SketchItems", 
            typeof(ObservableCollection<Sketch.Interface.ISketchItemModel>), 
            typeof(SketchPad), new PropertyMetadata(OnOutlinesChanged));

        public static readonly DependencyProperty DeleteEntriesProperty =
            DependencyProperty.Register("DeleteEntries", typeof(ICommand), 
            typeof(SketchPad),
            new PropertyMetadata(OnDeleteEntriesChanged));

        public static readonly DependencyProperty LogicalWidthProperty =
            DependencyProperty.Register("LogicalWidth", typeof(double), 
            typeof(SketchPad),
            new PropertyMetadata(OnLogicalWidthChanged));

        public static readonly DependencyProperty LogicalHeightProperty =
            DependencyProperty.Register("LogicalHeight", typeof(double), 
            typeof(SketchPad),
            new PropertyMetadata(OnLogicalHeightChanged));

        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register("EditMode", typeof(EditMode), 
            typeof(SketchPad),
            new PropertyMetadata(OnEditModeChanged));

        double _xScaling;
        double _yScaling;

        internal const int GridSize = 6;

        //Button _addConnectorButton = new Button();
        LinkedList<byte[]> _snapshots = new LinkedList<byte[]>();
        
        IGadgetUI _selectedGadget = null;
        List<IGadgetUI> _activeUis = new List<IGadgetUI>();
        ConnectableBase _from = null;
        //ConnectableBase _to = null;
        IEditOperation _currentInputHandler;
        IList<IGadgetUI> _selectedUis = new List<IGadgetUI>();
        ICommand _deleteCmd = null;
        bool _outlineEnable = true;

        object _synchRoot = new object();
        IntersectionFinder _intersectionFinder;

        public SketchPad()
        {
            Focusable = true;
            EditMode = Types.EditMode.Insert;
            Background = Brushes.White;   
            
        }
        
        public double LogicalWidth
        {
            get
            {
                return (double)base.GetValue(LogicalWidthProperty);
            }
            set
            {
                base.SetValue(LogicalWidthProperty, value);
            }
        }

        public int Grid
        {
            get
            {
                return GridSize;
            }
        }

        public double LogicalHeight
        {
            get
            {
                return (double)base.GetValue(LogicalHeightProperty);

            }
            set
            {
                base.SetValue(LogicalHeightProperty, value);
            }
        }

        public EditMode EditMode
        {
            get { return (EditMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }


        public ObservableCollection<ISketchItemModel> SketchItems
        {
            get { return (ObservableCollection<ISketchItemModel>)GetValue(SketchItemsPropery);}
            set { SetValue(SketchItemsPropery, value); }
        }


        public ICommand DeleteEntries
        {
            get { return (ICommand)GetValue(DeleteEntriesProperty); }
            set { SetValue(DeleteEntriesProperty, value); }
        }

        public void TakeSnapshot()
        {
            using (var stream = new MemoryStream())
            { 
                SketchPadHelper.TakeSnapshot(stream, SketchItems);
                var buffer = stream.ToArray();
                _snapshots.AddFirst(buffer);
            }

            if (_snapshots.Count > 100)
            {
                _snapshots.RemoveLast();
            }
        }

        public void DropSnapshot()
        {
            if( _snapshots.Count > 0)
            {
                _snapshots.RemoveFirst();
            }
        }



        public void Undo()
        {
            if(_snapshots.Count > 0)
            {
                var buffer = _snapshots.First();
                _snapshots.RemoveFirst();
                using (var stream = new MemoryStream(buffer))
                {
                    SketchPadHelper.RestoreSnapshot(stream, SketchItems);
                }
            }
        }

        public void HandleAddConnector(object sender, EventArgs e)
        {
            lock (_synchRoot)
            {
                _from = _selectedGadget.Model as ConnectableBase;
                if (_from != null)
                {
                    var p = ConnectorUtilities.ComputeCenter(_from.Bounds);
                    RegisterHandler(new AddConnectorOperation(this, _from, p));
                }
            }
        }

        public void SaveAsPng(string fileName)
        {
            // determin the 
            var minX = (int)ActualWidth;
            var maxX = 0;
            var minY = (int)ActualHeight;
            var maxY = 0;


            RenderTargetBitmap bmp = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Pbgra32);
            foreach( var ui in _activeUis.OfType<ConnectorUI>() )
            {
                bmp.Render(ui.Shape);
                AdjustBoundaries(ui.Model.Geometry.Bounds, ref minX, ref maxX, ref minY, ref maxY);
                
            }
            foreach( var ui in _activeUis.OfType<OutlineUI>())
            {
                bmp.Render(ui.Shape);
                AdjustBoundaries(ui.Model.Geometry.Bounds, ref minX, ref maxX, ref minY, ref maxY);
            }

            // x and y must not be smaller than 0
            var x = Math.Max(0,(minX - 50));
            var y = Math.Max(0,(minY - 50));
            var width = (int)Math.Min(ActualWidth, (maxX-x + 100));
            var height = (int)Math.Min(ActualHeight, (maxY-y + 100));


            CroppedBitmap cropped = new CroppedBitmap(bmp, new Int32Rect(x, y, width, height));
                
            //bmp.Render( this );
            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(cropped));
            
            using (System.IO.Stream stm = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                encoder.Save(stm);
            }

        }

        public void MoveSelected( double dx, double dy )
        {
            var transform = new TranslateTransform(dx, dy);
            foreach( var ui in _selectedUis)
            {
                ui.Model.Move(transform);
            }
        }

        public void AddVisualChild(ISketchItemModel model)
        {
            IGadgetUI newActiveOutline = null;

            if (model is ConnectableBase)
            {
                var connectable = model as ConnectableBase;
                newActiveOutline = new OutlineUI(this, connectable);
                // the outlines are always above the connectors
                Canvas.SetZIndex(newActiveOutline.Shape, 1500);
            }
            else
            {
                ((ConnectorModel)model).SetSiblings(SketchItems.OfType<ConnectorModel>().Where((x) => x != model));
                var connector = new ConnectorUI(this, model as ConnectorModel);
                newActiveOutline = connector;
                Canvas.SetZIndex(newActiveOutline.Shape, 0);
            }
            Children.Add(newActiveOutline.Shape);
            if ( !_outlineEnable)
            {
                newActiveOutline.Disable();
            }
            

            if( _selectedGadget != null)
            {
                _selectedGadget.IsSelected = false;
            }
            newActiveOutline.IsSelected = false;
            newActiveOutline.SelectionChanged += gadgetSelectionChanged;
            newActiveOutline.IsMarkedChanged += gadgetIsMarkedChanged;
            
            newActiveOutline.IsSelected = true;
            this._activeUis.Add(newActiveOutline);
        }

        public void RemoveVisualChild( ISketchItemModel model )
        {
            List<IGadgetUI> toRemove = new List<IGadgetUI>();
            toRemove.AddRange(_activeUis.Where((x) => x.Model == model));
            RemoveActiveUis(toRemove);
            
        }

        internal IEditOperation CurrentOperationHandler
        {
            get
            {
                return this._currentInputHandler;
            }
        }


        internal IntersectionFinder Intersections
        {
            get
            {
                //AdornerLayer.GetAdornerLayer(this).Add(_intersectionFinder);
                return _intersectionFinder;
                
            }
        }


        internal void HandleKeyDown( KeyEventArgs args)
        {
            this.OnKeyDown(args);
        }

        internal void RegisterHandler( IEditOperation handler )
        {
            if( _currentInputHandler != null)
            {
                _currentInputHandler.StopOperation(false);
            }
            _currentInputHandler = handler;
        }




        internal void SetOutlineEnable(bool enable)
        {
            foreach (var child in Children.OfType<IGadgetUI>())
            {
                _outlineEnable = enable;
                if (enable)
                {
                    child.Enable();
                }
                else
                {
                    child.Disable();
                }
            }
        }

        void EndOperation()
        {
            var factory = ModelFactoryRegistry.Instance.GetSketchItemFactory();
            if ( factory.SelectedForCreation == null||
                 factory.SelectedForCreation.GetInterface("IBoundedModelItem") == null)
            {
                EditMode = EditMode.Select;
            }

            if (EditMode == Types.EditMode.Insert)
            {
                RegisterHandler(new AddGadgetOperation(this));
            }
            else
            {
                RegisterHandler(new SelectUisOperation(this));
            }
            InvalidateVisual();
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if( _intersectionFinder == null)
            {
                _intersectionFinder = new IntersectionFinder(this);
                AdornerLayer.GetAdornerLayer(this).Add(_intersectionFinder);
            }
        }

        void OnOutlinesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //var hasFocus = Focus();
                foreach (var i in e.NewItems)
                {
                    AddVisualChild(i as ISketchItemModel);
                }
            }
            else if( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                List<IGadgetUI> toRemove = new List<IGadgetUI>();
                foreach (var m in e.OldItems)
                {
                    var model = m as ISketchItemModel;
                    toRemove.AddRange( _activeUis.Where((x) => x.Model == model || model.RefModel == x.Model) );
                }
                RemoveActiveUis(toRemove);
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                List<IGadgetUI> toRemove = new List<IGadgetUI>(_activeUis);
                RemoveActiveUis(toRemove);
                Children.Clear();
            }
        }

        void gadgetIsMarkedChanged( object obj, bool args)
        {
            lock( this._synchRoot)
            {
                var ui = obj as IGadgetUI;
                if (ui != null)
                {
                    if (_selectedUis.Contains(ui) && !args)
                    {
                        _selectedUis.Remove(ui);
                    }
                    else if (args && !_selectedUis.Contains(ui))
                    {
                        _selectedUis.Add(ui);
                    }
                }
            }
        }

        void gadgetSelectionChanged(object obj, bool args)
        {
            lock (this._synchRoot)
            {
                if (args )
                {
                    if( _selectedGadget != obj && _selectedGadget != null)
                    {
                        _selectedGadget.IsSelected = false;
                    }
                    _selectedGadget = obj as IGadgetUI;
                }
                else 
                {
                    if (_selectedGadget == obj)
                    {
                        _selectedGadget = null;
                    }
                }
            }

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch( e.Key )
            {
                case Key.Delete:
                    if (_deleteCmd != null)
                    {
                        TakeSnapshot();
                        _deleteCmd.Execute(null);
                    }
                    break;
                case Key.F2:
                    if (_selectedGadget != null)
                    {
                        {
                            var ui = _selectedGadget.Shape as OutlineUI;
                            if (ui != null)
                            {
                                ui.StartEdit();
                            }
                        }
                    }
                    break;
                case Key.Escape:
                    // we iterate over the _activeUi collection, since we are removing items from the _selectedUi collection 
                    foreach( var ui in _activeUis)
                    {
                        ui.IsMarked = false;
                    }
                    //_selectedUis.Clear();
                    if( _selectedGadget != null)
                    {
                        _selectedGadget.IsSelected = false;
                        //_selectedGadget = null;
                    }
                    break;
                case Key.Z:
                    if( Keyboard.IsKeyDown(Key.LeftCtrl)||
                        Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        Undo();
                    }
                    break;
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        void RemoveActiveUis(List<IGadgetUI> toRemove)
        {
            foreach (var ui in toRemove)
            {
                ui.IsSelected = false;
                ui.IsMarked = false;

                ui.SelectionChanged -= gadgetSelectionChanged;
                ui.IsMarkedChanged -= gadgetIsMarkedChanged;

                
                _activeUis.Remove(ui);
                Children.Remove(ui.Shape);

            }
        }

        //override 

        private static void AdjustBoundaries( Rect boundaries, ref int left, ref int right, ref int top, ref int bottom)
        {
            if (boundaries.Left < left)
            {
                left = (int)boundaries.Left;
            }
            if (boundaries.Right > right)
            {
                right = (int)boundaries.Right;
            }
            if (boundaries.Top < top)
            {
                top = (int)boundaries.Top;
            }
            if (boundaries.Bottom > bottom)
            {
                bottom = (int)boundaries.Bottom;
            }
        }

        private static void OnOutlinesChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            SketchPad pad = source as SketchPad;
            if( pad != null)
            {
                if( e.NewValue != e.OldValue)
                {
                    pad.Children.Clear();
                    var oldCollection = e.OldValue as ObservableCollection<ISketchItemModel>;
                    if (oldCollection != null)
                    {
                        oldCollection.CollectionChanged -= pad.OnOutlinesCollectionChanged;
                    }
                    var newCollection = e.NewValue as ObservableCollection<ISketchItemModel>;
                    if (newCollection != null)
                    {
                        newCollection.CollectionChanged += pad.OnOutlinesCollectionChanged;

                        if (newCollection.Count() > 0)
                        {
                            for (int i = 0; i < newCollection.Count(); ++i)
                            {
                                pad.AddVisualChild(newCollection[i]);
                            }
                        }
                    }
                }
            }
        }

        private static void OnLogicalWidthChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            SketchPad pad = source as SketchPad;
            var newLogicalWidth = (double)e.NewValue;
            pad._xScaling = newLogicalWidth / pad.Width;
        }

        private static void OnLogicalHeightChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            SketchPad pad = source as SketchPad;
            var newLogicalHeight = (double)e.NewValue;
            pad._yScaling = newLogicalHeight / pad.Height;
        }

        private static void OnEditModeChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            SketchPad pad = source as SketchPad;
            if( pad.EditMode == EditMode.Insert)
            {
                pad.RegisterHandler(new AddGadgetOperation(pad));
            }
            else
            {

                pad.RegisterHandler(new SelectUisOperation(pad));
            }
        }


        private static void OnDeleteEntriesChanged(DependencyObject source,
           DependencyPropertyChangedEventArgs e)
        {
            SketchPad pad = source as SketchPad;
            if( pad != null)
            {
                pad._deleteCmd = e.NewValue as ICommand;
            }

        }
    }
}
