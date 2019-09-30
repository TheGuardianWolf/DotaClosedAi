﻿using Emgu.CV;
using Emgu.CV.Structure;
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
        Image<Bgra, byte> Image { get; }
        Point Cursor { get; }
    }
}
