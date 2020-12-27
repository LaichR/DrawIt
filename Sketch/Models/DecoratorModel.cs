using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using Sketch.Interface;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Runtime.Serialization;
using Sketch.Helper.UiUtilities;

namespace Sketch.Models
{
    public abstract class DecoratorModel:ISerializable
    {
        public static readonly Pen DefaultPen = new Pen(Brushes.Black, 1);

        List<FieldInfo> _persistentFields;

        [PersistentField((int)ModelVersion.V_0_1, "Side" )]
        readonly ConnectorDocking _side;

        [PersistentField((int)ModelVersion.V_0_1, "Fill")]
        readonly SerializableColor _color = new SerializableColor() { Color = Colors.DarkGray };

        [PersistentField((int)ModelVersion.V_0_1, "Id")]
        readonly ulong _id;

        double _relativePosition;

        public DecoratorModel(ConnectorDocking side, ulong id)
        {
            _side = side;
            _id = id;
        }

        public ulong Id
        {
            get => _id;
        }

        public abstract Point Location
        {
            get;
            set;
        }

        public abstract Rect Bounds
        {
            get;
        }




        public ConnectorDocking Side
        {
            get => _side;
        }

        public double RelativePosition
        {
            get => _relativePosition;
            set => _relativePosition = value;
        }

        public abstract Geometry Geometry
        {
            get;
        }

        public virtual Brush Fill
        {
            get => _color.Brush;
        }

        public virtual Pen Pen
        {
            get => DefaultPen;
        }


        protected DecoratorModel(SerializationInfo info, StreamingContext context)
        {
            PersistencyHelper.RestorePersistentFields(this, info, PersistentFields, GetModelVersion());
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            PersistencyHelper.BackupPersistentFields(this, info, PersistentFields);
        }

        IEnumerable<FieldInfo> PersistentFields
        {
            get
            {
                if (_persistentFields == null)
                {
                    _persistentFields = new List<FieldInfo>(
                        PersistencyHelper.GetAllPersistentFields(this, typeof(DecoratorModel).BaseType));
                }
                return _persistentFields;
            }
        }

        public virtual int GetModelVersion()
        {
            return (int)ModelVersion.V_2_1;
        }
    }
}
