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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
            int numKey = 0;
            numKey = Convert.ToInt32(e.KeyData);

            if (numKey >= 96 && numKey <= 105)
                numKey = NumKeyDataConvert(e.KeyData);
            else
                numKey = e.KeyData;
            
            Border border = this.FindName("key" + numKey) as Border;
           // Console.WriteLine(e.KeyData);
            if (border != null)
            {
                border.Background = new SolidColorBrush(Colors.Transparent);
            }
            //throw new NotImplementedException();
        }

        private void KeyboardHook_OnKeyDownEvent(object sender, KeyDownEvent e)
        {
            int numKey = 0;
            numKey = Convert.ToInt32(e.KeyData);

            if (numKey >= 96 && numKey <= 105)
                numKey = NumKeyDataConvert(e.KeyData);
            else
                numKey = e.KeyData;

            Border border = this.FindName("key" + numKey) as Border;
            if (border!=null)
            {
                BrushConverter brush = new BrushConverter();
                border.Background = (Brush)brush.ConvertFromString("#7F808080");
            }
            //throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(txtPassword.Text=="88888888")
            {
              
                UnLockStateEvent?.Invoke(1, new EventArgs());

            }
            else
            {
                UnLockStateEvent?.Invoke(0, new EventArgs());
            }
        }

        /// <summary>
        /// 返回1成功，返回0失败
        /// </summary>
        public event EventHandler<EventArgs> UnLockStateEvent;

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            (this.Resources["keyBoardEnter"] as Storyboard).Begin();
            
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            
            (this.Resources["keyBoardLeave"] as Storyboard).Begin();
        }

        private int NumKeyDataConvert(int keyData)
        {
            return keyData - 48;
        }
    }
}
