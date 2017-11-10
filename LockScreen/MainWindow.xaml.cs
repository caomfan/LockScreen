using Microsoft.Win32;
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
        }

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
            this.WindowState = WindowState.Minimized;
            LKLockScreen lkLockScreen = new LKLockScreen();
            lkLockScreen.Activate();
            lkLockScreen.Show();
        }
    }
}
