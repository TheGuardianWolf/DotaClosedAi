using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;

namespace DotaClosedAi.Screen
{
    class FrameOcv : IDisposable, IFrame
    {
        public Mat Image { get; private set; }
        public Point Cursor { get; private set; }

        public FrameOcv(Mat mat, Point cursor)
        {
            Mat temp = new Mat(mat.Size, mat.Depth, mat.NumberOfChannels);
            mat.CopyTo(temp);
            Image = temp;
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
