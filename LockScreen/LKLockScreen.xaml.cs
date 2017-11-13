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
using LockScreen.Unity;
using System.Windows.Forms.Integration;

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
            
            //图案解锁事件订阅
            screenUnlock.OnCheckedPoint += ScreenUnlock_OnCheckedPoint;
            screenUnlock.OnRememberPoint += ScreenUnlock_OnRememberPoint;

            //键盘解锁，安装键盘钩子
            KeyboardHook keyboardHook = KeyboardHook.GetInstance();
            keyboardHook.SetHook();
            numUnlock.UnLockStateEvent += NumUnlock_UnLockStateEvent;
            keyboardHook.OnKeyDownEvent += KeyboardHook_OnKeyDownEvent;
            keyboardHook.OnKeyUpEvent += KeyboardHook_OnKeyUpEvent;

            //屏蔽任务管理器
            Win32Api.ManageTaskManager(1);

            this.DataContext = MainWindow.VM;

            //初始化背景
            InitBackground();

            this.Closed += (s, e) =>
            {
                //返回1，关闭窗口
                keyboardHook.UnHook();
                OnCloseEvent?.Invoke(1, new EventArgs());
                keyboardHook.OnKeyDownEvent -= KeyboardHook_OnKeyDownEvent;
                keyboardHook.OnKeyUpEvent -= KeyboardHook_OnKeyUpEvent;
                Win32Api.ManageTaskManager(0);
            };
        }

        #endregion

        #region 字段
        private Image IMG { get; set; }
        /// <summary>
        /// 图片个数
        /// </summary>
        public int ImgCount { get; set; }
        public int index { get; set; } = 0;
        public List<string> ImgPathList { get; set; } = new List<string>();
        public Storyboard storyboard = new Storyboard();
        #endregion

        #region 多线程单例
        private static LKLockScreen lkLockScreen ;
        private static readonly object locker = new object();
        public static LKLockScreen GetInstance()
        {
            if (lkLockScreen == null)
            {
                lock (locker)
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

        /// <summary>
        /// 初始化背景
        /// </summary>
        private void InitBackground()
        {
            if (MainWindow.VM.BackType == LKBackGround.BackGroundType.Image)
            {
                ImgCount = GetImageFileCount(MainWindow.VM.FilePath);
                if(ImgCount>=1)
                {
                    IMG = new Image();
                    Binding bind = new Binding()
                    {
                        Source = MainWindow.VM,
                        Path = new PropertyPath("LKImagesource"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    IMG.SetBinding(Image.SourceProperty, bind);
                    this.content.Content = IMG;

                    storyboard.Completed += Storyboard_Completed;

                    if (ImgCount > 1)
                    {
                        LoopToPalyAnimation();
                    }
                }
               
            }
            else if (MainWindow.VM.BackType == LKBackGround.BackGroundType.Gif)
            {
                GifImage gifImage = new GifImage(MainWindow.VM.FilePath);
                gifImage.Stretch = Stretch.Fill;
                this.content.Content = gifImage;
                gifImage.StartAnimate();
            }
            else if (MainWindow.VM.BackType == LKBackGround.BackGroundType.Video)
            {
                MediaElement media = new MediaElement();
                media.Source =new Uri( MainWindow.VM.FilePath);
                media.LoadedBehavior = MediaState.Manual;
                media.Play();
                //填充整个屏幕
                media.Stretch = Stretch.Fill;
                this.content.Content = media;
                //当媒体播放结束时重复播放
                media.MediaEnded += (s, e) =>
                {
                    media.Position = TimeSpan.Zero;
                    media.Play();
                };
            }
            
        }

        /// <summary>
        /// 判断数字解锁是否成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (item.Extension.Equals(".JPG", StringComparison.OrdinalIgnoreCase) || item.Extension.Equals(".PNG", StringComparison.OrdinalIgnoreCase) || item.Extension.Equals(".BMP", StringComparison.OrdinalIgnoreCase) || item.Extension.Equals(".GIF", StringComparison.OrdinalIgnoreCase))
                {
                    ImgPathList.Add(item.FullName);
                }

            }
            return ImgPathList.Count;
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

        private void KeyboardHook_OnKeyUpEvent(object sender, OnKeyUpEvent e)
        {
            numUnlock.ChangeKeyState(e.KeyData, 1);
        }

        private void KeyboardHook_OnKeyDownEvent(object sender, KeyDownEvent e)
        {
            numUnlock.ChangeKeyState(e.KeyData, 0);
        }
        #endregion

        #region 依赖属性
        public IList<string> PointArray
        {
            get { return (IList<string>)GetValue(PointArrayProperty); }
            set { SetValue(PointArrayProperty, value); }
        }
        public static readonly DependencyProperty PointArrayProperty =
            DependencyProperty.Register("PointArray", typeof(IList<string>), typeof(LKLockScreen), new PropertyMetadata(ConfigManager.GetDrawPass()));

        public string Operation
        {
            get { return (string)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }
        public static readonly DependencyProperty OperationProperty =
            DependencyProperty.Register("Operation", typeof(string), typeof(LKLockScreen), new PropertyMetadata("Check"));

        #endregion
    }
}

