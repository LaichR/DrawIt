using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using System.Drawing;

namespace UI.Utilities.Controls
{
    public class BitmapImage : System.Windows.Controls.Image
    {
        public static readonly DependencyProperty ImageBitmapProperty =
            DependencyProperty.Register("ImageBitmap", typeof(System.Drawing.Bitmap), typeof(BitmapImage),
            new PropertyMetadata(OnBitmapPropertyChanged));

        System.Drawing.Bitmap _bitmap;

        public BitmapImage():base(){}   

        public Bitmap ImageBitmap
        {
            get
            {
                return base.GetValue(ImageBitmapProperty) as Bitmap;
                
            }
            set
            {
                base.SetValue(ImageBitmapProperty, value);
            }
        }

        private static void OnBitmapPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            BitmapImage image = source as BitmapImage;
            image._bitmap = e.NewValue as System.Drawing.Bitmap;
            if (image._bitmap != null)
                image.Source = ToBitmapSource.Bitmap2BitmapSource(image._bitmap);
        }
    }
    
}
