using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Window
{
    interface IFrame
    {
        Mat Image { get; }
        Point Cursor { get; }
    }
}
