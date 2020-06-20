using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Sketch.Interface
{
    public interface ISketchItemModel: ISerializable
    {

        string Name
        {
            get;
            set;
        }

        string LabelPropertyName
        {
            get;
        }

        bool AllowEdit
        {
            get;
            set;
        }

        void UpdateGeometry();

        Geometry Geometry
        {
            get;
        }

        ISketchItemModel RefModel
        {
            get;
        }

        bool IsMarked
        {
            get;
        }

        bool IsSelected
        {
            get;
        }

        bool IsSerializable
        {
            get;
        }

        void Move(Transform translation);

    }
}
