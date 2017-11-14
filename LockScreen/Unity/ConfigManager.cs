using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;



namespace LockScreen
{
    public class ConfigManager
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
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }

        ///<summary>  
        ///在*.exe.config文件中appSettings配置节增加一对键值对  
        ///</summary>  
        ///<param name="newKey"></param>  
        ///<param name="newValue"></param>  
        public static void UpdateAppConfig(string newKey, string newValue)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            bool exist = false;
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == newKey)
                {
                    exist = true;
                }
            }
            if (exist)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            config.AppSettings.Settings.Add(newKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
