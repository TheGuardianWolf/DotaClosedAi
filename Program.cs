using DotaClosedAi.Window;
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
        static bool exitToken = false;
        static DotaWindowCapture dotaWindowCapture;
        static Process process;

        static void Main(string[] args)
        {
            process = Process.GetProcessesByName("dota2").FirstOrDefault();

            if (process != null)
            {
                process.Exited += Process_Exited;

                dotaWindowCapture = new DotaWindowCapture(process.MainWindowHandle);
                dotaWindowCapture.FrameCaptured += DotaWindowCapture_FrameCaptured;
                dotaWindowCapture.Run();

                while (!exitToken)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine($"{dotaWindowCapture.FramesPerSecond}");
                }
            }            
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            dotaWindowCapture.Stop();
            dotaWindowCapture.Dispose();

            process.Exited -= Process_Exited;

            exitToken = true;
        }

        private static void DotaWindowCapture_FrameCaptured(object sender, DotaOcvWindowCaptureFrameCapturedEventArgs e)
        {
            var frame = e.GetFrame();
            frame.Dispose();
        }
    }
}
