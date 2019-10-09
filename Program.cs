using DotaClosedAi.Unit;
using DotaClosedAi.Window;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            var img = new Image<Bgra, byte>(@"C:\Users\lichk\Documents\Git\dota2\2019-09-02.png");
            var frame = new Frame(img, new Point(0, 0));
            var props = new DotaCreepProps(0.95f);
            props.ProcessFrame(frame);
            while (true) ;
            //process = Process.GetProcessesByName("dota2").FirstOrDefault();

            //if (process != null)
            //{
            //    process.Exited += Process_Exited;

            //    dotaWindowCapture = new DotaWindowCapture(process.MainWindowHandle);
            //    dotaWindowCapture.FrameCaptured += DotaWindowCapture_FrameCaptured;
            //    dotaWindowCapture.Run();

            //    while (!exitToken)
            //    {
            //        Thread.Sleep(1000);
            //        Console.WriteLine($"{dotaWindowCapture.FramesPerSecond}");
            //    }
            //}            
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            dotaWindowCapture.Stop();
            dotaWindowCapture.Dispose();

            process.Exited -= Process_Exited;

            exitToken = true;
        }

        static bool once = false;

        private static void DotaWindowCapture_FrameCaptured(object sender, DotaOcvWindowCaptureFrameCapturedEventArgs e)
        {
            var frame = e.GetFrame();
            if (!once)
            {
                once = true;
                frame.Image.Save("test.png");
            }
            frame.Dispose();
        }
    }
}
