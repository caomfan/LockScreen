using LockScreen.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LockScreen
{
    /// <summary>
    /// LKLockScreen.xaml 的交互逻辑
    /// </summary>
    public partial class LKLockScreen : Window
    {
        public LKLockScreen()
        {
            InitializeComponent();
            screenUnlock.OnCheckedPoint += ScreenUnlock_OnCheckedPoint;
            screenUnlock.OnRememberPoint += ScreenUnlock_OnRememberPoint;

            numUnlock.UnLockStateEvent += NumUnlock_UnLockStateEvent;
            this.DataContext = MainWindow.VM;
        }

        private void NumUnlock_UnLockStateEvent(object sender, EventArgs e)
        {
            if(sender.ToString()=="1")
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("解锁失败！");
            }
        }

        /// <summary>
        /// 记忆密码成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenUnlock_OnRememberPoint(object sender, RememberPointArgs e)
        {
            MessageBox.Show("设置的密码为：" + string.Join(",", e.PointArray));
        }

        /// <summary>
        /// 解锁密码成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenUnlock_OnCheckedPoint(object sender, CheckPointArgs e)
        {
            if (e.Result)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("密码解锁失败！");
            }
        }

        public IList<string> PointArray
        {
            get { return (IList<string>)GetValue(PointArrayProperty); }
            set { SetValue(PointArrayProperty, value); }
        }
        public static readonly DependencyProperty PointArrayProperty =
            DependencyProperty.Register("PointArray", typeof(IList<string>), typeof(MainWindow), new PropertyMetadata(new List<string>() { "00", "01", "02", "12" }));

        public string Operation
        {
            get { return (string)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }
        public static readonly DependencyProperty OperationProperty =
            DependencyProperty.Register("Operation", typeof(string), typeof(MainWindow), new PropertyMetadata("Check"));
    }
}
