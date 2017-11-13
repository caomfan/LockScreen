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
            ucScreenUnlock.OnRememberPoint += UcScreenUnlock_OnRememberPoint;
            txtPwd.Text = ConfigManager.GetNumPass();
        }

        private void UcScreenUnlock_OnRememberPoint(object sender, RememberPointArgs e)
        {
            ConfigManager.UpdateAppConfig("drawpass", string.Join("|", e.PointArray)); 
        }

        /// <summary>
        /// 托盘通知改变窗体状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LkNotifyIcon_OnChangeWindowState(object sender, NotifyEvent e)
        {
            if (e.IsShowWindow)
            {
                this.Show();
                this.Activate();
                this.WindowState = e.WindowState;
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
            if(MainWindow.VM.PassType==LKBackGround.PasswordType.Num)
            {
                ConfigManager.UpdateAppConfig("numpass", txtPwd.Text);
            }
            this.Hide();

            if (timeTrigger == null)
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

        /// <summary>
        /// 时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void timeCycle(object sender, EventArgs e)
        {
            LastInputInfoHook lastInputInfoHook = new LastInputInfoHook();
            if (lastInputInfoHook.NoOpera(MainWindow.VM.SeccondLock * 1000))
            {
                this.WindowState = WindowState.Minimized;
                LKLockScreen lkLockScreen = LKLockScreen.GetInstance();
                lkLockScreen.OnCloseEvent += (s, e2) =>
                {
                    if (s.ToString() == "1")
                    {
                        timeTrigger.Start();
                    }
                };
                lkLockScreen.Activate();
                lkLockScreen.Show();
                timeTrigger.Stop();
            }
        }

        /// <summary>
        /// 窗体最小化时，隐藏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                stackpanelSetting.Visibility = Visibility.Visible;
            }
            else
            {
                stackpanelSetting.Visibility = Visibility.Collapsed;
            }
        }

        #region 依赖属性
        public IList<string> PointArray
        {
            get { return (IList<string>)GetValue(PointArrayProperty); }
            set { SetValue(PointArrayProperty, value); }
        }
        public static readonly DependencyProperty PointArrayProperty =
            DependencyProperty.Register("PointArray", typeof(IList<string>), typeof(MainWindow), new PropertyMetadata(new List<string>()));

        #endregion

        
    }
}
