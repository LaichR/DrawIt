using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Sketch.Interface;
using Sketch.Models;
using System.Windows.Controls;
using Sketch.Controls;


namespace Sketch.Utilities
{
    public static class SketchItemDisplayHelper
    {
        public static void SaveAsPng(Canvas canvas, string fileName)
        {
            // determin the 
            var minX = (int)canvas.ActualWidth;
            var maxX = 0;
            var minY = (int)canvas.ActualHeight;
            var maxY = 0;


            RenderTargetBitmap bmp = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);


            foreach (var ui in canvas.Children.OfType<ConnectorUI>())
            {
                ui.IsSelected = false;
               
                AdjustBoundaries(ui.Model.Geometry.Bounds, ref minX, ref maxX, ref minY, ref maxY);

            }


            foreach (var ui in canvas.Children.OfType<OutlineUI>())
            {
                ui.IsSelected = false;
               
                AdjustBoundaries(ui.Model.Geometry.Bounds, ref minX, ref maxX, ref minY, ref maxY);
            }

            // x and y must not be smaller than 0
            var x = 0; // Math.Max(0, (minX - 50));
            var y = 0; // Math.Max(0, (minY - 50));



            //var label = canvas.Children.OfType<SketchItemDisplayLabel>().First();

            

            //var oldX = Canvas.GetLeft(label);
            //var oldY = Canvas.GetTop(label);

            //Canvas.SetLeft(label, x + 10);
            //Canvas.SetTop(label, y + 5);

            
            bmp.Render(canvas);

            var adornders = AdornerLayer.GetAdornerLayer(canvas).GetAdorners(canvas)?.OfType<IntersectionFinder>();
            if(adornders != null && adornders.Any())
            {
                bmp.Render(adornders.First());
            }

            

            var width = (int)Math.Min(canvas.ActualWidth, (maxX - x + 100));
            var height = (int)Math.Min(canvas.ActualHeight, (maxY - y + 100));


            CroppedBitmap cropped = new CroppedBitmap(bmp, new Int32Rect(x, y, width, height));

            //bmp.Render( this );
            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(cropped));

            using (System.IO.Stream stm = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                encoder.Save(stm);
            }
        }

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
        public static void SetEqualVerticalSpacing(IEnumerable<ISketchItemModel> outlines)
        {
            var orderedUis = outlines.OfType<ConnectableBase>().Where((x) => x.IsMarked).OrderBy((x) => x.Geometry.Bounds.Top);
            int nrOfItems = orderedUis.Count();
            if (nrOfItems>2)
            {
                var first = orderedUis.First();
                var last = orderedUis.Last();
                var delta = (last.Bounds.Top - first.Bounds.Top) / (nrOfItems - 1);
                var top = first.Bounds.Top;
                foreach( var m in orderedUis.Skip(1))
                {
                    var targetY = top + delta;
                    var dy = m.Bounds.Top - targetY;
                    m.Move(new TranslateTransform(0, -dy));
                    top = targetY;
                }
            }
        }

        public static void SetToSameWidth(IEnumerable<ISketchItemModel> outlines)
        {
            var orderedUis = outlines.Where((x) => x is ConnectableBase && x.IsMarked).OrderBy((x) => x.Geometry.Bounds.Top);
            if (orderedUis.Count() > 1)
            {
                var first = orderedUis.First();
                var anchor = first as ConnectableBase;
                var targetWidht = (anchor.Bounds.Width);
                foreach (var model in orderedUis.Skip(1))
                {
                    var cb = model as ConnectableBase;
                    var bounds = cb.Bounds;
                    bounds.Width = targetWidht;
                    cb.Bounds = bounds;
                }
            }
        }

        public static void TakeSnapshot( Stream stream, ISketchItemContainer container)
        {
            var outlines = container.SketchItems;
            var ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(SketchItemContainerProxy),
                new StreamingContext(StreamingContextStates.All),
                new SketchItemContainerSerializationSurrogate(container));
            

            IFormatter formatter = new BinaryFormatter()
            {
                SurrogateSelector = ss
            };

            List<ISketchItemModel> list = new List<ISketchItemModel>(outlines.Where(
                (x)=>x.IsSerializable));
            formatter.Serialize(stream, list);
        }

        public static void RestoreSnapshot(Stream stream, ISketchItemContainer container )
        {
            IList<ISketchItemModel> outlines = container.SketchItems;
            outlines.Clear();
            var ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(SketchItemContainerProxy),
                new StreamingContext(StreamingContextStates.All),
                new SketchItemContainerSerializationSurrogate(container));


            IFormatter formatter = new BinaryFormatter()
            {
                SurrogateSelector = ss
            };
            var list = (List<ISketchItemModel>)formatter.Deserialize(stream);
            RestoreChildren(outlines, list);
        }

        internal static void RestoreChildren(IList<ISketchItemModel> outlines, List<ISketchItemModel> children)
        {
            //
            // is this needed? what happens if i just restore!
            // it was possible to handle the dependencies while doing the deserialisation as well?
            //
            foreach (var connectable in children.OfType<ConnectableBase>())
            {
                outlines.Add(connectable);
            }

            foreach (var connector in children.OfType<ConnectorModel>())
            {
                outlines.Add(connector);
            }
        }

        private static void AdjustBoundaries(Rect boundaries, ref int left, ref int right, ref int top, ref int bottom)
        {
            if (boundaries.Left < left)
            {
                left = (int)boundaries.Left;
            }
            if (boundaries.Right > right)
            {
                right = (int)boundaries.Right;
            }
            if (boundaries.Top < top)
            {
                top = (int)boundaries.Top;
            }
            if (boundaries.Bottom > bottom)
            {
                bottom = (int)boundaries.Bottom;
            }
        }
    }
}
