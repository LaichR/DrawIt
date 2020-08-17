using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Sketch.Controls
{
    public class GeometryBorder: Border
    {
        public static readonly DependencyProperty BorderGeometryProperty =
            DependencyProperty.Register("BorderGeometry", typeof(Geometry), typeof(GeometryBorder),
            new PropertyMetadata(OnBorderGeometryChanged));

        public static readonly DependencyProperty ShowShadowProperty =
            DependencyProperty.Register("ShowShadow", typeof(bool), typeof(GeometryBorder),
            new PropertyMetadata(OnShowShadowChanged));

        public GeometryBorder():base()
        {
            //BorderGeometry = new RectangleGeometry() { Rect = new Rect(0, 0, Width, Height)}; // provide a default
        }
        protected override void OnRender(DrawingContext dc)
        {
            //base.OnRender(dc);
            var path = PathGeometry.CreateFromGeometry(BorderGeometry);
            dc.DrawGeometry(this.Background, new Pen(BorderBrush, BorderThickness.Right),
                path);
            
        }

        public Effect BorderShadow
        {
            get
            {
                return new DropShadowEffect()
                {
                    BlurRadius = 4,
                    ShadowDepth = 4,
                    Opacity = 0.5,
                    Direction = 330,
                    Color = Colors.Gray
                };
            }
        }

        public Geometry BorderGeometry
        {
            get => (Geometry)GetValue(BorderGeometryProperty);
            set => SetValue(BorderGeometryProperty, value);
        }

        public bool ShowShadow
        {
            get => (bool)GetValue(ShowShadowProperty);
            set => SetValue(ShowShadowProperty, value);
        }



        private static void OnBorderGeometryChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if( source is GeometryBorder borderCtrl)
            {
                if( e.NewValue is Geometry geometry)
                {
                    if (geometry != null)
                    {
                        if (e.NewValue != e.OldValue)
                        {
                            borderCtrl.BorderGeometry = geometry;
                        }
                        if (geometry.Bounds != Rect.Empty)
                        {
                            borderCtrl.Width = borderCtrl.BorderGeometry.Bounds.Width;
                            borderCtrl.Height = borderCtrl.BorderGeometry.Bounds.Height;
                        }
                        borderCtrl.InvalidateVisual();
                    }  
                }
            }
        }

        private static void OnShowShadowChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            if (source is GeometryBorder borderCtrl)
            {
                if (e.NewValue is bool show)
                {
                    if (show)
                    {
                        borderCtrl.Effect = borderCtrl.BorderShadow;
                    }
                    else
                    {
                        borderCtrl.Effect = null;
                    }
                }
            }
        }

    }
}
