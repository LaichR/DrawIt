using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Sketch.Interface
{
    public interface ISketchItemModel: ISerializable, ISketchItemNode
    {

        string LabelPropertyName
        {
            get;
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
