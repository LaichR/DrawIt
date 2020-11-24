using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Prism.Mvvm;
using Prism.Commands;
using Sketch.Interface;
using Sketch.Utilities;
using Sketch.Controls;
using Sketch.Types;
using System.Windows;
using System.Windows.Input;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Sketch.Controls.ColorPicker;
using DrawIt.Uml;
using DrawIt.Shapes;

namespace DrawIt
{
    public class ApplicationViewModel : BindableBase
    {
        //DelegateCommand _cmdInsertMode;
        //DelegateCommand _cmdEditMode;

        readonly DelegateCommand _cmdSave;
        readonly DelegateCommand _cmdOpen;
        readonly DelegateCommand _cmdSavePng;

        readonly DelegateCommand _deleteEntry;

        readonly DelegateCommand _cmdAlignTop;
        readonly DelegateCommand _cmdAlignLeft;
        readonly DelegateCommand _cmdAlignCenter;

        readonly DelegateCommand _cmdZoomIn;
        readonly DelegateCommand _cmdZoomOut;

        Dictionary<Type, System.Windows.Input.Cursor> _registeredCursors = new Dictionary<Type, Cursor>();

        bool _isEditMode;
        bool _isInsertMode;
        //string _label = "No Name";
        EditMode _editMode;
        object _selectedItem;
        readonly Sketch.Models.Sketch _sketch;
        System.Windows.Input.Cursor _editCursor;

        static readonly System.Drawing.Bitmap _insertPackage = Properties.Resources.UmlPackageShape;
        static readonly System.Drawing.Bitmap _insertState = Properties.Resources.UmlStateShape;
        static readonly System.Drawing.Bitmap _insertClass = Properties.Resources.UmlClassShape;
        static readonly System.Drawing.Bitmap _insertNote = Properties.Resources.UmlNoteShape;
        static readonly System.Drawing.Bitmap _insertChoice = Properties.Resources.UmlChoiceShape;
        static readonly System.Drawing.Bitmap _insertPicture = Properties.Resources.image;


        //readonly ObservableCollection<ISketchItemModel> _sketchItems = new ObservableCollection<ISketchItemModel>();
        readonly List<UI.Utilities.Interfaces.ICommandDescriptor> _fileTools;
        readonly List<UI.Utilities.Interfaces.ICommandDescriptor> _alignTools;
        readonly List<UI.Utilities.Interfaces.ICommandDescriptor> _zoomTools;
        UmlShapeFactory _sketchItemFactory = new Uml.UmlShapeFactory();

        
        string _fileName;

        public ApplicationViewModel()
        {

            _sketch = new Sketch.Models.Sketch()
            {
                SketchItemFactory = _sketchItemFactory
            };
            _sketchItemFactory.RegisterBoundedItemSelectedNotification((x, y) => IsInsertMode = true);

            _editMode = global::Sketch.Types.EditMode.Insert;
            _isInsertMode = true;
            _isEditMode = false;
            _editCursor = Cursors.Arrow;
            //_cursorBitmap = Properties.Resources.UmlClassShape;
            //_insertCursor = UI.Utilities.BitmapToCursor.CreateCursor(_bmp, 1, 1);
            //_insertCursor = Cursors.Cross;
            _registeredCursors.Add(typeof(UmlClassModel), UI.Utilities.BitmapToCursor.CreateCursor(_insertClass, 1, 1));
            _registeredCursors.Add(typeof(UmlStateModel), UI.Utilities.BitmapToCursor.CreateCursor(_insertState, 1, 1));
            _registeredCursors.Add(typeof(UmlNoteModel), UI.Utilities.BitmapToCursor.CreateCursor(_insertNote, 1, 1));
            _registeredCursors.Add(typeof(UmlPackageModel), UI.Utilities.BitmapToCursor.CreateCursor(_insertPackage, 1, 1));
            _registeredCursors.Add(typeof(UmlChoiceModel), UI.Utilities.BitmapToCursor.CreateCursor(_insertChoice, 1, 1));
            _registeredCursors.Add(typeof(PictureModel), UI.Utilities.BitmapToCursor.CreateCursor(_insertPicture, 1, 1));


            _deleteEntry = new DelegateCommand(DoDeleteEntries);
            _cmdSave = new DelegateCommand(SaveDrawing);
            _cmdOpen = new DelegateCommand(OpenDrawing);
            _cmdSavePng = new DelegateCommand(SavePng);
            _cmdAlignLeft = new DelegateCommand(this.Sketch.AlignLeft);
            _cmdAlignCenter = new DelegateCommand(this.Sketch.AlignCenter);
            _cmdAlignTop = new DelegateCommand(this.Sketch.AlignTop);

            _cmdZoomIn = new DelegateCommand(this.Sketch.ZoomIn);
            _cmdZoomOut = new DelegateCommand(this.Sketch.ZoomOut);
            
            

            _fileTools = new List<UI.Utilities.Interfaces.ICommandDescriptor>
            {
                new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdOpen,
                    Bitmap = DrawIt.Properties.Resources.Open_file_icon,
                    Name = "Open"
                },
                new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdSave,
                    Bitmap = DrawIt.Properties.Resources.Save_as_icon,
                    Name = "Save as.."
                },
                new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdSavePng,
                    Bitmap = global::Sketch.Properties.Resources.SaveAsPicture,
                    Name = "Save PNG.."
                }
            };

            _alignTools = new List<UI.Utilities.Interfaces.ICommandDescriptor>
            {
                 new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdAlignLeft,
                    Bitmap = global::Sketch.Properties.Resources.left_align,
                    Name = "align left"
                },
                 new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdAlignCenter,
                    Bitmap = global::Sketch.Properties.Resources.align_center,
                    Name = "align center"
                },
                new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdAlignTop,
                    Bitmap = global::Sketch.Properties.Resources.top_align,
                    Name = "align top"
                },
               
            };

            _zoomTools = new List<UI.Utilities.Interfaces.ICommandDescriptor>()
            {
                new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdZoomIn,
                    Bitmap = DrawIt.Properties.Resources.ZoomIn,
                    Name = "Zoom In"
                },

                new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdZoomOut,
                    Bitmap = DrawIt.Properties.Resources.ZoomOut,
                    Name = "Zoom Out"
                }
            };
            //_cmdOpen = new DelegateCommand(LodadDrawing);
        }

        ~ApplicationViewModel()
        {
            foreach( var c in _registeredCursors.Values)
            {
                c.Dispose();
            }
            _registeredCursors.Clear();
            
        }

        void Outlines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ( e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                IsInsertMode = false;
            }
        }



        internal IList<UI.Utilities.Interfaces.ICommandDescriptor> FileTools
        {
            get
            {
                return _fileTools;
            }
        }

        internal IList<UI.Utilities.Interfaces.ICommandDescriptor> AlignmentTools
        {
            get
            {
                return _alignTools;
            }
        }

        internal IList<UI.Utilities.Interfaces.ICommandDescriptor> ZoomTools
        {
            get
            {
                return _zoomTools;
            }
        }

        public ICommand ZoomIn => _cmdZoomIn;

        public ICommand DeleteEntries
        {
            get
            {
                return _deleteEntry;
            }
        }

        public string Label
        {
            get => _sketch.Label;
            set => _sketch.Label = value;
        }

        public object SelectedItem
        {
            get => _selectedItem;
            set => SetProperty<object>(ref _selectedItem, value);
        }

        public EditMode EditMode
        {
            get { return _editMode; }
            set { SetProperty<EditMode>(ref _editMode, value); RaisePropertyChanged("Cursor"); }
        }

        public bool IsInsertMode
        {
            get { return _isInsertMode; }
            set { SetProperty<bool>(ref _isInsertMode, value);
                //IsEditMode = !_isInsertMode;
                EditMode = value? global::Sketch.Types.EditMode.Insert: global::Sketch.Types.EditMode.Select;
                
            }
        }

        public System.Windows.Input.Cursor Cursor
        {
            get
            {
                if (_isInsertMode )
                {
                    if (!_registeredCursors.TryGetValue(_sketchItemFactory.SelectedForCreation, out Cursor insertCursor))
                    {
                        insertCursor = Cursors.Hand;
                    }
                    return insertCursor;
                }
                return _editCursor;
            }
            
        }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty<bool>(ref _isEditMode, value);
                //IsInsertMode = !_isEditMode;
                EditMode = value ? global::Sketch.Types.EditMode.Select : global::Sketch.Types.EditMode.Insert;

            }
        }

        public Sketch.Models.Sketch Sketch
        {
            get { return _sketch; }
        }

        void DoDeleteEntries()
        {
            Sketch.DeleteMarked();
        }

        void SaveDrawing()
        {
            bool silent = true;
            if (string.IsNullOrEmpty(_fileName))
            {
                var dlg = new Microsoft.Win32.SaveFileDialog();
                var result = dlg.ShowDialog();
                if (result != true)
                {
                    return; // cancel operation
                }
                silent = false;
                _fileName = dlg.FileName;
            }
            Sketch.SaveFile(_fileName, silent);
        }

        void OpenDrawing()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == true)
            {
                _fileName = dlg.FileName;
                Label = System.IO.Path.GetFileNameWithoutExtension(_fileName);
                Sketch.OpenFile(_fileName);
                
            }
        }

        void SavePng(  )
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            { Title = "Save PNG as" };
            
            var result = dlg.ShowDialog();
            if( result == true)
            {
                Sketch.ExportDiagram(dlg.FileName);
            }
        }

       
    }
}
