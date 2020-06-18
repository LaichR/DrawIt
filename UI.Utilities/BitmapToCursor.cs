using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace UI.Utilities
{
    public static class BitmapToCursor
    {
        struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("user32.dll")]
        static extern IntPtr CreateIconIndirect(ref IconInfo icon);

       

        /// <summary>
        /// Create a 32x32 cursor from a bitmap, with the hot spot in the middle
        /// </summary>
        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IconInfo tmp = new IconInfo();
            GetIconInfo(bmp.GetHicon(), ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;

            IntPtr ptr = CreateIconIndirect(ref tmp);
    
            SafeFileHandle handle = new SafeFileHandle(ptr, false);
            return CursorInteropHelper.Create(handle);
            
        }

    }
}
