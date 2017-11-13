using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockScreen
{
    public class LastInputInfoHook
    {
        Win32Api.LASTINPUTINFO lastInPut = new Win32Api.LASTINPUTINFO();
        /// <summary>
        /// 无操作多少秒后返回True，有操作则返回false
        /// </summary>
        /// <param name="milli">毫秒</param>
        /// <returns></returns>
        public bool NoOpera(long milli)
        {
            //配置时间内无操作锁定
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            Win32Api.GetLastInputInfo(ref lastInPut);

            long noOperationTime = System.Environment.TickCount - lastInPut.dwTime;

            if (noOperationTime > milli)
            {
                //退出程序
                return true;
            }
            return false;
        }
    }
}
