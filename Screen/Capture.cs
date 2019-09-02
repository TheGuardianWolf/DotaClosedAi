using System;
using System.Drawing;

namespace DotaClosedAi.Screen
{
    public class Capture : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Point Cursor { get; private set; }

        public Capture(Bitmap bitmap, Point cursor)
        {
            Bitmap = bitmap;
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
                Bitmap.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
