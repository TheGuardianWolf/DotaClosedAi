using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using DotaClosedAi.Win32;

namespace DotaClosedAi.Screen
{
    class WindowCapture : IDisposable
    {
        private RECT _rc;
        private Process _process;
        private IntPtr _windowDC;
        private IntPtr _compatDC;
        private IntPtr _compatBitmap;
        private POINT _cursor;
        private bool _hasFrame = false;

        public Size WindowSize => new Size(_rc.Width, _rc.Height);

        public WindowCapture(Process process)
        {
            _process = process;
            User.GetWindowRect(_process.MainWindowHandle, out _rc);
            Init();
        }

        private void Init()
        {
            _windowDC = User.GetWindowDC(_process.MainWindowHandle);
            _compatDC = Gdi.CreateCompatibleDC(_windowDC);
            _compatBitmap = Gdi.CreateCompatibleBitmap(_windowDC, _rc.Width, _rc.Height);
            IntPtr hOldBmp = Gdi.SelectObject(_compatDC, _compatBitmap);
            Gdi.DeleteObject(hOldBmp);
        }

        public void PerformCapture()
        {
            lock(this)
            {   
                _hasFrame = true;
                RECT rc;
                User.GetWindowRect(_process.MainWindowHandle, out rc);

                if (rc != _rc)
                {
                    Dispose(false);
                    _rc = rc;
                    Init();
                }

                //IntPtr hSrce = GetWindowDC(hwnd);
                //IntPtr hDest = CreateCompatibleDC(hSrce);
                //IntPtr hBmp = CreateCompatibleBitmap(hSrce, rc.Width, rc.Height);

                //PrintWindow(hwnd, hDest, 0);
                User.GetCursorPos(out _cursor);

                Gdi.SelectObject(_compatDC, _compatBitmap);
                Gdi.BitBlt(_compatDC, 0, 0, _rc.Width, _rc.Height, _windowDC, 0, 0, CopyPixelOperation.SourceCopy);

                //Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
                //Graphics gfxBmp = Graphics.FromImage(bmp);
                //IntPtr hdcBitmap = gfxBmp.GetHdc();

                //PrintWindow(hwnd, hdcBitmap, 0);
                //ReleaseDC(hwnd, hdcBitmap);

                //gfxBmp.Dispose();

                //return bmp;
            }
        }

        public Frame GetFrame()
        {
            Frame c = null;

            if (_hasFrame)
            {
                lock(this)
                {
                    c = new Frame(Bitmap.FromHbitmap(_compatBitmap), _cursor);
                }
            }

            return c;
        }

        public Point GetCursor()
        {
            Point p = new Point();

            if (_hasFrame)
            {
                lock(this)
                {
                    p = _cursor;
                }
            }

            return p;
        }

        public bool CopyFrame(IntPtr copy)
        {
            if (_hasFrame)
            {
                lock(this)
                {
                    return Gdi.BitBlt(copy, 0, 0, _rc.Width, _rc.Height, _compatDC, 0, 0, CopyPixelOperation.SourceCopy);
                }
            }

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    // free managed resources
            //}
            // free native resources if there are any.
            User.ReleaseDC(_process.MainWindowHandle, _windowDC);
            Gdi.DeleteDC(_compatDC);
            Gdi.DeleteObject(_compatBitmap);
        }
    }
}
