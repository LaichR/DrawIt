using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sketch.Helper;
using System.Windows;
using System.Collections.ObjectModel;

namespace Sketch.Test
{
    [TestFixture]
    public class RoutingAssistentTest 
    {
        
        [Test]
        public void TestFindRoutingSlotRight1()
        {

            List<Rect> rectangles = new List<Rect>()
            {
                new Rect(){X=10,Y=10, Height=50, Width=100},
                new Rect(){X=30,Y=200, Height=50, Width=100},
                new Rect(){X=50,Y=70, Height=50, Width=100},
                new Rect(){X=200,Y=10, Height=50, Width=100},
            };

            var ra = new RoutingAssistent();
            ra.InitalizeBoundSearchStructures(rectangles);
            var r = ra.FindVerticalRoutingSlotRight(rectangles[0].Left + rectangles[0].Width,
                rectangles[0].Y, rectangles[1].Y);
            Assert.IsTrue(r < 200 && r > 150);
           
        }
        [Test]
        public void TestFindRoutingSlotRight2()
        {

            List<Rect> rectangles = new List<Rect>()
            {
                new Rect(){X=10,Y=10, Height=50, Width=100},
                new Rect(){X=200,Y=10, Height=50, Width=100},
            };

            var ra = new RoutingAssistent();
            ra.InitalizeBoundSearchStructures(rectangles);
            var r = ra.FindVerticalRoutingSlotRight(rectangles[0].Left + rectangles[0].Width,
                rectangles[0].Y, rectangles[1].Y);
            Assert.IsTrue(r > 110 && r < 200 );

        }
        [Test]
        public void TestFindRoutingSlotLeft1()
        {

            List<Rect> rectangles = new List<Rect>()
            {
                new Rect(){X=10,Y=10, Height=50, Width=100},
                new Rect(){X=30,Y=200, Height=50, Width=100},
                new Rect(){X=50,Y=70, Height=50, Width=100},
                new Rect(){X=200,Y=10, Height=50, Width=100},
            };

            var ra = new RoutingAssistent();
            ra.InitalizeBoundSearchStructures(rectangles);
            var r = ra.FindVerticalRoutingSlotLeft(rectangles[3].Left-1,
                rectangles[0].Y, rectangles[1].Y);
            Assert.IsTrue(r <= 200 && r > 150);

        }

        [Test]
        public void TestFindRoutingSlotTop1()
        {
            List<Rect> rectangles = new List<Rect>()
            {
                new Rect(){X=10,Y=10, Height=50, Width=100},
                new Rect(){X=30,Y=200, Height=50, Width=100},
                new Rect(){X=50,Y=70, Height=50, Width=100},
                new Rect(){X=200,Y=10, Height=50, Width=100},
            };

            var ra = new RoutingAssistent();
            ra.InitalizeBoundSearchStructures(rectangles);
            var r = ra.FindHorizontalRoutingSlotTop(rectangles[1].Top-1,
                rectangles[0].X, rectangles[3].X);
            Assert.IsTrue(r <= 200 && r > 60);
        }

        [Test]
        public void TestFindRoutingSlotTop2()
        {
            List<Rect> rectangles = new List<Rect>()
            {
                new Rect(){X=10,Y=10, Height=50, Width=100},
                new Rect(){X=30,Y=200, Height=50, Width=100},
                new Rect(){X=50,Y=150, Height=50, Width=100},
                new Rect(){X=200,Y=10, Height=50, Width=100},
            };

            var ra = new RoutingAssistent();
            ra.InitalizeBoundSearchStructures(rectangles);
            var r = ra.FindHorizontalRoutingSlotTop(rectangles[1].Top - 1,
                rectangles[0].X, rectangles[3].X);
            Assert.IsTrue(r <= 150 && r > 60);
        }

        [Test]
        public void TestFindRoutingSlotBottom1()
        {
            List<Rect> rectangles = new List<Rect>()
            {
                new Rect(){X=10,Y=10, Height=50, Width=100},
                new Rect(){X=30,Y=200, Height=50, Width=100},
                new Rect(){X=50,Y=70, Height=50, Width=100},
                new Rect(){X=200,Y=10, Height=50, Width=100},
            };

            var ra = new RoutingAssistent();
            ra.InitalizeBoundSearchStructures(rectangles);
            var r = ra.FindHorizontalRoutingSlotBottom(rectangles[0].Bottom + 1,
                rectangles[0].X, rectangles[3].X);
            Assert.IsTrue(r <= 70 && r > 60);
        }

        [Test]
        public void TestFindRoutingSlotBottom2()
        {
            List<Rect> rectangles = new List<Rect>()
            {
                new Rect(){X=10,Y=10, Height=150, Width=100},
                new Rect(){X=30,Y=200, Height=50, Width=100},
                new Rect(){X=50,Y=70, Height=50, Width=100},
                new Rect(){X=200,Y=10, Height=50, Width=100},
            };

            var ra = new RoutingAssistent();
            ra.InitalizeBoundSearchStructures(rectangles);
            var r = ra.FindHorizontalRoutingSlotBottom(rectangles[2].Bottom + 1,
                rectangles[0].X, rectangles[3].X);
            Assert.IsTrue(r > 160 && r < 200);
        }
        //void CreateRectangles(int count, LinkedAvlTree<BoundsComparer> tree,
        //    List<Rect> rectangles)
        //{
        //    var rand = new Random(-127);
            

            
        //    for (int i = 0; i < count; i++)
        //    {
        //        var x = rand.NextDouble() * 2048;
        //        var y = rand.NextDouble() * 1600;
        //        var w = rand.NextDouble() + 150;
        //        var h = rand.NextDouble() * 50;
        //        var p = new Point(x, y);
        //        var s = new Size(w, h);
        //        var r = new Rect(p, s);
        //        tree.Add(r);
        //        if (tree.Count == i + 1)
        //        {
        //            rectangles.Add(r);
        //        }
                
        //    }

        //}
    }
}
