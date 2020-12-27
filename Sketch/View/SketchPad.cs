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
using Sketch.Helper.Binding;
using Sketch.Models;
using Sketch.Interface;
using Sketch.Helper;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Sketch.View
{
    
    public class SketchPad: Canvas, ISketchPadControl, ISketchItemContainer
    {
        public static readonly RoutedEvent SelectedItemChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedItemChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SketchPad));

        public static readonly DependencyProperty ScalingProperty =
            DependencyProperty.Register("Scaling", typeof(double),
                typeof(SketchPad), new PropertyMetadata(OnScalingChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
                typeof(SketchPad), new PropertyMetadata(OnLabelChanged));

        public static readonly DependencyProperty SketchPropery =
            DependencyProperty.Register("Sketch", 
            typeof(Sketch.Models.Sketch), 
            typeof(SketchPad), new PropertyMetadata(OnSketchChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
                typeof(ISketchItemModel), typeof(SketchPad),
                new PropertyMetadata(OnSelectedItemChanged));

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

        public static readonly DependencyProperty ZoomDepthProperty =
            DependencyProperty.Register("ZoomDepth", typeof(int),
                typeof(SketchPad));

        //public const double GridSize = 6;

        //double _xScaling;
        //double _yScaling;
        
        double _initialWidht = double.NaN;
        double _initialHeigh = double.NaN;

        ISketchItemDisplay _rootDisplay;
        readonly Stack<ISketchItemDisplay> _displayStack = new Stack<ISketchItemDisplay>();

       
        public SketchPad():base()
        {
            EditMode = EditMode.Select;
            //this.RenderTransform = new ScaleTransform(0.6, 0.6);
            //SketchItemFactory.ActiveFactory = new SketchItemFactory();
        }

        public bool CanEditLabel
        {
            get => true;
        }

        public ISketchItemNode ParentNode
        {
            get => null;
        }


        // Provide CLR accessors for the event
        public event RoutedEventHandler SelectedItemChanged
        {
            add { AddHandler(SelectedItemChangedEvent, value); }
            remove { RemoveHandler(SelectedItemChangedEvent, value); }
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

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public double Scaling
        {
            get => (double)GetValue(ScalingProperty);
            set => SetValue(LabelProperty, value);
        }

        public ISketchItemModel SelectedItem
        {
            get => GetValue(SelectedItemProperty) as ISketchItemModel;
            set { SetValue(SelectedItemProperty, value); }
        }

        public EditMode EditMode
        {
            get { return (EditMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        public Sketch.Models.Sketch Sketch
        {
            get => (Sketch.Models.Sketch)GetValue(SketchPropery);
            set => SetValue(SketchPropery, value);
        }

        public int ZoomDepth
        {
            get => (int)GetValue(ZoomDepthProperty);
            set => SetValue(ZoomDepthProperty, value);
        }


        public ISketchItemFactory ItemFactory => Sketch.SketchItemFactory;

        public ObservableCollection<ISketchItemModel> SketchItems
        {
            get { return Sketch.SketchItems; }
        }

        public void HandleAddConnector(object sender, EventArgs e)
        {
            if( _displayStack.Any())
            {
                var topmost = _displayStack.Peek();
                topmost.HandleAddConnector(sender, e);
            }
        }

        public void SavePictureAs(string fileName)
        {
            if( _displayStack.Any())
            {
                var topMost = _displayStack.Peek();
                topMost.ShowIntersections();
                SketchItemDisplayHelper.SaveAsPng(topMost.Canvas, fileName);
            }
        }

        public void OpenChildElement()
        {
            if( _displayStack.Any())
            {
                var topMost = _displayStack.Peek();
                if( topMost.SelectedItem?.Model is ContainerModel)
                {
                    EditMode = EditMode.Select;
                    var container = topMost.SelectedItem.Model as ISketchItemContainer;
                    topMost.ClearSelection();
                    topMost.EndEdit();
                    this.Children.Remove(topMost.Canvas);
                    //topMost.Canvas.Visibility = Visibility.Hidden;
                    topMost.SelectedItemChanged -= Display_SelectedItemChanged;
                    var display = new SketchItemDisplay(this,  container );
                    display.SelectedItemChanged += Display_SelectedItemChanged;
                    _displayStack.Push(display);
                    this.Children.Add(display);
                    Display_SelectedItemChanged(this, EventArgs.Empty);
                    EditMode = EditMode.Select;
                    ZoomDepth += 1;
                }
            }
        }

        public void CloseChildElement()
        {
            if (_displayStack.Any())
            {
                var topMost = _displayStack.Peek();
                if( topMost != _rootDisplay)
                {
                    topMost.EndEdit();

                    this.Children.Remove(topMost.Canvas);
                    topMost.SelectedItemChanged -= Display_SelectedItemChanged;
                    //topMost.Canvas.Visibility = Visibility.Hidden;
                    _displayStack.Pop();
                    
                    topMost = _displayStack.Peek();
                    this.Children.Add(topMost.Canvas);
                    ZoomDepth -= 1;
                }
                
                topMost.SelectedItemChanged += Display_SelectedItemChanged;
                topMost.Canvas.Visibility = Visibility.Visible;
                Display_SelectedItemChanged(this, EventArgs.Empty);
                EditMode = EditMode.Select;
                topMost.BeginEdit(new SelectUisOperation(topMost));
            }
            
        }

        //override 


        private static void OnSketchChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if( source is SketchPad pad)
            {
                if( e.NewValue != e.OldValue)
                {
                    pad.Children.Clear();
                    //var oldCollection = e.OldValue as ObservableCollection<ISketchItemModel>;
                    
                    if (e.OldValue is Sketch.Models.Sketch oldSketch)
                    {
                        oldSketch.SketchItemFactory.UnregisterBoundedItemSelectedNotification(pad.HandleAddConnector);
                        if( pad._displayStack != null && pad._displayStack.Any())
                        {
                            foreach( var display in pad._displayStack )
                            {
                                display.Dispose();
                            }
                            pad._displayStack.Clear();
                        }   
                    }
                    
                    if (e.NewValue is Sketch.Models.Sketch newSketch)
                    {
                        SketchItemFactory.ActiveFactory = newSketch.SketchItemFactory;
                        newSketch.RegisterControlImplementation(pad);
                        newSketch.SketchItemFactory.RegisterConnectorItemSelectedNotification(pad.HandleAddConnector);
                        var b = new Binding("Sketch.Label")
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        };
                        pad.SetBinding(SketchPad.LabelProperty, b); 
                        var display = new SketchItemDisplay(pad, pad);
                        pad._rootDisplay = display;
                        pad.Children.Add(display);
                        pad._displayStack.Push(display);
                        display.SelectedItemChanged += pad.Display_SelectedItemChanged;
                    }
                }
            }
        }


        private void Display_SelectedItemChanged(object sender, EventArgs e)
        {
            
            SelectedItem = _displayStack.Peek().SelectedItem?.Model;
            
        }

        private static void OnLogicalWidthChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            //SketchPad pad = source as SketchPad;
            //var newLogicalWidth = (double)e.NewValue;
            //pad._xScaling = newLogicalWidth / pad.Width;
        }

        private static void OnLogicalHeightChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            //SketchPad pad = source as SketchPad;
            //var newLogicalHeight = (double)e.NewValue;
            //pad._yScaling = newLogicalHeight / pad.Height;
        }

        private static void OnLabelChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            SketchPad pad = source as SketchPad;
            // show new lable
            if( pad._rootDisplay != null && e.NewValue != e.OldValue)
            {
                pad._rootDisplay.Label = e.NewValue.ToString();
            }
            
        }

        private static void OnScalingChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if( source is SketchPad pad)
            {
                var scaling = (double)e.NewValue;
                pad.RenderTransform = new ScaleTransform(scaling, scaling);
                if( double.IsNaN(pad._initialHeigh))
                {
                    pad._initialHeigh = pad.Height;
                }
                if( double.IsNaN(pad._initialWidht))
                {
                    pad._initialWidht = pad.Width;
                }
                pad.Width = pad._initialWidht*scaling; pad.Height = pad._initialHeigh * scaling;
               
                //pad.RenderTransformOrigin = new Point(0, 0);
            }
        }

        private static void OnEditModeChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            SketchPad pad = source as SketchPad;
            if (pad._displayStack.Any())
            {
                var topMost = pad._displayStack.Peek();
                if (pad.EditMode == EditMode.Insert)
                {
                    topMost.BeginEdit(new AddConnectableItemOperation(topMost));
                }
                else
                {
                    topMost.BeginEdit(new SelectUisOperation(topMost));
                }
            }
        }

        private static void OnSelectedItemChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if( source is SketchPad pad )
            {
                var changedEvent = new RoutedEventArgs(SelectedItemChangedEvent);
                pad.RaiseEvent(changedEvent);
            }
        }

        public void ExportDiagram(string fileName)
        {

            if (_displayStack.Any())
            {
                var top = _displayStack.Peek();
                SketchItemDisplayHelper.SaveAsPng(top.Canvas, fileName);
            }
        }

        public void SaveFile(string fileName)
        {
            using(var stream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                SketchItemDisplayHelper.TakeSnapshot(stream, this);
            }
        }

        public void OpenFile(string fileName)
        {
            if(!File.Exists(fileName))
            {
                throw new FileNotFoundException("Failed to load the diagram", fileName);
            }
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                SketchItemDisplayHelper.RestoreSnapshot(stream, this);
                EditMode = EditMode.Select;
            }
        }

        public void AlignLeft()
        {
            if (_displayStack.Any())
            {
                SketchItemDisplayHelper.AlignLeft(_displayStack.Peek().SketchItems);
            }
        }

        public void AlignTop()
        {
            if (_displayStack.Any())
            {
                SketchItemDisplayHelper.AlignTop(_displayStack.Peek().SketchItems);
            }
        }

        public void AlignCenter()
        {
            if (_displayStack.Any())
            {
                SketchItemDisplayHelper.AlignCenter(_displayStack.Peek().SketchItems);
            }
        }

        public void SetToSameWidth()
        {
            if (_displayStack.Any())
            {
                SketchItemDisplayHelper.SetToSameWidth(_displayStack.Peek().SketchItems);
            }
        }

        public void SetEqualVerticalSpacing()
        {
            if (_displayStack.Any())
            {
                SketchItemDisplayHelper.SetEqualVerticalSpacing(_displayStack.Peek().SketchItems);
            }
        }

        public void ZoomIn()
        {
            OpenChildElement();
        }

        public void ZoomOut()
        {
            CloseChildElement();
        }
    }
}
