using DotaClosedAi.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotaClosedAi.Input
{
    class Keyboard
    {
        public event EventHandler<Keys> OnKeyDown;
        public event EventHandler<Keys> OnKeyUp;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public Keyboard()
        {
            _proc = HookCallback;
        }

        public void HookKeyboard()
        {
            _hookID = SetHook(_proc);
        }

        public void UnhookKeyboard()
        {
            User.UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return User.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, proc, Kernel.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, WindowsMessages wParam, KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0 && wParam == WindowsMessages.WM_KEYDOWN || wParam == WindowsMessages.WM_SYSKEYDOWN)
            {
                OnKeyDown.Invoke(this, ((Keys)lParam.vkCode));
            }
            else if (nCode >= 0 && wParam == WindowsMessages.WM_KEYUP || wParam == WindowsMessages.WM_SYSKEYUP)
            {
                OnKeyUp.Invoke(this, ((Keys)lParam.vkCode));
            }

            return User.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
