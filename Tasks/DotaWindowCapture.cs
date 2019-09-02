using DotaClosedAi.Screen;
using DotaClosedAi.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace DotaClosedAi.Tasks
{
    class DotaWindowCapture : ICaptureTask, IDisposable
    {
        private readonly string DOTA_PROCESS_NAME = "dota2";
        private readonly int SECOND_MS = 1000;
        private WindowCapture _windowCapture;
        private Process _dotaProcess;
        private Task _captureTask;
        private bool _taskStopToken;
        private int _frameCounter;
        private Timer _frameCounterTimer;

        public int FramesPerSecond { get; private set; }

        public Capture Capture => _windowCapture.GetCapture();

        public DotaWindowCapture()
        {
            _dotaProcess = Process.GetProcessesByName(DOTA_PROCESS_NAME).FirstOrDefault();
            _windowCapture = new WindowCapture(_dotaProcess);
            _captureTask = null;
            _taskStopToken = false;
            _frameCounter = 0;
            _frameCounterTimer = new Timer(FrameCountLoop, null, Timeout.Infinite, SECOND_MS);
        }

        private void CaptureLoop()
        {
            while(!_taskStopToken)
            {
                _windowCapture.PerformCapture();
                Interlocked.Increment(ref _frameCounter);
            }
        }

        private void FrameCountLoop(object state)
        {
            var fps = Interlocked.Exchange(ref _frameCounter, 0);
            FramesPerSecond = fps;
        }

        public void Run()
        {
            if (_captureTask == null)
            {
                _captureTask = Task.Factory.StartNew(CaptureLoop, TaskCreationOptions.LongRunning);
                _frameCounterTimer.Change(SECOND_MS, SECOND_MS);
            }
        }

        public void Stop()
        {
            if (_captureTask != null)
            {
                _taskStopToken = true;
                _frameCounterTimer.Change(Timeout.Infinite, SECOND_MS);
                _captureTask.Wait();
                _captureTask = null;
            }
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
                // free managed resources
                Stop();
                _frameCounterTimer.Dispose();
                _windowCapture.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
