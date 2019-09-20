using System.Drawing;

namespace DotaClosedAi.Window
{
    interface IWindowCapture
    {
        Size WindowSize { get; }

        Point GetCursor();
        Frame GetFrame();
        void PerformCapture();
    }
}