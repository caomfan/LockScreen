using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockScreen
{
    public class LKBackGround : ViewModel.ViewModelBase
    {

        private int _SeccondLock = 60;
        /// <summary>
        /// 无操作多少秒锁屏
        /// </summary>
        public int SeccondLock
        {
            get { return _SeccondLock; }
            set { _SeccondLock = value; RaisePropertyChanged(()=>SeccondLock); }
        }

        private PasswordType _PassType=PasswordType.Num;
        /// <summary>
        /// 密码通过类型
        /// </summary>
        public PasswordType PassType
        {
            get { return _PassType; }
            set { _PassType = value;RaisePropertyChanged(() => PassType); }
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

        private string _FilePath =AppDomain.CurrentDomain.BaseDirectory+@"Media\Image\GTGraphics.png";
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; RaisePropertyChanged(() => FilePath); }
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
            Num=101,
            Draw=102
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
                case 0:return "*.JPG;*.PNG;*.BMP;*.GIF|*.JPG;*.PNG;*.BMP;*.GIF";
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
