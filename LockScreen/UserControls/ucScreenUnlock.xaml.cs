using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LockScreen.UserControls
{
    /// <summary>
    /// ucScreenUnlock.xaml 的交互逻辑
    /// </summary>
    public partial class ucScreenUnlock : UserControl
    {
        public ucScreenUnlock()
        {
            InitializeComponent();
            this.Loaded += ScreenUnlock_Loaded;
            this.Unloaded += ScreenUnlock_Unloaded;
            this.MouseMove += ScreenUnlock_MouseMove;
            this.SizeChanged += ScreenUnlock_SizeChanged;
            this.MouseUp += UcScreenUnlock_MouseUp;
        }

        #region 事件
        private void UcScreenUnlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Operation == ScreenUnLockOperationType.Check)
            {
                Console.WriteLine("playAnimation Check");
                var result = CheckPoint(); //执行图形检查
                                           //执行完成动画并触发检查事件
                PlayAnimation(result, () =>
                {
                    if (OnCheckedPoint != null)
                    {
                        this.Dispatcher.BeginInvoke(OnCheckedPoint, this, new CheckPointArgs() { Result = result }); //触发检查完成事件
                    }
                });

            }
            else if (Operation == ScreenUnLockOperationType.Remember)
            {
                Console.WriteLine("playAnimation Remember");
                RememberPoint(); //记忆绘制的坐标
                var args = new RememberPointArgs() { PointArray = this.PointArray };
                //执行完成动画并触发记忆事件
                PlayAnimation(true, () =>
                {
                    if (OnRememberPoint != null)
                    {
                        this.Dispatcher.BeginInvoke(OnRememberPoint, this, args);   //触发图形记忆事件
                    }
                });
            }
        }
        private void ScreenUnlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CreatePoint();
        }
        private void ScreenUnlock_Unloaded(object sender, RoutedEventArgs e)
        {
            rightColor = null;
            errorColor = null;
            if (currentPointArray != null)
            {
                this.currentPointArray.Clear();
            }
            if (currentLineList != null)
            {
                this.currentLineList.Clear();
            }
            if (ellipseList != null)
            {
                this.ellipseList.Clear();
            }
            this.canvasRoot.Children.Clear();
        }

        private void ScreenUnlock_Loaded(object sender, RoutedEventArgs e)
        {
            isChecking = false;
            rightColor = new SolidColorBrush(Colors.Green);
            errorColor = new SolidColorBrush(Colors.Red);
            currentPointArray = new List<string>();
            currentLineList = new List<Line>();
            ellipseList = new List<Ellipse>();
            CreatePoint();
        }

        /// <summary>
        /// 鼠标移动画线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenUnlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (isChecking)//如果图形正在检查中，不响应后续处理
            {
                return;
            }
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                //如果是按下
                var point = e.GetPosition(this);
                HitTestResult result = VisualTreeHelper.HitTest(this, point);
                Ellipse ellipse = result.VisualHit as Ellipse;

                if (ellipse != null)
                {
                    if (currentLine == null)
                    {
                        //从头开始绘制
                        currentLine = CreateLine();
                        canvasRoot.Children.Add(currentLine);
                        var ellipseCenterPoint = GetCenterPoint(ellipse);
                        currentLine.X1 = currentLine.X2 = ellipseCenterPoint.X;
                        currentLine.Y1 = currentLine.Y2 = ellipseCenterPoint.Y;

                        currentPointArray.Add(ellipse.Tag.ToString());
                        Console.WriteLine(string.Join(",", currentPointArray));
                        currentLineList.Add(currentLine);
                    }
                    else
                    {

                        //遇到下一个点，排除已经经过的点
                        if (currentPointArray.Contains(ellipse.Tag.ToString()))
                        {
                            return;
                        }
                        OnAfterByPoint(ellipse);
                    }
                }
                else
                {
                    if (ellipse == null)
                    {
                        ellipse = IsOnLineEx(point);
                    }
                    if (currentLine != null)
                    {
                        currentLine.X2 = point.X;
                        currentLine.Y2 = point.Y;
                    }
                }

            }
            else
            {
                if (currentPointArray.Count == 0)
                    return;
                isChecking = true;
                if (currentLineList.Count + 1 != currentPointArray.Count)
                {
                    //最后一条线的终点不在点上
                    //两点一线，点的个数-1等于线的条数
                    currentLineList.Remove(currentLine); //从已记录的线集合中删除最后一条多余的线
                    canvasRoot.Children.Remove(currentLine); //从界面上删除最后一条多余的线
                    currentLine = null;
                }

                //if (Operation == ScreenUnLockOperationType.Check)
                //{
                //    Console.WriteLine("playAnimation Check");
                //    var result = CheckPoint(); //执行图形检查
                //                               //执行完成动画并触发检查事件
                //    PlayAnimation(result, () =>
                //    {
                //        if (OnCheckedPoint != null)
                //        {
                //            this.Dispatcher.BeginInvoke(OnCheckedPoint, this, new CheckPointArgs() { Result = result }); //触发检查完成事件
                //        }
                //    });

                //}
                //else if (Operation == ScreenUnLockOperationType.Remember)
                //{
                //    Console.WriteLine("playAnimation Remember");
                //    RememberPoint(); //记忆绘制的坐标
                //    var args = new RememberPointArgs() { PointArray = this.PointArray };
                //    //执行完成动画并触发记忆事件
                //    PlayAnimation(true, () =>
                //    {
                //        if (OnRememberPoint != null)
                //        {
                //            this.Dispatcher.BeginInvoke(OnRememberPoint, this, args);   //触发图形记忆事件
                //        }
                //    });
                //}
            }
        }

        public event EventHandler<CheckPointArgs> OnCheckedPoint;
        public event EventHandler<RememberPointArgs> OnRememberPoint;
        #endregion

        #region 创建操作
        /// <summary>
        /// 创建点 9宫格
        /// </summary>
        private void CreatePoint()
        {
            canvasRoot.Children.Clear();
            DropShadowEffect shadow = new DropShadowEffect();
            shadow.ShadowDepth = 0;
            shadow.BlurRadius = 20;
            shadow.Color = Colors.White;

            if (ellipseList == null)
                ellipseList = new List<Ellipse>();
            if (ellipseList.Count > 0)
                ellipseList.Clear();
            int row = 3, column = 3;//三行三列九宫格
            double oneColumnWidth = (this.ActualWidth == 0 ? this.Width : this.ActualWidth / 3);//单列的宽度
            double oneRowHeight = (this.ActualHeight == 0 ? this.Height : this.ActualHeight / 3);//单行的高度
            double leftDistance = (oneColumnWidth - PointSize) / 2;
            double topDistance = (oneRowHeight - PointSize) / 2;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    Ellipse ellipse = new Ellipse()
                    {
                        Width = PointSize,
                        Height = PointSize,
                        Fill = Color,
                        Tag = string.Format("{0}{1}", i, j),
                        Effect= shadow
                    };
                    Canvas.SetLeft(ellipse, j * oneColumnWidth + leftDistance);
                    Canvas.SetTop(ellipse, i * oneRowHeight + topDistance);
                    canvasRoot.Children.Add(ellipse);
                    ellipseList.Add(ellipse);
                }
            }
        }

        /// <summary>
        /// 创建线
        /// </summary>
        /// <returns></returns>
        private Line CreateLine()
        {
            Line line = new Line()
            {
                Stroke = Color,
                StrokeThickness = 2
            };
            return line;
        }
        #endregion

        #region 判断
        /// <summary>
        /// 记录绘制的点
        /// </summary>
        private void RememberPoint()
        {
            if (PointArray.Count > 0)
            {
                PointArray.Clear();
                foreach (var item in currentPointArray)
                {
                    PointArray.Add(item);
                }
            }
        }

        /// <summary>
        /// 判断是否经过了某点
        /// </summary>
        /// <returns></returns>
        private Ellipse IsOnLine()
        {
            double lineAB = 0;  //当前画线的长度
            double lineCA = 0;  //当前点和A点的距离  
            double lineCB = 0;   //当前点和B点的距离
            double dis = 0;
            double deciation = 1; //允许的偏差距离
            lineAB = GetLineLength(currentLine.X1, currentLine.Y1, currentLine.X2, currentLine.Y2);  //计算当前画线的长度

            foreach (Ellipse ellipse in ellipseList)
            {
                if (currentPointArray.Contains(ellipse.Tag.ToString())) //排除已经经过的点
                    continue;
                var ellipseCenterPoint = GetCenterPoint(ellipse); //取当前点的中心点
                lineCA = GetLineLength(currentLine.X1, currentLine.Y1, ellipseCenterPoint.X, ellipseCenterPoint.Y); //计算当前点到线A端的长度
                lineCB = GetLineLength(currentLine.X2, currentLine.Y2, ellipseCenterPoint.X, ellipseCenterPoint.Y); //计算当前点到线B端的长度
                dis = Math.Abs(lineAB - (lineCA + lineCB));  //线CA的长度+线CB的长度>当前线AB的长度 说明点不在线上
                if (dis <= deciation)  //因为绘制的点具有一个宽度和高度，所以需设定一个允许的偏差范围，让线靠近点就命中之（吸附效果）
                {
                    return ellipse;
                }
            }
            return null;
        }

        /// <summary>
        /// 判断是否经过了某点，扩展
        /// </summary>
        /// <returns></returns>
        private Ellipse IsOnLineEx(Point p)
        {
            double lineCurrent = 0;//当前绘制的长度
            double lineTarget = 0;//目标点的长度
            double deciation = 20;//允许偏差的距离

            Ellipse ellipse = null;
            if (currentLine != null)
            {
                lineCurrent = GetLineLength(currentLine.X1, currentLine.Y1, currentLine.X2, currentLine.Y2);
                ellipse = ellipseList.ToList().Find(x => x.Tag.ToString() == string.Format("{0}{1}", (int)p.Y / (int)(this.ActualHeight / 3), (int)p.X / (int)(this.ActualWidth / 3)));
                if (ellipse != null)
                {
                    var point = GetCenterPoint(ellipse);
                    lineTarget = GetLineLength(currentLine.X1, currentLine.Y1, point.X, point.Y);
                    if (lineTarget > lineCurrent)//如果比目标长度小，判断鼠标点与目标点的距离
                    {
                        double lineTem = GetLineLength(p.X, p.Y, point.X, point.Y);
                        if (lineTem <= deciation)
                        {
                            //遇到下一个点，排除已经经过的点
                            if (currentPointArray.Contains(ellipse.Tag.ToString()))
                            {
                                return null;
                            }
                            OnAfterByPoint(ellipse);
                        }
                    }
                    else
                    {
                        double lineTem = GetPointToLineLength(point, new Point(currentLine.X1, currentLine.Y1), new Point(currentLine.X2, currentLine.Y2));
                        if (lineTem <= deciation)
                        {
                            //遇到下一个点，排除已经经过的点
                            if (currentPointArray.Contains(ellipse.Tag.ToString()))
                            {
                                return null;
                            }
                            OnAfterByPoint(ellipse);
                        }
                    }
                }
            }
            return ellipse;
        }

        /// <summary>
        /// 获取点到线的距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <returns></returns>
        private double GetPointToLineLength(Point p, Point linePoint1, Point linePoint2)
        {
            double k1 = 0, k2 = 0, a1 = 0, a2 = 0, x = 0, y = 0;
            k1 = (linePoint2.Y - linePoint1.Y) / (linePoint2.X - linePoint1.X);
            a1 = linePoint1.Y - k1 * linePoint1.X;
            k2 = -1.0 / k1;
            a2 = p.Y - k2 * p.X;
            y = (a2 - (k2 / k1) * a1) / (1 - (k2 / k1));
            x = (y - a1) / k1;
            return GetLineLength(p.X, p.Y, x, y);
        }

        /// <summary>
        /// 获取两点长度
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private double GetLineLength(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));//两点计算公式
        }

        /// <summary>
        /// 检查坐标点是否正确
        /// </summary>
        /// <returns></returns>
        private bool CheckPoint()
        {
            if (currentPointArray.Count != PointArray.Count)
            {
                return false;
            }
            for (int i = 0; i < currentPointArray.Count; i++)
            {
                if (currentPointArray[i] != PointArray[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 记录经过的点
        /// </summary>
        /// <param name="ellipse"></param>
        private void OnAfterByPoint(Ellipse ellipse)
        {
            var ellipseCenterPoint = GetCenterPoint(ellipse);
            currentLine.X2 = ellipseCenterPoint.X;
            currentLine.Y2 = ellipseCenterPoint.Y;
            currentLine = CreateLine();
            currentLine.X1 = currentLine.X2 = ellipseCenterPoint.X;
            currentLine.Y1 = currentLine.Y2 = ellipseCenterPoint.Y;
            currentPointArray.Add(ellipse.Tag.ToString());
            Console.WriteLine(string.Join(",", currentPointArray));
            currentLineList.Add(currentLine);
            canvasRoot.Children.Add(currentLine);

        }

        /// <summary>
        /// 获取原点的中心坐标
        /// </summary>
        /// <param name="ellipse"></param>
        /// <returns></returns>
        private Point GetCenterPoint(Ellipse ellipse)
        {
            Point p = new Point(Canvas.GetLeft(ellipse) + ellipse.Width / 2, Canvas.GetTop(ellipse) + ellipse.Height / 2);
            return p;
        }
        #endregion

        #region 依赖属性
        /// <summary>
        /// 验证正确的颜色
        /// </summary>
        private SolidColorBrush rightColor;

        /// <summary>
        /// 验证失败的颜色
        /// </summary>
        private SolidColorBrush errorColor;

        /// <summary>
        /// 图形正在检查中
        /// </summary>
        private bool isChecking;

        /// <summary>
        /// 记忆的坐标点
        /// </summary>
        public IList<string> PointArray
        {
            get { return (IList<string>)GetValue(PointArrayProperty); }
            set { SetValue(PointArrayProperty, value); }
        }

        public static readonly DependencyProperty PointArrayProperty =
            DependencyProperty.Register("PointArray", typeof(IList<string>), typeof(ucScreenUnlock));

        /// <summary>
        /// 当前坐标集合
        /// </summary>
        private IList<string> currentPointArray;

        /// <summary>
        /// 当前线集合
        /// </summary>
        private IList<Line> currentLineList;

        /// <summary>
        /// 点集合
        /// </summary>
        private IList<Ellipse> ellipseList;

        /// <summary>
        /// 当前正在绘制的线
        /// </summary>
        private Line currentLine;

        /// <summary>
        /// 操作类型
        /// </summary>
        public ScreenUnLockOperationType Operation
        {
            get { return (ScreenUnLockOperationType)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }
        public static readonly DependencyProperty OperationProperty =
            DependencyProperty.Register("Operation", typeof(ScreenUnLockOperationType), typeof(ucScreenUnlock), new FrameworkPropertyMetadata(ScreenUnLockOperationType.Remember));

        /// <summary>
        /// 坐标点大小
        /// </summary>
        public double PointSize
        {
            get { return (double)GetValue(PointSizeProperty); }
            set { SetValue(PointSizeProperty, value); }
        }
        public static readonly DependencyProperty PointSizeProperty =
            DependencyProperty.Register("PointSize", typeof(double), typeof(ucScreenUnlock), new PropertyMetadata(15.0));

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(ucScreenUnlock), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback((s, e) =>
            {
                (s as ucScreenUnlock).Refresh();
            })));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ucScreenUnlock), new PropertyMetadata(new CornerRadius(10)));

        #endregion

        #region 其它
        /// <summary>
        /// 刷新颜色
        /// </summary>
        private void Refresh()
        {

        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public enum ScreenUnLockOperationType
        {
            Remember = 0, Check = 1
        }

        /// <summary>
        /// 执行动画
        /// </summary>
        /// <param name="result"></param>
        /// 
        private void PlayAnimation(bool result, Action callback = null)
        {
            Task.Factory.StartNew(() =>
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    foreach (Line l in currentLineList)
                        l.Stroke = result ? rightColor : errorColor;
                    foreach (Ellipse e in ellipseList)
                        if (currentPointArray.Contains(e.Tag.ToString()))
                            e.Fill = result ? rightColor : errorColor;
                });
                Thread.Sleep(500);
                this.Dispatcher.Invoke((Action)delegate
                {
                    foreach (Line l in currentLineList)
                        this.canvasRoot.Children.Remove(l);
                    foreach (Ellipse e in ellipseList)
                        e.Fill = Color;
                });
                currentLine = null;
                this.currentPointArray.Clear();
                this.currentLineList.Clear();
                isChecking = false;
            }).ContinueWith(t =>
            {
                try
                {
                    if (callback != null)
                        callback();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    t.Dispose();
                }
            });
        }
        #endregion
    }
    public class CheckPointArgs : EventArgs
    {
        public CheckPointArgs()
        {
        }

        public bool Result { get; set; }
    }

    public class RememberPointArgs : EventArgs
    {
        public RememberPointArgs()
        {
        }

        public IList<string> PointArray { get; set; }
    }
}
