using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using static LockScreen.LKBackGround;

namespace LockScreen.Converters
{
    public class BackGroundTypeConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parm = parameter.ToString();
            switch (parm)
            {
                case "img":if ((int)value == 0)return true;break;
                case "gif": if ((int)value == 1) return true; break;
                case "video": if ((int)value == 2) return true; break;
                case "flash": if ((int)value == 3) return true; break;
                case "num": if ((int)value == 101) return true; break;
                case "draw": if ((int)value == 102) return true; break;
                default:
                    break;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parm = parameter.ToString();
            switch (parm)
            {
                case "img": return BackGroundType.Image;
                case "gif": return BackGroundType.Gif;
                case "video": return BackGroundType.Video;
                case "flash": return BackGroundType.Flash;
                case "num": return PasswordType.Num;
                case "draw": return PasswordType.Draw;
                default:
                    break;
            }
            return false;
        }
    }

    public class TypeToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parm = parameter.ToString();
            switch (parm)
            {
                case "num": return((int)value == 101?Visibility.Visible:Visibility.Collapsed);
                case "draw":return((int)value == 102 ?Visibility.Visible : Visibility.Collapsed); 
                default:
                    break;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == Visibility.Visible.ToString())
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
