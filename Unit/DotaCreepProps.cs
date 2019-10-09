using DotaClosedAi.Overlay;
using DotaClosedAi.Window;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Unit
{
    class DotaCreepProps : IDisposable
    {
        private static readonly ScalarArray[] _yellowEmptyThreshold = new ScalarArray[2]
        {
            new ScalarArray(new MCvScalar(27, 255, 17)), new ScalarArray(new MCvScalar(34, 255, 29))
        };
        private static readonly ScalarArray[] _yellowFillThreshold = new ScalarArray[2]
        {
            new ScalarArray(new MCvScalar(27, 255, 84)), new ScalarArray(new MCvScalar(33, 255, 129))
        };
        private static readonly ScalarArray[] _redEmptyThreshold = new ScalarArray[2]
        {
            new ScalarArray(new MCvScalar(3, 191, 18)), new ScalarArray(new MCvScalar(9, 211, 33))
        };
        private static readonly ScalarArray[] _redFillThreshold = new ScalarArray[2]
        {
            new ScalarArray(new MCvScalar(5, 148, 90)), new ScalarArray(new MCvScalar(9, 159, 140))
        };

        private static readonly Image<Gray, byte> _creepHpTemplate = new Image<Gray, byte>(Properties.Resources.CreepHpTemplate);

        private Size _frameSize = new Size(0, 0);
        private Image<Hsv, byte> _hsv = new Image<Hsv, byte>(0, 0);
        private Image<Gray, byte> _yellowEmpty = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _yellowFill = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _yellow = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _redEmpty = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _redFill = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _red = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _redAndYellow = new Image<Gray, byte>(0, 0);
        private Image<Gray, float> _matchResults = new Image<Gray, float>(0, 0);
        private Image<Gray, byte> _fill = new Image<Gray, byte>(0, 0);
        private Image<Gray, byte> _empty = new Image<Gray, byte>(0, 0);
        private Dictionary<int, (Point, int)> _matchDuplicates = new Dictionary<int, (Point, int)>();
        private List<Point> _matches = new List<Point>();
        private List<(Rectangle, Creep)> _pvtCreepProps = new List<(Rectangle, Creep)>();
        private List<(Rectangle, Creep)> _creepProps = new List<(Rectangle, Creep)>();
        private double _templateMatchingThreshold = 0.95;

        public DotaCreepProps(float templateMatchingThreshold)
        {
            _templateMatchingThreshold = templateMatchingThreshold;
        }

        private bool Init(Size frameSize)
        {
            if (frameSize != _frameSize)
            {
                _frameSize = frameSize;
                Dispose(true);
                _hsv = new Image<Hsv, byte>(frameSize);
                _yellowEmpty = new Image<Gray, byte>(frameSize);
                _yellowFill = new Image<Gray, byte>(frameSize);
                _yellow = new Image<Gray, byte>(frameSize);
                _redEmpty = new Image<Gray, byte>(frameSize);
                _redFill = new Image<Gray, byte>(frameSize);
                _red = new Image<Gray, byte>(frameSize);
                _redAndYellow = new Image<Gray, byte>(frameSize);
                _matchResults = new Image<Gray, float>(frameSize.Width - _creepHpTemplate.Width + 1, frameSize.Height - _creepHpTemplate.Height + 1);
                _fill = new Image<Gray, byte>(frameSize);
                _empty = new Image<Gray, byte>(frameSize);
            }
            
            return true;
        }

        public bool ProcessFrame(IFrame frame)
        {
            _matchDuplicates.Clear();
            _matches.Clear();
            _pvtCreepProps.Clear();

            var mat = frame.Image;
            Init(mat.Size);
            CvInvoke.CvtColor(mat, _hsv, ColorConversion.Bgra2Bgr);
            CvInvoke.CvtColor(_hsv, _hsv, ColorConversion.Bgr2Hsv);
            CvInvoke.InRange(_hsv, _yellowEmptyThreshold[0], _yellowEmptyThreshold[1], _yellowEmpty);
            CvInvoke.InRange(_hsv, _yellowFillThreshold[0], _yellowFillThreshold[1], _yellowFill);
            CvInvoke.InRange(_hsv, _redEmptyThreshold[0], _redEmptyThreshold[1], _redEmpty);
            CvInvoke.InRange(_hsv, _redFillThreshold[0], _redFillThreshold[1], _redFill);
            CvInvoke.BitwiseOr(_yellowEmpty, _yellowFill, _yellow);
            CvInvoke.BitwiseOr(_redEmpty, _redFill, _red);
            CvInvoke.BitwiseOr(_red, _yellow, _redAndYellow);
            CvInvoke.BitwiseOr(_redFill, _yellowFill, _fill);
            CvInvoke.BitwiseOr(_redEmpty, _yellowEmpty, _empty);

            CvInvoke.MatchTemplate(_redAndYellow, _creepHpTemplate, _matchResults, TemplateMatchingType.CcorrNormed);

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
                            var check = matchesMat.Data[row + h / 2, col + w / 2, 0];
                            if (check == 0)
                            {
                                matchCount += 1;
                                using (var subDetection = matchesMat.GetSubRect(new Rectangle(col, row, w, h)))
                                {
                                    subDetection.SetValue(matchCount);
                                }
                                _matchDuplicates.Add(matchCount, (new Point(col, row), 1));
                            }
                            else
                            {
                                _matchDuplicates[check] =
                                    (
                                        new Point(_matchDuplicates[check].Item1.X + col, _matchDuplicates[check].Item1.Y + row),
                                        _matchDuplicates[check].Item2 + 1
                                    );
                            }
                        }
                    }
                }
            }

            foreach (var duplicate in _matchDuplicates)
            {
                var match = new Point(duplicate.Value.Item1.X / duplicate.Value.Item2, duplicate.Value.Item1.Y / duplicate.Value.Item2);
                var pnt = new Point(match.X + 2, match.Y);
                var sz = new Size(w - 4, h);
                var bar = new Rectangle(pnt, sz);
                var barFill = _fill.GetSubRect(bar);
                var barEmpty = _empty.GetSubRect(bar);
                
                var fill = barFill.CountNonzero()[0];
                var total = fill + barEmpty.CountNonzero()[0];

                var health = (double)fill / total;
                _creepProps.Add((bar, new Creep(health)));
            }

            if (_pvtCreepProps.Count > 0)
            {
                lock (_creepProps)
                {
                    _creepProps.Clear();
                    _creepProps.AddRange(_pvtCreepProps);
                }
            }

            return true;
        }

        public bool DrawOverlay(DotaOverlay overlay)
        {
            lock (_creepProps)
            {
                
            }
            return true;
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
                _hsv.Dispose();
                _yellowEmpty.Dispose();
                _yellowFill.Dispose();
                _yellow.Dispose();
                _redEmpty.Dispose();
                _red.Dispose();
                _redAndYellow.Dispose();
                _matchResults.Dispose();
                _fill.Dispose();
                _empty.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
