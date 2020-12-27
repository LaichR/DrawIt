using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;


namespace Sketch.Helper.UiUtilities
{
    public static class ToBitmapSource
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource Bitmap2BitmapSource(System.Drawing.Bitmap bitmap)
        {
            RuntimeCheck.Contract.Requires<ArgumentNullException>(bitmap != null, "bitmap must not be null" );
            lock (bitmap)
            {
                IntPtr hBitmap = bitmap.GetHbitmap();
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(
                                 hBitmap,
                                 IntPtr.Zero,
                                 Int32Rect.Empty,
                                 BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }

        }

        public static BitmapSource Icon2BitmapSource(System.Drawing.Icon icon)
        {
            RuntimeCheck.Contract.Requires<ArgumentNullException>(icon != null, "icon must not be null");
            lock (icon)
            {

                try
                {
                    return Imaging.CreateBitmapSourceFromHIcon(
                                 icon.Handle,
                                 Int32Rect.Empty,
                                 BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    
                }
            }

        }
    }
}
