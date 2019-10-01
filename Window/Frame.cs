using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace DotaClosedAi.Window
{
    class Frame : IDisposable, IFrame
    {
        public Image<Bgra, byte> Image { get; private set; }
        public Point Cursor { get; private set; }

        public Frame(Image<Bgra,byte> mat, Point cursor)
        {
            Image = mat.Clone();
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
