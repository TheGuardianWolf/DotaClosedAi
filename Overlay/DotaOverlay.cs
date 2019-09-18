using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Overlay
{
    public class DotaOverlay : IDisposable
    {
        private Process _dotaProcess;

        private GraphicsWindow _window;

        private Font _font;

        private SolidBrush _black;
        private SolidBrush _transparent;
        private SolidBrush _red;
        private SolidBrush _green;
        private SolidBrush _blue;

        public DotaOverlay(Process dotaProcess)
        {
            _dotaProcess = dotaProcess;

            // initialize a new Graphics object
            // GraphicsWindow will do the remaining initialization
            var graphics = new Graphics
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = true,
                VSync = true,
                WindowHandle = IntPtr.Zero
            };

            // it is important to set the window to visible (and topmost) if you want to see it!
            _window = new StickyWindow(_dotaProcess.MainWindowHandle, graphics)
            {
                IsTopmost = true,
                IsVisible = true,
                FPS = 60,
                X = 0,
                Y = 0
            };

            _window.SetupGraphics += _window_SetupGraphics;
            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
        }

        public bool Run()
        {
            if (_dotaProcess == null || _dotaProcess.HasExited)
            {
                return false;
            }

            // creates the window and setups the graphics
            _window.StartThread();

            return true;
        }

        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            // creates a simple font with no additional style
            _font = gfx.CreateFont("Arial", 16);

            // colors for brushes will be automatically normalized. 0.0f - 1.0f and 0.0f - 255.0f is accepted!
            
            _black = gfx.CreateSolidBrush(0, 0, 0);
            _transparent = gfx.CreateSolidBrush(Color.Transparent);

            _red = gfx.CreateSolidBrush(Color.Red); // those are the only pre defined Colors
            _green = gfx.CreateSolidBrush(Color.Green);
            _blue = gfx.CreateSolidBrush(Color.Blue);
        }

        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            // you do not need to call BeginScene() or EndScene()
            var gfx = e.Graphics;

            gfx.ClearScene(_transparent); // set the background of the scene (can be transparent)

            gfx.DrawTextWithBackground(_font, _red, _black, 10, 10, "FPS: " + gfx.FPS);

            gfx.DrawCircle(_red, 100, 100, 50, 2);
            gfx.DashedCircle(_green, 250, 100, 50, 2);

            // Rectangle.Create takes x, y, width, height instead of left top, right, bottom
            gfx.DrawRectangle(_blue, Rectangle.Create(350, 50, 100, 100), 2);
            gfx.DrawRoundedRectangle(_red, RoundedRectangle.Create(500, 50, 100, 100, 6), 2);

            gfx.DrawTriangle(_green, 650, 150, 750, 150, 700, 50, 2);

            gfx.DrawLine(_blue, 50, 175, 750, 175, 2);
            gfx.DashedLine(_red, 50, 200, 750, 200, 2);

            gfx.OutlineCircle(_black, _red, 100, 275, 50, 4);
            gfx.FillCircle(_green, 250, 275, 50);

            // parameters will always stay in this order: outline color, inner color, position, stroke
            gfx.OutlineRectangle(_black, _blue, Rectangle.Create(350, 225, 100, 100), 4);
            gfx.FillRoundedRectangle(_red, RoundedRectangle.Create(500, 225, 100, 100, 6));

            gfx.FillTriangle(_green, 650, 325, 750, 325, 700, 225);

        }

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            // you may want to dispose any brushes, fonts or images
            _black.Dispose();
            _red.Dispose();
            _green.Dispose();
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
                _window.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
