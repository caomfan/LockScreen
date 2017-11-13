using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LockScreen.UserControls
{
    public class KeyboardHook
    {
        int hHook;

        Win32Api.HookProc KeyboardHookDelegate;

        private static KeyboardHook keyboardHook;
        public static KeyboardHook GetInstance()
        {
            if(keyboardHook==null)
            {
                keyboardHook = new KeyboardHook();
            }
            return keyboardHook;

        }

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
            if (hHook != 0)
            {
                Win32Api.UnhookWindowsHookEx(hHook);
                hHook = 0;
            }
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
                if (KeyDataFromHook.vkCode == 91)//截获左边WIN键
                {
                    return 1;
                }
                if (KeyDataFromHook.vkCode == 92)//截获右边WIN键
                {
                    return 1;
                }
                if (KeyDataFromHook.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control)//截获Ctrl+ESC键
                {
                    return 1;
                }
                if (KeyDataFromHook.vkCode == (int)Keys.F4 && (int)Control.ModifierKeys == (int)Keys.Alt)//截获ALT+F4
                {
                    return 1;
                }
                if (KeyDataFromHook.vkCode == (int)Keys.Tab && (int)Control.ModifierKeys == (int)Keys.Alt)//截获ALT+TAB
                {
                    return 1;
                }
            }

            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);

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
