﻿using DotaClosedAi.Screen;
using DotaClosedAi.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace DotaClosedAi.Vision
{
    class DotaWindowCapture
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

        public bool IsRunning { get; private set; }

        public Frame Frame => _windowCapture.GetFrame();

        public Size WindowSize => _windowCapture.WindowSize;

        public event EventHandler<DotaWindowCaptureFrameCapturedEventArgs> FrameCaptured;

        public DotaWindowCapture()
        {
            IsRunning = false;
            _dotaProcess = null;
            _windowCapture = null;
            _captureTask = null;
            _taskStopToken = false;
            _frameCounter = 0;
            _frameCounterTimer = new Timer(FrameCountLoop, null, Timeout.Infinite, SECOND_MS);
        }

        private void CaptureLoop()
        {
            while(!_taskStopToken)
            {
                if (!_dotaProcess.HasExited)
                {
                    _windowCapture.PerformCapture();
                    Interlocked.Increment(ref _frameCounter);
                    FrameCaptured?.Invoke(this, new DotaWindowCaptureFrameCapturedEventArgs(_windowCapture));
                }
                else
                {
                    Stop(false);
                }
            }
        }

        private void FrameCountLoop(object state)
        {
            if (!_taskStopToken && !_dotaProcess.HasExited)
            {
                var fps = Interlocked.Exchange(ref _frameCounter, 0);
                FramesPerSecond = fps;
            }
        }

        public bool Run()
        {
            if (!IsRunning)
            {
                _dotaProcess = Process.GetProcessesByName(DOTA_PROCESS_NAME).FirstOrDefault();

                if (_dotaProcess == null)
                {
                    return false;
                }

                _taskStopToken = false;
                _windowCapture = new WindowCapture(_dotaProcess);
                _captureTask = Task.Factory.StartNew(CaptureLoop, TaskCreationOptions.LongRunning);
                _frameCounter = 0;
                _frameCounterTimer.Change(SECOND_MS, SECOND_MS);

                IsRunning = true;

                return true;
            }

            return false;
        }

        private void Stop(bool wait)
        {
            if (IsRunning && !_taskStopToken)
            {
                _taskStopToken = true;
                if (wait)
                {
                    _frameCounterTimer.Change(Timeout.Infinite, SECOND_MS);
                    _captureTask.Wait();
                    _captureTask = null;
                    _windowCapture.Dispose();
                    _windowCapture = null;
                    _dotaProcess = null;
                    IsRunning = false;
                }
                else
                {
                    Task.Run(() =>
                    {
                        Stop(true);
                    });
                }
            }
        }

        public void Stop() => Stop(true);

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
                Stop(true);
                _frameCounterTimer.Dispose();
            }
            // free native resources if there are any.
        }
    }

    class DotaWindowCaptureFrameCapturedEventArgs : EventArgs
    {
        private WindowCapture _windowCapture;
        public Point Cursor => _windowCapture.GetCursor();
        public Frame Frame => _windowCapture.GetFrame();

        internal DotaWindowCaptureFrameCapturedEventArgs(WindowCapture windowCapture)
        {
            _windowCapture = windowCapture;
        }

        public void CopyFrame(IntPtr destination) => _windowCapture.CopyFrame(destination);
    }
}
