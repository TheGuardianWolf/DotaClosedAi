using DotaClosedAi.Screen;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Unit
{
    class DotaCreepProps
    {
        private readonly int[][] _black = new int[2][] { new int[3] { 0, 0, 0 }, new int[3] { 5, 208, 17 } };
        private readonly int[][] _yellowEmpty = new int[2][] { new int[3] { 27, 255, 17 }, new int[3] { 34, 255, 29 } };
        private readonly int[][] _yellowFill = new int[2][] { new int[3] { 27, 255, 84 }, new int[3] { 33, 255, 129 } };
        private readonly int[][] _redEmpty = new int[2][] { new int[3] { 5, 148, 90 }, new int[3] { 9, 159, 140 } };
        private readonly int[][] _redFill = new int[2][] { new int[3] { 3, 191, 18 }, new int[3] { 9, 211, 33 } };

        public bool TranslateVisuals(IFrame<Mat> frame)
        {

        }
    }
}
