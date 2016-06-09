using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easyIcon
{
    /// <summary>
    /// 此类用于实现对注册表的操作
    /// </summary>
    class Registry
    {
        # region 注册表操作
        //设置软件开机启动项： RegistrySave(@"Microsoft\Windows\CurrentVersion\Run", "QQ", "C:\\windows\\system32\\QQ.exe");  

        /// <summary>
        /// 记录键值数据到注册表subkey = @"Scimence\Email\Set";
        /// </summary>
        public static void RegistrySave(string subkey, string name, object value)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keyCur = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\" + subkey, true);
            if (keySet == null) keySet = keyCur.CreateSubKey(subkey);   //键不存在时创建

            keySet.SetValue(name, value);   //保存键值数据
        }

        /// <summary>
        /// 获取注册表subkey下键name的字符串值
        /// <summary>
        public static string RegistryStrValue(string subkey, string name)
        {
            object value = RegistryValue(subkey, name);
            return value == null ? "" : value.ToString();
        }

        /// <summary>
        /// 获取注册表subkey下键name的值
        /// <summary>
        public static object RegistryValue(string subkey, string name)
        {
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\" + subkey, true);
            return (keySet == null ? null : keySet.GetValue(name, null));
        }

        /// <summary>
        /// 判断注册表是否含有子键subkey
        /// <summary>
        public static bool RegistryCotains(string subkey)
        {
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\" + subkey, true);
            return (keySet != null);
        }

        /// <summary>
        /// 判断注册表subkey下是否含有name键值信息
        /// <summary>
        public static bool RegistryCotains(string subkey, string name)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\" + subkey, true);

            if (keySet == null) return false;
            else return keySet.GetValueNames().Contains<string>(name);
        }

        /// <summary>
        /// 删除注册表subkey信息
        /// <summary>
        public static void RegistryRemove(string subkey)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keyCur = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\" + subkey, true);

            if (keySet != null) keyCur.DeleteSubKeyTree(subkey);      //删除注册表信息
        }

        /// <summary>
        /// 删除注册表subkey下的name键值信息
        /// <summary>
        public static void RegistryRemove(string subkey, string name)
        {
            //设置一个具有写权限的键 访问键注册表"HKEY_CURRENT_USER\Software"
            Microsoft.Win32.RegistryKey keySet = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\" + subkey, true);
            if (keySet != null) keySet.DeleteValue(name, false);
        }

        # endregion
    }
}
