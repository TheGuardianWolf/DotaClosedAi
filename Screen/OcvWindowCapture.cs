using DotaClosedAi.Win32;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Screen
{
    class OcvWindowCapture : IDisposable
    {
        private RECT _rc;
        private Process _process;
        private IntPtr _windowDC;
        private IntPtr _compatDC;
        private IntPtr _bitmap;
        private IntPtr _bitmapPixels;
        private POINT _cursor;
        private Mat _mat;
        private bool _hasFrame = false;

        public Size WindowSize => new Size(_rc.Width, _rc.Height);

        public OcvWindowCapture(Process process)
        {
            _process = process;
            User.GetWindowRect(_process.MainWindowHandle, out _rc);
            Init();
        }

        private void Init()
        {
            _windowDC = User.GetWindowDC(_process.MainWindowHandle); // hdcSys
            _compatDC = Gdi.CreateCompatibleDC(_windowDC); // hdcMem

            BITMAPINFO bi = new BITMAPINFO();
            bi.bmiHeader.Init();
            bi.bmiHeader.biWidth = _rc.Width;
            bi.bmiHeader.biHeight = -_rc.Height;
            bi.bmiHeader.biPlanes = 1;
            bi.bmiHeader.biBitCount = 32;

            _bitmap = Gdi.CreateDIBSection(_windowDC, ref bi, DIBColors.DIB_RGB_COLORS, out _bitmapPixels, IntPtr.Zero, 0);
            IntPtr hOldBmp = Gdi.SelectObject(_compatDC, _bitmap);
            Gdi.DeleteObject(hOldBmp);

            _mat = new Mat(_rc.Height, _rc.Width, MatType.CV_8UC4, _bitmapPixels, 0);
        }

        public void PerformCapture()
        {
            lock (this)
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

                User.GetCursorPos(out _cursor);

                Gdi.SelectObject(_compatDC, _bitmap);
                Gdi.BitBlt(_compatDC, 0, 0, _rc.Width, _rc.Height, _windowDC, 0, 0, System.Drawing.CopyPixelOperation.SourceCopy);
            }
        }

        public OcvFrame GetFrame()
        {
            OcvFrame c = null;

            if (_hasFrame)
            {
                lock (this)
                {
                    c = new OcvFrame(_mat, new Point(_cursor.X, _cursor.Y));
                }
            }

            return c;
        }

        public Point GetCursor()
        {
            Point p = new Point();

            if (_hasFrame)
            {
                lock (this)
                {
                    p = new Point(_cursor.X, _cursor.Y);
                }
            }

            return p;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _mat.Dispose();
            }
            // free native resources if there are any.
            User.ReleaseDC(_process.MainWindowHandle, _windowDC);
            Gdi.DeleteDC(_compatDC);
            Gdi.DeleteObject(_bitmap);
            _hasFrame = false;
        }
    }
}
