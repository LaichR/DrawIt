using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sketch.Models;
using Sketch.Helper;
using System.Windows;

namespace Sketch.Test
{
    [TestFixture]
    public class AvlTreeTest
    {
        [Test]
        public void TestBoundedItemInsertAndFind()
        {
            List<Rect> rectangles = new List<Rect>();
            LinkedAvlTree<BoundsComparer> tree = new LinkedAvlTree<BoundsComparer>();
            CreateRectangles(100, tree, rectangles);
            

            foreach ( var r in rectangles )
            {
                Assert.IsNotNull(tree.Find(r));
            }

        }

        [Test]
        public void TestBoundedItemOrder()
        {
            List<Rect> rectangles = new List<Rect>();
            LinkedAvlTree<BoundsComparer> tree = new LinkedAvlTree<BoundsComparer>();
            CreateRectangles(100, tree, rectangles);

            var left = -1.0;
            var lowerBound = tree.LowerBound(rectangles.First());
            var previous = lowerBound;
            while( lowerBound != null)
            {
                if(left > lowerBound.Data.Left)
                {
                    Assert.True(left <= lowerBound.Data.Left);
                }
                
                left = lowerBound.Data.Left;
                previous = lowerBound;
                lowerBound = lowerBound.Next;
            }
        }
        void CreateRectangles(int count, LinkedAvlTree<BoundsComparer> tree,
            List<Rect> rectangles)
        {
            var rand = new Random(-127);
            
            for (int i = 0; i < count; i++)
            {
                var x = rand.NextDouble() * 2048;
                var y = rand.NextDouble() * 1600;
                var w = 150;
                var h = 50;
                var p = new Point(x, y);
                var s = new Size(w, h);
                var r = new Rect(p, s);
                tree.Add(r);
                if (tree.Count == i + 1)
                {
                    rectangles.Add(r);
                }
                
            }

        }
    }
}
