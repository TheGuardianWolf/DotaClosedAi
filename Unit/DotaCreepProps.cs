using DotaClosedAi.Overlay;
using DotaClosedAi.Window;
using Emgu.CV;
using Emgu.CV.Cuda;
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

        private static readonly Image<Gray, byte> _creepHpTemplate = new Image<Gray, byte>(Properties.Resources.CreepHpTemplate);

        private static Mat _denoiseKernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));

        private static void Denoise(IInputArray src, IOutputArray dst)
        {
            CvInvoke.MorphologyEx(src, dst, MorphOp.Open, _denoiseKernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            CvInvoke.MorphologyEx(dst, dst, MorphOp.Close, _denoiseKernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
        }

        private Image<Hsv, byte> _hsv = new Image<Hsv, byte>(0, 0);
        private Image<Gray, byte> _yellowEmpty = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _yellowFill = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _yellow = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _redEmpty = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _redFill = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _red = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _redAndYellow = new Image<Gray, byte>(0, 0);
        private Image<Gray, float> _matchResults = new Image<Gray, float>(0, 0);
        private Dictionary<int, Tuple<Point, int>> _matchDuplicates = new Dictionary<int, Tuple<Point, int>>();
        private List<Point> _matches = new List<Point>();
        private double _templateMatchingThreshold = 0.95;

        public DotaCreepProps(float templateMatchingThreshold)
        {
            _templateMatchingThreshold = templateMatchingThreshold;
        }

        public bool DetectInFrame(IFrame frame)
        {
            _matchDuplicates.Clear();

            var mat = frame.Image;
            CvInvoke.CvtColor(mat, _hsv, ColorConversion.Bgra2Bgr);
            CvInvoke.CvtColor(_hsv, _hsv, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(_hsv, _yellowEmptyThreshold[0], _yellowEmptyThreshold[1], _yellowEmpty);
            Denoise(_yellowEmpty, _yellowEmpty);
            CvInvoke.InRange(_hsv, _yellowFillThreshold[0], _yellowFillThreshold[1], _yellowFill);
            Denoise(_yellowFill, _yellowFill);
            CvInvoke.InRange(_hsv, _redEmptyThreshold[0], _redEmptyThreshold[1], _redEmpty);
            Denoise(_redEmpty, _redEmpty);
            CvInvoke.InRange(_hsv, _redFillThreshold[0], _redFillThreshold[1], _redFill);
            Denoise(_redFill, _redEmpty);
            CvInvoke.BitwiseOr(_yellowEmpty, _yellowFill, _yellow);
            CvInvoke.BitwiseOr(_redEmpty, _redFill, _red);
            CvInvoke.BitwiseOr(_red, _yellow, _redAndYellow);
            CvInvoke.MatchTemplate(_redAndYellow, _creepHpTemplate, _matchResults, TemplateMatchingType.CcoeffNormed);

            var w = _creepHpTemplate.Width;
            var h = _creepHpTemplate.Height;
            using (var matchesMat = new Image<Gray, int>(mat.Size))
            {
                matchesMat.SetZero();
                int matchCount = 0;
                for (int row = 0; row < _matchResults.Height; row++)
                {
                    for (int col = 0; col < _matchResults.Width; col++)
                    {
                        if (_matchResults.Data[row, col, 0] > _templateMatchingThreshold)
                        {
                            matchCount += 1;
                            var check = matchesMat.Data[row + h / 2, col + w / 2, 0];
                            if (check == 0)
                            {
                                using (var subDetection = matchesMat.GetSubRect(new Rectangle(col, row, w, h)))
                                {
                                    subDetection.SetValue(matchCount);
                                }
                                _matchDuplicates.Add(matchCount, new Tuple<Point, int>(new Point(col, row), 1));
                            }
                            else
                            {
                                _matchDuplicates[check] = new Tuple<Point, int>
                                    (
                                        new Point(_matchDuplicates[check].Item1.X + col, _matchDuplicates[check].Item1.Y + row),
                                        _matchDuplicates[check].Item2 + 1
                                    );
                            }
                        }
                    }
                }

                lock(_matches)
                {
                    _matches.Clear();
                    foreach (var match in _matchDuplicates)
                    {
                        _matches.Add(new Point(match.Value.Item1.X / match.Value.Item2, match.Value.Item1.Y / match.Value.Item2));
                    }
                }
            }

            return true;
        }

        public bool DrawOverlay(DotaOverlay overlay)
        {
            lock(_matches)
            {

            }
            return false;
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
            }
            // free native resources if there are any.
        }
    }
}
