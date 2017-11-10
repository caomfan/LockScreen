using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LockScreen.UserControls
{
    /// <summary>
    /// ucKeyboard.xaml 的交互逻辑
    /// </summary>
    public partial class ucKeyboard : UserControl
    {
        public ucKeyboard()
        {
            InitializeComponent();
            KeyboardHook keyboardHook = new KeyboardHook();
            keyboardHook.SetHook();
            keyboardHook.OnKeyDownEvent += KeyboardHook_OnKeyDownEvent;
            keyboardHook.OnKeyUpEvent += KeyboardHook_OnKeyUpEvent;
            this.Unloaded += (s, e) =>
            {
                keyboardHook.UnHook();
                keyboardHook.OnKeyDownEvent -= KeyboardHook_OnKeyDownEvent;
                keyboardHook.OnKeyUpEvent -= KeyboardHook_OnKeyUpEvent;
            };
        }

        private void KeyboardHook_OnKeyUpEvent(object sender, OnKeyUpEvent e)
        {
            
            //throw new NotImplementedException();
        }

        private void KeyboardHook_OnKeyDownEvent(object sender, KeyDownEvent e)
        {
            //throw new NotImplementedException();
        }
    }
}
