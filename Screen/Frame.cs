using System;
using System.Drawing;

namespace DotaClosedAi.Screen
{
    public class Frame : IDisposable
    {
        public Bitmap Image { get; private set; }
        public Point Cursor { get; private set; }

        public Frame(Bitmap bitmap, Point cursor)
        {
            Image = bitmap;
            Cursor = cursor;
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
                Image.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
