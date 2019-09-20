using DotaClosedAi.Overlay;
using DotaClosedAi.Window;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Unit
{
    class DotaCreepProps : IDisposable
    {
        private static readonly ScalarArray[] _blackThreshold = new ScalarArray[2] 
        {
            new ScalarArray(new MCvScalar(0, 0, 0)), new ScalarArray(new MCvScalar(5, 208, 17))
        };
        private static readonly ScalarArray[] _yellowEmptyThreshold = new ScalarArray[2] 
        {
            new ScalarArray(new MCvScalar(27, 255, 17 )), new ScalarArray(new MCvScalar(34, 255, 29))
        };
        private static readonly ScalarArray[] _yellowFillThreshold = new ScalarArray[2]
        {
            new ScalarArray(new MCvScalar(27, 255, 84 )), new ScalarArray(new MCvScalar(33, 255, 129))
        };
        private static readonly ScalarArray[] _redEmptyThreshold = new ScalarArray[2] 
        {
            new ScalarArray(new MCvScalar(5, 148, 90)), new ScalarArray(new MCvScalar(9, 159, 140))
        };
        private static readonly ScalarArray[] _redFillThreshold = new ScalarArray[2] 
        {
            new ScalarArray(new MCvScalar(3, 191, 18)), new ScalarArray(new MCvScalar(9, 211, 33))
        };

        private static readonly Mat _creepHpTemplate = (new Image<Gray, byte>(Properties.Resources.CreepHpTemplate)).Mat;

        private static Mat _denoiseKernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));

        private Mat _hsv = new Mat();
        private Mat _yellowEmpty = new Mat();
        private Mat _yellowFill = new Mat();
        private Mat _yellow = new Mat();
        private Mat _redEmpty = new Mat();
        private Mat _redFill = new Mat();
        private Mat _red = new Mat();
        private Mat _redAndYellow = new Mat();
        private Mat _matchResults = new Mat();

        private static void Denoise(Mat src, Mat dst)
        {
            CvInvoke.MorphologyEx(src, dst, MorphOp.Open, _denoiseKernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            CvInvoke.MorphologyEx(dst, dst, MorphOp.Close, _denoiseKernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
        }

        public DotaCreepProps()
        {

        }

        public bool DetectInFrame(IFrame frame)
        {
            var mat = frame.Image;
            CvInvoke.CvtColor(mat, _hsv, ColorConversion.Bgr2Hsv);

            CvInvoke.InRange(_hsv, _yellowEmptyThreshold[0], _yellowEmptyThreshold[1], _yellowEmpty);
            CvInvoke.InRange(_hsv, _yellowFillThreshold[0], _yellowFillThreshold[1], _yellowFill);
            CvInvoke.InRange(_hsv, _redEmptyThreshold[0], _redEmptyThreshold[1], _redEmpty);
            CvInvoke.InRange(_hsv, _redFillThreshold[0], _redFillThreshold[1], _redFill);
            CvInvoke.BitwiseOr(_yellowEmpty, _yellowFill, _yellow);
            CvInvoke.BitwiseOr(_redEmpty, _redFill, _red);
            CvInvoke.BitwiseOr(_red, _yellow, _redAndYellow);
            CvInvoke.MatchTemplate(_redAndYellow, _creepHpTemplate, _matchResults, TemplateMatchingType.CcoeffNormed);
            CvInvoke.Threshold(_matchResults, _matchResults, 0.95, 1.0, ThresholdType.ToZero);
            return false;
        }

        public bool DrawOverlay(DotaOverlay overlay)
        {
            return false;
        }
    }
}
