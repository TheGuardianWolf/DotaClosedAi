using DotaClosedAi.Screen;
using DotaClosedAi.Tasks;
using DotaClosedAi.Vision;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotaClosedAi
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = Process.GetProcessesByName("dota2").FirstOrDefault();

            var dotaWindowCapture = new DotaOcvWindowCapture();
            dotaWindowCapture.FrameCaptured += DotaWindowCapture_FrameCaptured;
            dotaWindowCapture.Run();

            while(true)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"{dotaWindowCapture.FramesPerSecond}");
            }
        }

        private static void DotaWindowCapture_FrameCaptured(object sender, DotaOcvWindowCaptureFrameCapturedEventArgs e)
        {
            var frame = e.Frame;
            frame.Dispose();
        }
    }
}
