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
        , ISketchItemContainer, IColorSelectionTarget
    {
        DelegateCommand _cmdInsertMode;
        DelegateCommand _cmdEditMode;

        DelegateCommand _cmdSave;
        DelegateCommand _cmdOpen;
        DelegateCommand _cmdSavePng;

        DelegateCommand _deleteEntry;

        DelegateCommand _cmdAlignTop;
        DelegateCommand _cmdAlignLeft;
        DelegateCommand _cmdAlignCenter;

        Dictionary<Type, System.Windows.Input.Cursor> _registeredCursors = new Dictionary<Type, Cursor>();

        bool _isEditMode;
        bool _isInsertMode;
        EditMode _editMode;
        
        System.Windows.Input.Cursor _editCursor;
        System.Windows.Input.Cursor _insertCursor;



        static readonly System.Drawing.Bitmap _insertPackage = Properties.Resources.UmlPackageShape;
        static readonly System.Drawing.Bitmap _insertState = Properties.Resources.UmlStateShape;
        static readonly System.Drawing.Bitmap _insertClass = Properties.Resources.UmlClassShape;
        static readonly System.Drawing.Bitmap _insertNote = Properties.Resources.UmlNoteShape;
        static readonly System.Drawing.Bitmap _insertChoice = Properties.Resources.UmlChoiceShape;
        static readonly System.Drawing.Bitmap _insertPicture = Properties.Resources.image;


        ObservableCollection<ISketchItemModel> _sketchItems = new ObservableCollection<ISketchItemModel>();
        List<UI.Utilities.Interfaces.ICommandDescriptor> _fileTools;
        List<UI.Utilities.Interfaces.ICommandDescriptor> _alignTools;


        public ApplicationViewModel(Action<string> savePng )
        { 
            
            _editMode = Sketch.Types.EditMode.Insert;
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
            _cmdSavePng = new DelegateCommand(() => SavePng(savePng));
            _cmdAlignLeft = new DelegateCommand(AlignLeft);
            _cmdAlignCenter = new DelegateCommand(AlignCenter);
            _cmdAlignTop = new DelegateCommand(AlignTop);
            
            //_outlines.CollectionChanged += _outlines_CollectionChanged;

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
                    Bitmap = DrawIt.Properties.Resources.Actions_system_run_icon,
                    Name = "Save PNG.."
                }
            };

            _alignTools = new List<UI.Utilities.Interfaces.ICommandDescriptor>
            {
                 new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdAlignLeft,
                    Bitmap = Sketch.Properties.Resources.left_align,
                    Name = "align left"
                },
                 new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdAlignCenter,
                    Bitmap = Sketch.Properties.Resources.align_center,
                    Name = "align center"
                },
                new UI.Utilities.Behaviors.CommandDescriptor
                {
                    Command = _cmdAlignTop,
                    Bitmap = Sketch.Properties.Resources.top_align,
                    Name = "align top"
                },
               
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

        void _outlines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

        public ICommand DeleteEntries
        {
            get
            {
                return _deleteEntry;
            }
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
                EditMode = value? Sketch.Types.EditMode.Insert: Sketch.Types.EditMode.Select;
                
            }
        }

        public System.Windows.Input.Cursor Cursor
        {
            get
            {
                if (_isInsertMode )
                {
                    var factory = Sketch.ModelFactoryRegistry.Instance.GetSketchItemFactory() as Uml.UmlShapeFactory;
                    if (!_registeredCursors.TryGetValue(factory.SelectedForCreation, out Cursor insertCursor))
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
            EditMode = value ? Sketch.Types.EditMode.Select : Sketch.Types.EditMode.Insert;

            }
        }

        public System.Windows.Rect ChildArea
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.ObjectModel.ObservableCollection<ISketchItemModel> SketchItems
        {
            get { return _sketchItems; }
        }

        void DoDeleteEntries()
        {
            var toDelete = new List<ISketchItemModel>(_sketchItems.Where((x) => x.IsMarked||x.IsSelected));
            
            foreach( var m in toDelete)
            {
                _sketchItems.Remove(m);
            }
        }

        void SaveDrawing()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            var result = dlg.ShowDialog();
            if( result == true)
            {
                using( var stream = dlg.OpenFile() )
                {
                    SketchPadHelper.TakeSnapshot(stream, _sketchItems);
                }
            }
        }

        void OpenDrawing()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    
                    using (var stream = dlg.OpenFile())
                    {
                        SketchPadHelper.RestoreSnapshot(stream, 
                            _sketchItems);
                    }
                }
                finally
                {
                    RaisePropertyChanged("Children");
                }
            }
        }

        void SavePng( Action<string> savePng )
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = "Save PNG as";
            var result = dlg.ShowDialog();
            if( result == true)
            {
                savePng(dlg.FileName);
            }
        }

        public void AlignLeft()
        {
            SketchPadHelper.AlignLeft(_sketchItems);
        }

        public void AlignTop()
        {
            SketchPadHelper.AlignTop(_sketchItems);
        }

        public void AlignCenter()
        {
            SketchPadHelper.AlignCenter(_sketchItems);        }

        public void SetColor(Color newFill )
        {
            var uis = _sketchItems.OfType<Sketch.Models.ConnectableBase>().Where((x) => x.IsMarked);
            foreach( var ui in uis)
            {
                ui.FillColor = newFill;
            }
        }
    }
}
