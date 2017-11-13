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
            this.DataContext = MainWindow.VM;
        }

        /// <summary>
        /// 改变虚拟键盘按键状态
        /// </summary>
        /// <param name="keyData">key值 例如：65</param>
        /// <param name="keyState">按键状态 1为弹起，0为按下</param>
        public void ChangeKeyState(int keyData,int keyState)
        { 

            if (keyData >= 96 && keyData <= 105)
                keyData = NumKeyDataConvert(keyData);

            Border border = this.FindName("key" + keyData) as Border;
            // Console.WriteLine(e.KeyData);
            if (border != null)
            {
                if(keyState==1)
                border.Background = new SolidColorBrush(Colors.Transparent);
                else if(keyState==0)
                {
                    BrushConverter brush = new BrushConverter();
                    border.Background = (Brush)brush.ConvertFromString("#7F808080");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(txtPassword.Text==ConfigManager.GetNumPass())
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

        private  int NumKeyDataConvert(int keyData)
        {
            return keyData - 48;
        }
    }
}
