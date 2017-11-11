using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LockScreen.UserControls
{
    public class KeyboardHook
    {
        int hHook;

        Win32Api.HookProc KeyboardHookDelegate;

        Win32Api.LASTINPUTINFO lastInPut = new Win32Api.LASTINPUTINFO();

        /// <summary>
        /// 安装键盘钩子
        /// </summary>
        public void SetHook()
        {

            KeyboardHookDelegate = new Win32Api.HookProc(KeyboardHookProc);

            ProcessModule cModule = Process.GetCurrentProcess().MainModule;

            var mh = Win32Api.GetModuleHandle(cModule.ModuleName);

            hHook = Win32Api.SetWindowsHookEx(Win32Api.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);

        }

        /// <summary>
        /// 卸载键盘钩子
        /// </summary>
        public void UnHook()
        {

            Win32Api.UnhookWindowsHookEx(hHook);

        }

        /// <summary>
        /// 获取键盘消息
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 如果该消息被丢弃（nCode<0
            if (nCode >= 0)
            {

                Win32Api.KeyboardHookStruct KeyDataFromHook = (Win32Api.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.KeyboardHookStruct));

                int keyData = KeyDataFromHook.vkCode;

                //WM_KEYDOWN和WM_SYSKEYDOWN消息，将会引发OnKeyDownEvent事件
                if (OnKeyDownEvent != null && (wParam == Win32Api.WM_KEYDOWN || wParam == Win32Api.WM_SYSKEYDOWN))
                {
                    // 此处触发键盘按下事件
                    // keyData为按下键盘的值,对应 虚拟码
                    OnKeyDownEvent(this, new KeyDownEvent() { KeyData = keyData });

                }

                //WM_KEYUP和WM_SYSKEYUP消息，将引发OnKeyUpEvent事件 

                if (OnKeyUpEvent != null && (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP))
                {
                    // 此处触发键盘抬起事件
                    OnKeyUpEvent(this, new OnKeyUpEvent() { KeyData = keyData });
                }
            }

            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);

        }


        /// <summary>
        /// 无操作多少秒后返回True，有操作则返回false
        /// </summary>
        /// <param name="milli">毫秒</param>
        /// <returns></returns>
        public bool NoOpera(long milli)
        {
            //配置时间内无操作锁定
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            Win32Api.GetLastInputInfo(ref lastInPut);

            long noOperationTime = System.Environment.TickCount - lastInPut.dwTime;

            if (noOperationTime > milli)
            {
                //退出程序
                return true;
            }
            return false;
        }

        /// <summary>
        /// 键按下事件
        /// </summary>
        public event EventHandler<KeyDownEvent> OnKeyDownEvent;
        /// <summary>
        /// 键释放事件
        /// </summary>
        public event EventHandler<OnKeyUpEvent> OnKeyUpEvent;
    }

    public class OnKeyUpEvent:EventArgs
    {
        public int KeyData { get; set; }
    }

    public class KeyDownEvent:EventArgs
    {
        public int KeyData { get; set; }
    }
}
