using LockScreen.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LockScreen
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static LKBackGround VM;
        public MainWindow()
        {
            InitializeComponent();
            VM = new LKBackGround();
            this.DataContext = VM;
            LKNotifyIcon lkNotifyIcon = new LKNotifyIcon();
            lkNotifyIcon.OnChangeWindowState += LkNotifyIcon_OnChangeWindowState;
        }

        private void LkNotifyIcon_OnChangeWindowState(object sender, NotifyEvent e)
        {
            this.WindowState = e.WindowState;
            if(e.IsShowWindow)
            {
                this.Show();
                this.Activate();
            }
        }

        private static System.Windows.Threading.DispatcherTimer timeTrigger;

        /// <summary>
        /// 选择文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = LKBackGround.GetFilter((int)VM.BackType);
            if (!string.IsNullOrWhiteSpace(VM.FilePath))
                of.InitialDirectory = System.IO.Path.GetDirectoryName(VM.FilePath);
            else
                of.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (of.ShowDialog() == true)
            {
                VM.FilePath = of.FileName;
            }
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            if(timeTrigger==null)
            {
                timeTrigger = new System.Windows.Threading.DispatcherTimer();
                timeTrigger.Tick += new EventHandler(timeCycle);
                timeTrigger.Interval = TimeSpan.FromSeconds(1);
                timeTrigger.Start();
            }
            else
            {
                timeTrigger.Stop();
                timeTrigger.Start();
            }
            
        }

        //时钟事件
        public  void timeCycle(object sender, EventArgs e)
        {
            KeyboardHook keyboardHook = new KeyboardHook();
            if(keyboardHook.NoOpera(MainWindow.VM.SeccondLock*1000))
            {
                this.WindowState = WindowState.Minimized;
                LKLockScreen lkLockScreen = LKLockScreen.GetInstance();
                lkLockScreen.OnCloseEvent += (s, e2) =>
                {
                    if(s.ToString()=="1")
                    {
                        timeTrigger.Start();
                    }
                };
                lkLockScreen.Activate();
                lkLockScreen.Show();
                timeTrigger.Stop();
            }
        }
    }
}
