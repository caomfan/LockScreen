using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;



namespace LockScreen
{
    public class Unity
    {
        /// <summary>
        /// 获取绘图密码
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDrawPass()
        {
            List<string> drawpass = new List<string>() { "00", "01", "02", "12" };
            drawpass = GetAppConfig("drawpass").Split('|').ToList<string>();
            return drawpass;
        }

        /// <summary>
        /// 获取数字密码
        /// </summary>
        /// <returns></returns>
        public static string GetNumPass()
        {
            string numpass = "88888888";
            numpass = GetAppConfig("numpass");
            return numpass;
        }

        ///<summary> 
        ///返回*.exe.config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string strKey)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(strKey))
            {
                return ConfigurationManager.AppSettings[strKey];
            }
            return "";
        }

        ///<summary>  
        ///在*.exe.config文件中appSettings配置节增加一对键值对  
        ///</summary>  
        ///<param name="newKey"></param>  
        ///<param name="newValue"></param>  
        public static void  UpdateAppConfig(string newKey, string newValue)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            if (ConfigurationManager.AppSettings.AllKeys.Contains(newKey))
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            config.AppSettings.Settings.Add(newKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
