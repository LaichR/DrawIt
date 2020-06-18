using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Sketch.Interface;
using Sketch.Models;

namespace Sketch.Utilities
{
    public static class SketchPadHelper
    {
        public static void AlignLeft(IEnumerable<ISketchItemModel> outlines)
        {
            RuntimeCheck.Contract.Requires(outlines != null, "list of shapes to align must not be null");
            var orderedUis = outlines.Where((x) => x is ConnectableBase && x.IsMarked).OrderBy((x) => x.Geometry.Bounds.Top);
            if (orderedUis.Count() > 1)
            {
                var first = orderedUis.First();
                var anchor = first as ConnectableBase;
                foreach (var model in orderedUis.Skip(1))
                {
                    var cb = model as ConnectableBase;
                    var dx = anchor.Bounds.Left - cb.Bounds.Left;
                    cb.Move(new TranslateTransform(dx, 0));
                }
            }
        }

        public static void AlignTop(IEnumerable<ISketchItemModel> outlines)
        {
            RuntimeCheck.Contract.Requires(outlines != null, "list of shapes to align must not be null");
            var orderedUis = outlines.Where((x) => x is ConnectableBase && x.IsMarked).OrderBy((x) => x.Geometry.Bounds.Left);
            if (orderedUis.Count() > 1)
            {
                var first = orderedUis.First();
                var anchor = first as ConnectableBase;
                foreach (var model in orderedUis.Skip(1))
                {
                    var cb = model as ConnectableBase;
                    var dy = anchor.Bounds.Top - cb.Bounds.Top;
                    cb.Move(new TranslateTransform(0, dy));
                }
            }
        }

        public static void AlignCenter(IEnumerable<ISketchItemModel> outlines)
        {
            RuntimeCheck.Contract.Requires(outlines != null, "list of shapes to align must not be null");
            var orderedUis = outlines.Where((x) => x is ConnectableBase && x.IsMarked).OrderBy((x) => x.Geometry.Bounds.Top);
            if (orderedUis.Count() > 1)
            {
                var first = orderedUis.First();
                var anchor = first as ConnectableBase;
                var targetX = (anchor.Bounds.Left + anchor.Bounds.Right) / 2;
                foreach (var model in orderedUis.Skip(1))
                {

                    var cb = model as ConnectableBase;
                    var centerX = (cb.Bounds.Left + cb.Bounds.Right) / 2;
                    var dx = targetX - centerX;
                    cb.Move(new TranslateTransform(dx, 0));
                }
            }
        }

        public static void TakeSnapshot( Stream stream, IEnumerable<ISketchItemModel> outlines)
        {
            IFormatter formatter = new BinaryFormatter();
            List<ISketchItemModel> list = new List<ISketchItemModel>(outlines);
            formatter.Serialize(stream, list);
        }

        public static void RestoreSnapshot(Stream stream, IList<ISketchItemModel> outlines )
        {
            outlines.Clear();
            IFormatter formatter = new BinaryFormatter();
            var list = (List<ISketchItemModel>)formatter.Deserialize(stream);

            //
            // is this needed? what happens if i just restore!
            // it was possible to handle the dependencies while doing the deserialisation as well?
            //
            foreach (var connectable in list.OfType<ConnectableBase>())
            {
                outlines.Add(connectable);
            }

            foreach (var connector in list.OfType<ConnectorModel>())
            {
                outlines.Add(connector);
            }
        }
    }
}
