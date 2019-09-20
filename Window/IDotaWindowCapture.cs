using DotaClosedAi.AiTask;
using System;
using System.Drawing;

namespace DotaClosedAi.Window
{
    interface IDotaWindowCapture : IAiTask
    {
        int FramesPerSecond { get; }
        Size WindowSize { get; }

        event EventHandler<DotaOcvWindowCaptureFrameCapturedEventArgs> FrameCaptured;

        Frame GetFrame();
    }
}