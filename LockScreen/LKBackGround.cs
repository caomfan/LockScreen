using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LockScreen
{
    public class LKBackGround : ViewModel.ViewModelBase
    {
        private SolidColorBrush _BackgroundColor =(SolidColorBrush)( new BrushConverter() .ConvertFromString("#33000000"));
        /// <summary>
        /// 背景颜色
        /// </summary>
        public SolidColorBrush BackgroundColor
        {
            get { return _BackgroundColor; }
            set { _BackgroundColor = value;this.RaisePropertyChanged(() => BackgroundColor); }
        }

        private int _SeccondLock = 60;
        /// <summary>
        /// 无操作多少秒锁屏
        /// </summary>
        public int SeccondLock
        {
            get { return _SeccondLock; }
            set { _SeccondLock = value; RaisePropertyChanged(() => SeccondLock); }
        }

        private PasswordType _PassType = PasswordType.Num;
        /// <summary>
        /// 密码通过类型
        /// </summary>
        public PasswordType PassType
        {
            get { return _PassType; }
            set { _PassType = value; RaisePropertyChanged(() => PassType); }
        }

        private BackGroundType _BackType;
        /// <summary>
        /// 背景类型
        /// </summary>
        public BackGroundType BackType
        {
            get { return _BackType; }
            set { _BackType = value; RaisePropertyChanged(() => BackType); }
        }

        private string _FilePath = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value; RaisePropertyChanged(() => FilePath);

                if(BackType==BackGroundType.Image)
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    if (System.IO.File.Exists(FilePath))
                    {
                        image.UriSource = new System.Uri(value);
                        image.DecodePixelWidth = (int)System.Windows.SystemParameters.WorkArea.Width;
                        image.EndInit();
                        image.Freeze();
                    }
                    LKImagesource = image;
                }
               
            }
        }

        /// <summary>
        /// 图片源
        /// </summary>

        public ImageSource LKImagesource
        {
            get
            {
                BitmapImage image = new BitmapImage();
                if (BackType == BackGroundType.Image)
                {
                   
                    image.BeginInit();
                    if (System.IO.File.Exists(FilePath))
                    {
                        image.UriSource = new System.Uri(FilePath);
                        image.DecodePixelWidth = (int)System.Windows.SystemParameters.WorkArea.Width;
                        image.EndInit();
                        image.Freeze();
                    }

                   
                }
                return image;

            }
            set { this.RaisePropertyChanged(() => LKImagesource); }
        }


        /// <summary>
        /// 背景类型
        /// </summary>
        public enum BackGroundType
        {
            Image = 0,
            Gif = 1,
            Video = 2,
            Flash = 3
        }

        /// <summary>
        ///密码类型，0位数字类型，1为图案类型
        /// </summary>
        public enum PasswordType
        {
            Num = 101,
            Draw = 102
        }

        /// <summary>
        /// 获取文件过滤字符
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFilter(int type)
        {
            switch (type)
            {
                case 0: return "*.JPG;*.PNG;*.BMP;*.GIF|*.JPG;*.PNG;*.BMP;*.GIF";
                case 1: return "*.GIF|*.GIF";
                case 2: return "*.WMV;*.AVI;*.MP4;*.MPG|*.WMV;*.AVI;*.MP4;*.MPG";
                case 3: return "*.SWF|*.SWF";
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
