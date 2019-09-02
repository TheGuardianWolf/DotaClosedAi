using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace DotaClosedAi.Screen
{
    //static class ScreenCapture
    //{
    //    public static Bitmap Capture(Size sz)
    //    {
    //        IntPtr hDesk = GetDesktopWindow();
    //        IntPtr hSrce = GetWindowDC(hDesk);
    //        IntPtr hDest = CreateCompatibleDC(hSrce);
    //        IntPtr hBmp = CreateCompatibleBitmap(hSrce, sz.Width, sz.Height);
    //        IntPtr hOldBmp = SelectObject(hDest, hBmp);
    //        bool b = BitBlt(hDest, 0, 0, sz.Width, sz.Height, hSrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
    //        Bitmap bmp = Bitmap.FromHbitmap(hBmp);
    //        SelectObject(hDest, hOldBmp);
    //        DeleteObject(hBmp);
    //        DeleteDC(hDest);
    //        ReleaseDC(hDesk, hSrce);
    //        return bmp;
    //    }
    //}
}
