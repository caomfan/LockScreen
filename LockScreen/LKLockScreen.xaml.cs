using LockScreen.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace LockScreen
{
    /// <summary>
    /// LKLockScreen.xaml 的交互逻辑
    /// </summary>
    public partial class LKLockScreen : Window
    {
        #region 构造函数
        public LKLockScreen()
        {
            InitializeComponent();
            screenUnlock.OnCheckedPoint += ScreenUnlock_OnCheckedPoint;
            screenUnlock.OnRememberPoint += ScreenUnlock_OnRememberPoint;

            numUnlock.UnLockStateEvent += NumUnlock_UnLockStateEvent;
            this.DataContext = MainWindow.VM;
            ImgCount = GetImageFileCount(MainWindow.VM.FilePath);
            storyboard.Completed += Storyboard_Completed;
            LoopToPalyAnimation();
            this.Closed += LKLockScreen_Closed;
        }

        #endregion

        #region 字段
        /// <summary>
        /// 图片个数
        /// </summary>
        public int ImgCount { get; set; }
        public int index { get; set; } = 0;
        public List<string> ImgPathList { get; set; } = new List<string>();
        public Storyboard storyboard = new Storyboard();
        #endregion

        #region 多线程单例
        private static LKLockScreen lkLockScreen;
        private static readonly object locker = new object();
        public static LKLockScreen GetInstance()
        {
            if(lkLockScreen==null)
            {
                lock(locker)
                {
                    if (lkLockScreen == null)
                    {
                        lkLockScreen = new LKLockScreen();
                    }
                }
                
            }
            return lkLockScreen;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            lkLockScreen = null;
        }
        #endregion

        #region Method
        private void NumUnlock_UnLockStateEvent(object sender, EventArgs e)
        {
            if (sender.ToString() == "1")
            {
                GC.Collect();
                storyboard.Completed -= Storyboard_Completed;
                storyboard.Stop();
                this.Close();
            }
            else
            {
                //MessageBox.Show("解锁失败！");
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
                GC.Collect();
                storyboard.Completed -= Storyboard_Completed;
                storyboard.Stop();
                this.Close();
            }
            else
            {
                //MessageBox.Show("密码解锁失败！");
            }
        }

        /// <summary>
        /// 循环播放动画
        /// </summary>
        private void LoopToPalyAnimation()
        {
            //创建一个故事板


            //创建关键帧动画
            DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            doubleAnimationUsingKeyFrames.AutoReverse = true;
            storyboard.Children.Add(doubleAnimationUsingKeyFrames);
            Storyboard.SetTarget(doubleAnimationUsingKeyFrames, IMG);
            Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath("Opacity"));

            //创建关键帧
            EasingDoubleKeyFrame easingDoubleKeyFrame1 = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            EasingDoubleKeyFrame easingDoubleKeyFrame2 = new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3)));
            EasingDoubleKeyFrame easingDoubleKeyFrame3 = new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(5)));

            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame1);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame2);
            doubleAnimationUsingKeyFrames.KeyFrames.Add(easingDoubleKeyFrame3);
            storyboard.Begin();

        }

        /// <summary>
        /// 获取文件夹下图像个数
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private int GetImageFileCount(string filePath)
        {
            ImgPathList.Clear();
            string fileDir = System.IO.Path.GetDirectoryName(filePath);
            DirectoryInfo dirInfo = new DirectoryInfo(fileDir);
            foreach (FileInfo item in dirInfo.GetFiles())
            {
                ImgPathList.Add(item.FullName);
            }
            if (dirInfo.GetFiles().Count() >= 1)
            {
                return dirInfo.GetFiles().Count();
            }
            else return 0;

        }
        #endregion

        #region 事件

        public event EventHandler<EventArgs> OnCloseEvent;

        private void LKLockScreen_Closed(object sender, EventArgs e)
        {
            //返回1，关闭窗口
            OnCloseEvent?.Invoke(1, new EventArgs());
        }
        /// <summary>
        /// 动画完成，播放另外一个动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            index++;
            if (MainWindow.VM.FilePath == ImgPathList[index % ImgCount])
                index++;
            MainWindow.VM.FilePath = ImgPathList[index % ImgCount];
            storyboard.Begin();
        }
        #endregion

        #region 依赖属性
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
        #endregion
    }
}
