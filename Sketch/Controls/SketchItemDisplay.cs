using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Shapes;
using UI.Utilities.Interfaces;
using Sketch.Models;
using Sketch.Types;
using Sketch.Interface;
using Sketch.Utilities;
using System.IO;

namespace Sketch.Controls
{
    
    public class SketchItemDisplay: Canvas, ISketchItemDisplay
    {
        
        
        
        LinkedList<byte[]> _snapshots = new LinkedList<byte[]>();
        
        ISketchItemUI _selectedUI = null;                    // currently selected item or null if no item is selected
        
        ConnectableBase _from = null;                        // item that is the starting object for a new connector to be inserted
        
        IEditOperation _currentInputHandler;                 // handles the user input

        IList<ISketchItemUI> _markedUis = new List<ISketchItemUI>(); // list of items that are highlighted; this includes the selected item as well
        
        bool _outlineEnable = true;                          // flag to indicate if new items shall react on user gestures

        object _synchRoot = new object();                    // i think this is not really consistently used!
        IntersectionFinder _intersectionFinder;
        readonly SketchPad _parent;
        readonly ISketchItemContainer _container;
        readonly SketchItemDisplayLabel _myLabel;

        public SketchItemDisplay(SketchPad parent, ISketchItemContainer container)
        {
            this.Height = parent.Height;
            this.Width = parent.Width;
            _parent = parent;
            _container = container;
            Focusable = true;
            Background = Brushes.White;
            foreach( var item in _container.SketchItems)
            {
                AddVisualChild(item);
            }
            _container.SketchItems.CollectionChanged += SketchItems_CollectionChanged;
            Visibility = Visibility.Visible;
            _myLabel = new SketchItemDisplayLabel(container, this);
            BeginEdit(new AddConnectableItemOperation(this));
        }


        public event EventHandler SelectedItemChanged;


        public ISketchItemUI SelectedItem
        {
            get => _selectedUI;
        }

        public void ClearSelection()
        {
            if ( _selectedUI != null )
            {
                _selectedUI.IsSelected = false;
            }
        }

        public string Label
        {
            get => _container.Label;
            set
            {
                _myLabel.Tag = value;
                _myLabel.UpdateGeometry();
                _myLabel.InvalidateVisual();
            }
        }

        public IList<ISketchItemUI> MarkedItems
        {
            get => _markedUis;
        }

        public IEnumerable<ISketchItemUI> Items
        {
            get => Children.OfType<ISketchItemUI>();
        }
        
        public  double LogicalWidth
        {
            get => _parent.LogicalWidth;
            
        }

        public  double LogicalHeight
        {
            get => _parent.LogicalHeight;
            
        }

        public  EditMode EditMode
        {
            get => _parent.EditMode;
            set => _parent.EditMode = value; 
        }


        public  ObservableCollection<ISketchItemModel> SketchItems
        {
            get => _container.SketchItems;
            
        }


        public void ShowIntersections()
        {
            if (_intersectionFinder != null)
            {
                _intersectionFinder.InvalidateVisual();
            }
        }

        public void TakeSnapshot()
        {
            using (var stream = new MemoryStream())
            { 
                SketchItemDisplayHelper.TakeSnapshot(stream, SketchItems);
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
                    SketchItemDisplayHelper.RestoreSnapshot(stream, SketchItems);
                }
            }
        }

        public void HandleAddConnector(object sender, EventArgs e)
        {
            lock (_synchRoot)
            {
                _from = _selectedUI.Model as ConnectableBase;
                if (_from != null)
                {
                    var p = ConnectorUtilities.ComputeCenter(_from.Bounds);
                    BeginEdit(new AddConnectorOperation(this, _from, p));
                }
            }
        }

        public void DeleteSelectedItems()
        {
            var toRemove = new List<ISketchItemModel>(SketchItems.Where((x) => x.IsMarked || x.IsSelected));
            foreach (var item in toRemove)
            {
                SketchItems.Remove(item);
            }
        }

        public void AddVisualChild(ISketchItemModel model)
        {
            ISketchItemUI newActiveOutline = null;

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
            
            if( _selectedUI != null)
            {
                _selectedUI.IsSelected = false;
            }
            newActiveOutline.IsSelected = false;
            newActiveOutline.SelectionChanged += SketchItemSelectionChanged;
            newActiveOutline.IsMarkedChanged += SketchItemIsMarkedChanged;
            
            newActiveOutline.IsSelected = true;
            //this._activeUis.Add(newActiveOutline);
        }

        public void RemoveVisualChild( ISketchItemModel model )
        {
            List<ISketchItemUI> toRemove = new List<ISketchItemUI>();
            toRemove.AddRange(Children.OfType<ISketchItemUI>().Where((x) => x.Model == model));
            RemoveActiveUis(toRemove);
            
        }

        public IEditOperation CurrentOperationHandler
        {
            get
            {
                return this._currentInputHandler;
            }
        }

        public Canvas Canvas => this;

        public void HandleKeyDown( KeyEventArgs args)
        {
            this.OnKeyDown(args);
        }

        public void BeginEdit( IEditOperation handler )
        {
            if( _currentInputHandler != null)
            {
                _currentInputHandler.StopOperation(false);
            }
            _currentInputHandler = handler;
        }


        public void SetSketchItemEnable(bool enable)
        {
            _outlineEnable = enable;
            foreach (var child in Children.OfType<ISketchItemUI>())
            {
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

        public void EndEdit()
        {
            var factory = ModelFactoryRegistry.Instance.GetSketchItemFactory();
            if ( factory.SelectedForCreation == null||
                 factory.SelectedForCreation.GetInterface("IBoundedModelItem") == null)
            {
                EditMode = EditMode.Select;
            }

            if (EditMode == Types.EditMode.Insert)
            {
                BeginEdit(new AddConnectableItemOperation(this));
            }
            else
            {
                BeginEdit(new SelectUisOperation(this));
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

        void SketchItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                List<ISketchItemUI> toRemove = new List<ISketchItemUI>();
                foreach (var m in e.OldItems)
                {
                    var model = m as ISketchItemModel;
                    toRemove.AddRange( Children.OfType<ISketchItemUI>().Where((x) => x.Model == model || model.RefModel == x.Model) );
                }
                RemoveActiveUis(toRemove);
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                List<ISketchItemUI> toRemove = new List<ISketchItemUI>(Children.OfType<ISketchItemUI>());
                RemoveActiveUis(toRemove);
                
            }
        }

        void SketchItemIsMarkedChanged( object obj, bool args)
        {
            lock( this._synchRoot)
            {
                var ui = obj as ISketchItemUI;
                if (ui != null)
                {
                    if (_markedUis.Contains(ui) && !args)
                    {
                        _markedUis.Remove(ui);
                    }
                    else if (args && !_markedUis.Contains(ui))
                    {
                        _markedUis.Add(ui);
                    }
                }
            }
        }

        void SketchItemSelectionChanged(object obj, bool args)
        {
            lock (this._synchRoot)
            {
                if (args )
                {
                    if( _selectedUI != obj && _selectedUI != null)
                    {
                        _selectedUI.IsSelected = false;
                    }
                    _selectedUI = obj as ISketchItemUI;
                }
                else 
                {
                    if (_selectedUI == obj)
                    {
                        _selectedUI = null;
                    }
                }
                
                SelectedItemChanged?.Invoke(this, EventArgs.Empty);
                
            }

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch( e.Key )
            {
                case Key.Delete:
                    TakeSnapshot();
                    DeleteSelectedItems();
                    break;
                case Key.F2:
                    if (_selectedUI != null)
                    {
                        {
                            var ui = _selectedUI.Shape as OutlineUI;
                            if (ui != null)
                            {
                                ui.StartEdit();
                            }
                        }
                    }
                    break;
                case Key.Escape:
                    // we iterate over the _activeUi collection, since we are removing items from the _selectedUi collection 
                    foreach( var ui in Children.OfType<ISketchItemUI>())
                    {
                        ui.IsMarked = false;
                    }
                    
                    if( _selectedUI != null)
                    {
                        _selectedUI.IsSelected = false;
                        
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

        void RemoveActiveUis(List<ISketchItemUI> toRemove)
        {
            foreach (var ui in toRemove)
            {
                ui.IsSelected = false;
                ui.IsMarked = false;
                ui.SelectionChanged -= SketchItemSelectionChanged;
                ui.IsMarkedChanged -= SketchItemIsMarkedChanged;
                Children.Remove(ui.Shape);
            }
        }

        public void Dispose()
        {
            List<ISketchItemUI> list = new List<ISketchItemUI>(Children.OfType<ISketchItemUI>());
            RemoveActiveUis(list);
            Children.Clear();
            SketchItems.CollectionChanged -= SketchItems_CollectionChanged;
        }
    }
}
