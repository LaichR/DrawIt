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
using System.Runtime.CompilerServices;

namespace Sketch.Controls
{
    
    public class SketchPad: Canvas, ISketchItemContainer
    {
        public static readonly RoutedEvent SelectedItemChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedItemChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SketchPad));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
                typeof(SketchPad), new PropertyMetadata(OnLabelChanged));

        public static readonly DependencyProperty SketchItemsPropery =
            DependencyProperty.Register("SketchItems", 
            typeof(ObservableCollection<Sketch.Interface.ISketchItemModel>), 
            typeof(SketchPad), new PropertyMetadata(OnSketchItemsChanged));


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

        public const double GridSize = 6;

        double _xScaling;
        double _yScaling;
        ISketchItemDisplay _rootDisplay;
        Stack<ISketchItemDisplay> _displayStack = new Stack<ISketchItemDisplay>();

       
        public SketchPad():base()
        {
            EditMode = Types.EditMode.Insert;       
            
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
                    EditMode = EditMode.Insert;
                    var container = topMost.SelectedItem.Model as ISketchItemContainer;
                    topMost.ClearSelection();
                    this.Children.Remove(topMost.Canvas);
                    //topMost.Canvas.Visibility = Visibility.Hidden;
                    topMost.SelectedItemChanged -= Display_SelectedItemChanged;
                    var display = new SketchItemDisplay(this,  container );
                    display.SelectedItemChanged += Display_SelectedItemChanged;
                    _displayStack.Push(display);
                    this.Children.Add(display);
                    Display_SelectedItemChanged(this, EventArgs.Empty);
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
                    this.Children.Remove(topMost.Canvas);
                    topMost.SelectedItemChanged -= Display_SelectedItemChanged;
                    //topMost.Canvas.Visibility = Visibility.Hidden;
                    _displayStack.Pop();
                    topMost = _displayStack.Peek();
                    topMost.SelectedItemChanged += Display_SelectedItemChanged;
                    this.Children.Add(topMost.Canvas);
                    topMost.Canvas.Visibility = Visibility.Visible;
                    
                    Display_SelectedItemChanged(this, EventArgs.Empty);
                }
            }
        }

        //override 


        private static void OnSketchItemsChanged(DependencyObject source,
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
                        if( pad._displayStack != null && pad._displayStack.Any())
                        {
                            foreach( var display in pad._displayStack )
                            {
                                display.Dispose();
                            }
                            pad._displayStack.Clear();
                        }
                    }
                    var newCollection = e.NewValue as ObservableCollection<ISketchItemModel>;
                    if (newCollection != null)
                    {
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
            var changedEvent = new RoutedEventArgs(SelectedItemChangedEvent);
            RaiseEvent(changedEvent);
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


    }
}
