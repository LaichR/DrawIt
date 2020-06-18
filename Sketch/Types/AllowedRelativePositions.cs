using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Types
{
    internal static class AllowedRelativePositions
    {
        static readonly SortedSet<RelativePosition> _south = new SortedSet<RelativePosition> { RelativePosition.S };
        static readonly SortedSet<RelativePosition> _southWest = new SortedSet<RelativePosition> { RelativePosition.SW };
        static readonly SortedSet<RelativePosition> _southEast = new SortedSet<RelativePosition> { RelativePosition.SE };
        static readonly SortedSet<RelativePosition> _north = new SortedSet<RelativePosition> { RelativePosition.N };
        static readonly SortedSet<RelativePosition> _northWest = new SortedSet<RelativePosition> { RelativePosition.NW };
        static readonly SortedSet<RelativePosition> _northEast = new SortedSet<RelativePosition> { RelativePosition.NE };

        static readonly SortedSet<RelativePosition> _anySouthernPos = new SortedSet<RelativePosition>
            {
                RelativePosition.S, RelativePosition.SW, RelativePosition.SE
            };
        static readonly SortedSet<RelativePosition> _anyNorthernPos = new SortedSet<RelativePosition>
            {
                RelativePosition.N, RelativePosition.NW, RelativePosition.NE
            };
        static readonly SortedSet<RelativePosition> _anyWesternPos = new SortedSet<RelativePosition>
            {
                RelativePosition.W, RelativePosition.NW, RelativePosition.SW
            };
        static readonly SortedSet<RelativePosition> _anyEasternPos = new SortedSet<RelativePosition>
            {
                RelativePosition.E, RelativePosition.NE, RelativePosition.SE
            };

        static readonly SortedSet<RelativePosition> _allButEastOrWest = new SortedSet<RelativePosition>
            {
                RelativePosition.N, RelativePosition.NE, RelativePosition.NW,
                RelativePosition.S, RelativePosition.SE, RelativePosition.SW
            };
        static readonly SortedSet<RelativePosition> _allButNorthOrSouth = new SortedSet<RelativePosition>
            {
                RelativePosition.E, RelativePosition.NE, RelativePosition.SE,
                RelativePosition.W, RelativePosition.NW, RelativePosition.SW
            };

        public static readonly Dictionary<LineType, SortedSet<RelativePosition>>
            Table = new Dictionary<LineType, SortedSet<RelativePosition>>
            {
                {LineType.TopBottom, _anyNorthernPos},
                {LineType.BottomTop, _anySouthernPos},
                {LineType.LeftRight, _anyWesternPos},
                {LineType.RightLeft,  _anyEasternPos},
                {LineType.LeftTop, _southWest},
                {LineType.TopLeft, _northEast},
                {LineType.TopRight, _northWest},
                {LineType.RightTop,_southEast},
                {LineType.BottomLeft, _southEast},
                {LineType.LeftBottom,_northWest},
                {LineType.BottomRight,_southWest},
                {LineType.RightBottom, _northEast},
                {LineType.LeftLeft, _allButEastOrWest},
                {LineType.RightRight, _allButEastOrWest},
                {LineType.TopTop, _allButNorthOrSouth},
                {LineType.BottomBottom, _allButNorthOrSouth},
            };
    }
}
