﻿using DotaClosedAi.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using DotaClosedAi.AiTask;

namespace DotaClosedAi.Window
{
    class DotaWindowCapture : IDotaWindowCapture
    {
        private readonly int SECOND_MS = 1000;
        private WindowCapture _windowCapture;
        private IntPtr _windowHandle;
        private Task _captureTask;
        private bool _taskStopToken;
        private int _frameCounter;
        private Timer _frameCounterTimer;

        public int FramesPerSecond { get; private set; }

        public bool IsRunning { get; private set; }

        public Size WindowSize => _windowCapture.WindowSize;

        public event EventHandler<DotaOcvWindowCaptureFrameCapturedEventArgs> FrameCaptured;

        public DotaWindowCapture(IntPtr windowHandle)
        {
            IsRunning = false;
            _windowHandle = windowHandle;
            _windowCapture = null;
            _captureTask = null;
            _taskStopToken = false;
            _frameCounter = 0;
            _frameCounterTimer = new Timer(FrameCountLoop, null, Timeout.Infinite, SECOND_MS);
        }

        public Frame GetFrame() => _windowCapture.GetFrame();

        private void CaptureLoop()
        {
            while (!_taskStopToken)
            {
                _windowCapture.PerformCapture();
                Interlocked.Increment(ref _frameCounter);
                FrameCaptured?.Invoke(this, new DotaOcvWindowCaptureFrameCapturedEventArgs(_windowCapture));
            }
        }

        private void FrameCountLoop(object state)
        {
            if (!_taskStopToken)
            {
                var fps = Interlocked.Exchange(ref _frameCounter, 0);
                FramesPerSecond = fps;
            }
        }

        public bool Run()
        {
            if (!IsRunning)
            {
                _taskStopToken = false;
                _windowCapture = new WindowCapture(_windowHandle);
                _captureTask = Task.Factory.StartNew(CaptureLoop, TaskCreationOptions.LongRunning);
                _frameCounter = 0;
                _frameCounterTimer.Change(SECOND_MS, SECOND_MS);

                IsRunning = true;

                return true;
            }

            return false;
        }

        private bool Stop(bool wait)
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
                    _windowHandle = IntPtr.Zero;
                    IsRunning = false;
                }
                else
                {
                    Task.Run(() =>
                    {
                        Stop(true);
                    });
                }
                return true;
            }

            return false;
        }

        public bool Stop() => Stop(true);

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

    class DotaOcvWindowCaptureFrameCapturedEventArgs : EventArgs
    {
        private WindowCapture _windowCapture;
        public Point GetCursor() => _windowCapture.GetCursor();
        public Frame GetFrame() => _windowCapture.GetFrame();

        internal DotaOcvWindowCaptureFrameCapturedEventArgs(WindowCapture windowCapture)
        {
            _windowCapture = windowCapture;
        }
    }
}
