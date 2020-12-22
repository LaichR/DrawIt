using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Helper
{
    internal static class AllowedDockings
    {
        public static readonly Dictionary<RelativePosition, List<LineType>> Table = new Dictionary<RelativePosition, List<LineType>>
        {
            {RelativePosition.E, new List<LineType>{LineType.RightLeft, LineType.TopTop, LineType.BottomBottom}},
            {RelativePosition.SE, new List<LineType>{LineType.RightTop, LineType.BottomLeft, LineType.RightLeft, LineType.BottomTop}},
            {RelativePosition.S, new List<LineType>{LineType.BottomTop, LineType.LeftLeft, LineType.RightRight}},
            {RelativePosition.SW, new List<LineType>{LineType.LeftTop, LineType.BottomRight, LineType.LeftRight, LineType.BottomTop}},
            {RelativePosition.W, new List<LineType>{LineType.LeftRight, LineType.TopTop, LineType.BottomBottom}},
            {RelativePosition.NW, new List<LineType>{LineType.TopRight, LineType.LeftBottom, LineType.BottomBottom, LineType.TopTop}},
        };
    }
}
