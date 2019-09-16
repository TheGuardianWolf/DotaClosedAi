using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Screen
{
    class OcvFrame : IDisposable
    {
        public Mat Mat { get; private set; }
        public Point Cursor { get; private set; }

        public OcvFrame(Mat mat, Point cursor)
        {
            Mat temp = new Mat(mat.Height, mat.Width, mat.Type());
            mat.CopyTo(temp);
            Mat = temp;
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
                Mat.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
