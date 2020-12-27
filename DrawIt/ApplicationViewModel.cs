using System;
using System.Collections.Generic;
using Sketch.Helper;
using System.Windows.Input;
using Sketch.Helper.Binding;
using DrawIt.Uml;
using DrawIt.Shapes;
using Sketch.Interface;
using System.Windows.Controls;
using Sketch.View.CustomControls;
using Sketch.Helper.UiUtilities;
using Sketch.Models;

namespace DrawIt
{
    public class ApplicationViewModel : BindableModel
    {
        //DelegateCommand _cmdInsertMode;
        //DelegateCommand _cmdEditMode;

        readonly DelegateCommand _cmdSave;
        readonly DelegateCommand _cmdSaveAs;
        readonly DelegateCommand _cmdOpen;
        readonly DelegateCommand _cmdSavePng;

        readonly DelegateCommand _deleteEntry;

        readonly DelegateCommand _cmdAlignTop;
        readonly DelegateCommand _cmdAlignLeft;
        readonly DelegateCommand _cmdAlignCenter;
        readonly DelegateCommand _cmdSameWidth;
        readonly DelegateCommand _cmdSameVerticalSpace;

        readonly DelegateCommand _cmdZoomIn;
        readonly DelegateCommand _cmdZoomOut;

        readonly Dictionary<Type, System.Windows.Input.Cursor> _registeredCursors = new Dictionary<Type, Cursor>();

        bool _isEditMode;
        bool _isInsertMode;
        int _zoomDepth;
        
        double _scaling;

        //string _label = "No Name";
        EditMode _editMode;
        object _selectedItem;
        readonly Sketch.Models.Sketch _sketch;
        readonly System.Windows.Input.Cursor _editCursor;

        static readonly System.Drawing.Bitmap _insertPackage = Properties.Resources.UmlPackageShape;
        static readonly System.Drawing.Bitmap _insertState = Properties.Resources.UmlStateShape;
        static readonly System.Drawing.Bitmap _insertClass = Properties.Resources.UmlClassShape;
        static readonly System.Drawing.Bitmap _insertNote = Properties.Resources.UmlNoteShape;
        static readonly System.Drawing.Bitmap _insertChoice = Properties.Resources.UmlChoiceShape;
        static readonly System.Drawing.Bitmap _insertPicture = Properties.Resources.image;


        //readonly ObservableCollection<ISketchItemModel> _sketchItems = new ObservableCollection<ISketchItemModel>();
        readonly List<ICommandDescriptor> _fileTools;
        readonly List<ICommandDescriptor> _alignTools;
        readonly List<ICommandDescriptor> _zoomTools;

        readonly List<string> _supportedDiagrams = new List<string>()
        {
            "Class Diagram", "others"
        };
        readonly SketchItemFactory _factory = new Uml.UmlShapeFactory();
        
        


        string _fileName;

        public ApplicationViewModel()
        {
            _sketch = new Sketch.Models.Sketch()
            {
                SketchItemFactory = _factory
            };
            _factory.PropertyChanged += Factory_PropertyChanged;
            
            _editMode = global::Sketch.Helper.EditMode.Insert;
            _isInsertMode = true;
            _isEditMode = false;
            _editCursor = Cursors.Arrow;
            _scaling = 1.0;

            //_cursorBitmap = Properties.Resources.UmlClassShape;
            //_insertCursor = BitmapToCursor.CreateCursor(_bmp, 1, 1);
            //_insertCursor = Cursors.Cross;
            _registeredCursors.Add(typeof(UmlClassModel), BitmapToCursor.CreateCursor(_insertClass, 1, 1));
            _registeredCursors.Add(typeof(UmlStateModel), BitmapToCursor.CreateCursor(_insertState, 1, 1));
            _registeredCursors.Add(typeof(UmlNoteModel), BitmapToCursor.CreateCursor(_insertNote, 1, 1));
            _registeredCursors.Add(typeof(UmlPackageModel), BitmapToCursor.CreateCursor(_insertPackage, 1, 1));
            _registeredCursors.Add(typeof(UmlChoiceModel), BitmapToCursor.CreateCursor(_insertChoice, 1, 1));
            _registeredCursors.Add(typeof(PictureModel), BitmapToCursor.CreateCursor(_insertPicture, 1, 1));


            _deleteEntry = new DelegateCommand(DoDeleteEntries);
            _cmdSave = new DelegateCommand(SaveDrawing);
            _cmdSaveAs = new DelegateCommand(SaveDrawingAs);
            _cmdOpen = new DelegateCommand(OpenDrawing);
            _cmdSavePng = new DelegateCommand(SavePng);
            _cmdAlignLeft = new DelegateCommand(this.Sketch.AlignLeft);
            _cmdAlignCenter = new DelegateCommand(this.Sketch.AlignCenter);
            _cmdAlignTop = new DelegateCommand(this.Sketch.AlignTop);
            _cmdSameVerticalSpace = new DelegateCommand(this.Sketch.SetEqualVerticalSpacing);
            _cmdSameWidth = new DelegateCommand(this.Sketch.SetToSameWidth);

            _cmdZoomIn = new DelegateCommand(this.Sketch.ZoomIn, ()=> SelectedItem is Sketch.Models.ContainerModel );
            _cmdZoomOut = new DelegateCommand(this.Sketch.ZoomOut, ()=> ZoomDepth > 0);
            
            

            _fileTools = new List<ICommandDescriptor>
            {
                new CommandDescriptor
                {
                    Command = _cmdOpen,
                    Bitmap = global::Sketch.Properties.Resources.open_file,
                    Name = "Open"
                },
                new CommandDescriptor
                {
                    Command = _cmdSave,
                    Bitmap = global::Sketch.Properties.Resources.save_file,
                    Name = "Save"
                },
                new CommandDescriptor
                {
                    Command = _cmdSaveAs,
                    Bitmap = global::Sketch.Properties.Resources.save_file_as,
                    Name = "Save as .."
                },
                new CommandDescriptor
                {
                    Command = _cmdSavePng,
                    Bitmap = global::Sketch.Properties.Resources.SaveAsPicture,
                    Name = "Save PNG.."
                }
            };

            _alignTools = new List<ICommandDescriptor>
            {
                 new CommandDescriptor
                {
                    Command = _cmdAlignLeft,
                    Bitmap = global::Sketch.Properties.Resources.left_align,
                    Name = "align left"
                },
                 new CommandDescriptor
                {
                    Command = _cmdAlignCenter,
                    Bitmap = global::Sketch.Properties.Resources.align_center,
                    Name = "align center"
                },
                new CommandDescriptor
                {
                    Command = _cmdAlignTop,
                    Bitmap = global::Sketch.Properties.Resources.top_align,
                    Name = "align top"
                },
                new CommandDescriptor
                {
                    Command = _cmdSameWidth,
                    Bitmap = global::Sketch.Properties.Resources.equal_sizing,
                    Name = "same width"
                },
                new CommandDescriptor
                {
                    Command = _cmdSameVerticalSpace,
                    Bitmap = global::Sketch.Properties.Resources.equal_spacing,
                    Name = "equal spacing"
                },

            };

            _zoomTools = new List<ICommandDescriptor>()
            {
                new CommandDescriptor
                {
                    Command = _cmdZoomIn,
                    Bitmap = DrawIt.Properties.Resources.ZoomIn,
                    Name = "Zoom In"
                },

                new CommandDescriptor
                {
                    Command = _cmdZoomOut,
                    Bitmap = DrawIt.Properties.Resources.ZoomOut,
                    Name = "Zoom Out"
                }
            };
            //_cmdOpen = new DelegateCommand(LodadDrawing);
        }

        private void Factory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SketchItemFactory.SelectedItemBitmap))
            {
                if (Sketch.SketchItemFactory.SelectedForCreation != null &&
                    Sketch.SketchItemFactory.SelectedForCreation.GetInterface(nameof(IBoundedSketchItemModel)) != null)
                {
                    IsInsertMode = true;
                }
                else
                {
                    IsEditMode = true;
                }
            }
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



        internal IList<ICommandDescriptor> FileTools
        {
            get
            {
                return _fileTools;
            }
        }

        internal IList<ICommandDescriptor> AlignmentTools
        {
            get
            {
                return _alignTools;
            }
        }

        internal IList<ICommandDescriptor> ZoomTools
        {
            get
            {
                return _zoomTools;
            }
        }

        public double Scaling
        {
            get => _scaling;
            set
            {
                SetProperty<double>(ref _scaling, value);
                RaisePropertyChanged(nameof(SketchScalePercentage));
            }
        }

        public int SketchScalePercentage
        {
            get => (int)Math.Round(_scaling * 100.0);
            set => Scaling = value / 100.0;
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
            set
            {
                _sketch.Label = value;
                RaisePropertyChanged(nameof(Label));
            }
        }

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty<object>(ref _selectedItem, value);
                _cmdZoomIn.RaiseCanExecuteChanged();
            }
        }

        public int ZoomDepth
        {
            get => _zoomDepth;
            set
            {
                _zoomDepth = value;
                _cmdZoomOut.RaiseCanExecuteChanged();
            }
        }

        public EditMode EditMode
        {
            get { return _editMode; }
            set { 
                
                SetProperty<EditMode>(ref _editMode, value);
                _isInsertMode = _editMode == EditMode.Insert;
                _isEditMode = _editMode == EditMode.Select;
                RaisePropertyChanged(nameof(IsInsertMode));
                RaisePropertyChanged(nameof(IsEditMode));
                RaisePropertyChanged("Cursor"); }
        }

        public bool IsInsertMode
        {
            get { return _isInsertMode; }
            set { SetProperty<bool>(ref _isInsertMode, value);
                //IsEditMode = !_isInsertMode;
                EditMode = value? global::Sketch.Helper.EditMode.Insert: global::Sketch.Helper.EditMode.Select;
                
            }
        }

        

        public System.Windows.Input.Cursor Cursor
        {
            get
            {
                if (_isInsertMode )
                {
                    Cursor insertCursor = Cursors.Hand;
                    if( Sketch.SketchItemFactory.SelectedForCreation != null)
                    {
                        if (!_registeredCursors.TryGetValue(Sketch.SketchItemFactory?.SelectedForCreation, out insertCursor))
                        {
                            if (_factory.SelectedItemBitmap != null)
                            {
                                insertCursor = BitmapToCursor.CreateCursor(_factory.SelectedItemBitmap, 1, 1);
                                _registeredCursors.Add(Sketch.SketchItemFactory.SelectedForCreation, insertCursor);
                            }
                        }
                    }
                    return insertCursor;
                }
                return _editCursor;
            }
            
        }

        public IList<ISketchItemGroup> SupportedDiagrams
        {
            get => _factory.ItemGroups;
        }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty<bool>(ref _isEditMode, value);
                IsInsertMode = !_isEditMode;
                EditMode = value ? global::Sketch.Helper.EditMode.Select : global::Sketch.Helper.EditMode.Insert;

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

        void SaveDrawingAs()
        {
            _fileName = SaveFile("");
        }

        void SaveDrawing()
        {
            _fileName = SaveFile(_fileName);
        }

        string SaveFile(string fName )
        {
            var newName = fName;
            if (string.IsNullOrEmpty(fName))
            {
                var dlg = new Microsoft.Win32.SaveFileDialog();
                var result = dlg.ShowDialog();
                if (result != true)
                {
                    return ""; // cancel operation
                }
                newName = dlg.FileName;
            }
            Sketch.SaveFile(newName);
            return newName;
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

        List<MenuItem> CreateDiagramTools(IEnumerable<ICommandDescriptor> commandDescriptors)
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            foreach( var d in commandDescriptors)
            {
                menuItems.Add(new MenuItem()
                {
                    //Name = d.Name,
                    Command = d.Command,
                    Icon = d.Bitmap,
                    ToolTip = d.Name
                }
                );
            }
            return menuItems;
        }


    }
}
