using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi
{
    class Program
    {
        static int captured = 0;

        static async Task OutputAsync()
        {
            while(true)
            {
                await Task.Delay(1000);
                Console.WriteLine($"{captured}");
                captured = 0;
            }
        }

        static void Main(string[] args)
        {
            var process = System.Diagnostics.Process.GetProcessesByName("dota2").FirstOrDefault();
            var sg = new ScreenGrabber();
            //sg.TakeScreenshot().Save("Full.png", ImageFormat.Png);

            var sgp = new ScreenGrabber(process);
            var bmp = sgp.TakeScreenshot();
            bmp.Save("Process.png", ImageFormat.Png);
            bmp.Dispose();

            //Task.Run(OutputAsync);

            //while (true)
            //{
            //    //sgp.TakeScreenshot();
            //    var bmp = sgp.TakeScreenshot();
            //    bmp.Dispose();
            //    captured += 1;
            //}
        }
    }
}
