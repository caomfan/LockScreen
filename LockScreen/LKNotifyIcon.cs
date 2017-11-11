using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LockScreen
{
    public class LKNotifyIcon
    {
        public LKNotifyIcon()
        {
            InitialTray();
        }

        public event EventHandler<NotifyEvent> OnChangeWindowState;

        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private void InitialTray()
        {
            //设置托盘各个属性
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            //notifyIcon.BalloonTipText = "程序开始运行";
            notifyIcon.Text = "JC锁屏";
            notifyIcon.Icon = new System.Drawing.Icon(System.Windows.Application.GetResourceStream(new Uri("Pack://application:,,,/LOCK.ico")).Stream);

            notifyIcon.Visible = true;
            //notifyIcon.ShowBalloonTip(500);
            notifyIcon.MouseClick += NotifyIcon_MouseClick; ;

            //设置菜单项
            System.Windows.Forms.MenuItem showMenu = new System.Windows.Forms.MenuItem("显示主界面");
            showMenu.Click += ShowMenu_Click;

            System.Windows.Forms.MenuItem menu = new System.Windows.Forms.MenuItem("-");
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += Exit_Click;

            //关联托盘控件
            System.Windows.Forms.MenuItem[] children = new System.Windows.Forms.MenuItem[] { showMenu, menu, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(children);
            //窗体状态改变时触发

        }

        /// <summary>
        /// 单击托盘图标显示主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button==System.Windows.Forms.MouseButtons.Left)
            {
                OnChangeWindowState?.Invoke(WindowState.Normal, new NotifyEvent() { WindowState = WindowState.Normal, IsShowWindow = true });
            }
         
        }

        /// <summary>
        /// 显示主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMenu_Click(object sender, EventArgs e)
        {
            OnChangeWindowState?.Invoke(WindowState.Normal, new NotifyEvent() { WindowState=WindowState.Normal,IsShowWindow=true});
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e)
        {
            if (System.Windows.MessageBox.Show("确定要退出吗？", "退出", MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                notifyIcon.Dispose();
                System.Windows.Application.Current.Shutdown();
            }
        }
    }

    public class NotifyEvent:EventArgs
    {
        public WindowState WindowState { get; set; }
        public bool IsShowWindow { get; set; }
        public object tag { get; set; }
    }
}
