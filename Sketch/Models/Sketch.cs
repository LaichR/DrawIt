using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sketch.View.ColorPicker;
using Sketch.Interface;
using Sketch.Helper.RuntimeCheck;

namespace Sketch.Models
{
    /// <summary>
    /// A Sketch is a collection of sketch-items that can visually be arranged on a canvas. 
    /// The class Sketch provides access to this collection and the basic commands of the graphical 
    /// edior such as aligning, deleting, saving exporting to PNG file etc..
    /// By providing a custom Sketch-Item factory arbitry sketch items may be constructed and added to a sketch.
    /// </summary>
    public class Sketch : ISketchItemContainer, ISketchPadControl, IColorSelectionTarget, INotifyPropertyChanged
    {
        readonly ObservableCollection<ISketchItemModel> _sketchItems = new ObservableCollection<ISketchItemModel>();

        ISketchPadControl _control;
        ISketchItemFactory _sketchItemFactory;
        string _label = "new sketch";
        

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler SketchLoaded;

        public ObservableCollection<ISketchItemModel> SketchItems => _sketchItems;


        public string Label 
        { 
            get => _label; 
            set => SetProperty<string>(ref _label, value); 
        }

        public bool CanEditLabel
        {
            get => true;
        }

        public ISketchItemNode ParentNode
        {
            get => this;
        }

        public ISketchItemFactory SketchItemFactory
        {
            get => _sketchItemFactory;
            set
            {
                SetProperty<ISketchItemFactory>(ref _sketchItemFactory, value);
            }
        }

       
        public void SetEqualVerticalSpacing()
        {
            _control.SetEqualVerticalSpacing();
        }

        public void SetToSameWidth()
        {
            _control.SetToSameWidth();
        }

        public void AlignCenter()
        {
            _control.AlignCenter();
        }

        public void AlignLeft()
        {
            _control.AlignLeft();
        }

        public void AlignTop()
        {
            _control.AlignTop();
        }

        public void DeleteMarked()
        {
            var toDelete = new List<ISketchItemModel>(_sketchItems.Where((x) => x.IsMarked || x.IsSelected));

            foreach (var m in toDelete)
            {
                _sketchItems.Remove(m);
            }
        }

        public void SetColor(System.Windows.Media.Color newFill)
        {
            var uis = _sketchItems.OfType<ConnectableBase>().Where((x) => x.IsMarked);
            foreach (var ui in uis)
            {
                ui.FillColor = newFill;
            }
        }

        public void ExportDiagram(string fileName)
        {
            _control?.ExportDiagram(fileName);
        }

        public void OpenFile(string fileName)
        {
            _control?.OpenFile(fileName);
            SketchLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void SaveFile(string fileName)
        {
            _control?.SaveFile(fileName);
        }

        public void ZoomIn()
        {
            _control?.ZoomIn();
        }

        public void ZoomOut()
        {
            _control?.ZoomOut();
        }

        internal void RegisterControlImplementation(ISketchPadControl control)
        {
            Contract.Requires<ArgumentNullException>(control != null, "control must not be null");
            _control = control;
        }

        void SetProperty<T>(ref T propertyValue, T value, [CallerMemberName]string name="" )
        {
            propertyValue = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
