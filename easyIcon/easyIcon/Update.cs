using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace easyIcon
{
    class Update
    {
        //public static string ToolsName = "easyIcon";          // 工具名称
        static string ToolsName = ToolsFunction.ToolsName;      // 工具名称
        static long curVersion = 20160609;                      // 记录当前软件版本信息


        /// <summary>
        /// 检查工具是否已是最新版本, 不是则自动更新
        /// </summary>
        public static bool Updated()
        {
            try
            {
                // 获取版本配置信息 示例：scimence( Name1(6JSO-F2CM-4LQJ-JN8P) )scimence
                string VersionInfo = WebSettings.getWebData("https://git.oschina.net/scimence/easyIcon/raw/master/files/versionInfo.txt");
                if (VersionInfo.Equals("")) return true;

                // 获取版本更新信息
                long lastVersion = long.Parse(WebSettings.getNodeData(VersionInfo, "version", true));
                string url = WebSettings.getNodeData(VersionInfo, "url", true);

                // 检测到新的版本
                if (lastVersion > curVersion)
                {
                    bool ok = (MessageBox.Show("检测到新的版本，现在更新？", "版本更新", MessageBoxButtons.OKCancel) == DialogResult.OK);
                    if (ok)
                    {
                        CheckUpdateEXE();

                        // 获取更新插件名
                        string update_EXE = curDir() + "UpdateFiles.exe";
                        if (System.IO.File.Exists(update_EXE))
                        {
                            // 获取url中对应的文件名
                            string fileName = curDir() + System.IO.Path.GetFileName(url);

                            // 生成更新时的显示信息
                            String info = "当前版本：" + curVersion + "\r\n" + "最新版本：" + lastVersion + "\r\n";

                            // 调用更新插件执行软件更新逻辑
                            String arg = "\"" + url + "\"" + " " + "\"" + fileName + "\"" + " " + "true" + " " + "\"" + info + "\"";
                            System.Diagnostics.Process.Start(update_EXE, arg);

                            return false;
                        }
                        else MessageBox.Show("未找到更新插件：\r\n" + update_EXE);
                    }
                }

                return true;
            }
            catch (Exception ex) { return true; }  // 未获取到版本信息
        }

        // 检测更新插件UpdateFiles.exe是否存在，更新替换文件逻辑
        private static void CheckUpdateEXE()
        {
            string info = "更新UpdateFiles.exe\r\n";

            // 若文件不存在则自动创建
            string file1 = curDir() + "UpdateFiles.exe";

            string key = @"Scimence\" + ToolsName + @"\Set";
            string value = Registry.RegistryStrValue(key, "文件更新");

            if (!value.Contains(info) || !System.IO.File.Exists(file1))
            {
                Registry.RegistrySave(key, "文件更新", info);

                // UpdateFiles.exe文件资源 https://git.oschina.net/scimence/UpdateFiles/raw/master/files/UpdateFiles.exe
                Byte[] array = Properties.Resources.UpdateFiles.ToArray<Byte>();
                SaveFile(array, file1);
            }
        }


        /// <summary>
        /// 保存Byte数组为文件
        /// </summary>
        public static void SaveFile(Byte[] array, string path)
        {
            // 创建输出流
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);

            //将byte数组写入文件中
            fs.Write(array, 0, array.Length);
            fs.Close();
        }

        /// <summary>
        /// 更新文件逻辑,判定文件是否处于运行状态，关闭并删除后，创建新的文件
        /// </summary>
        private static void outPutFile(Byte[] array, string pathName)
        {
            // 若文件正在运行，则从进程中关闭
            string fileName = System.IO.Path.GetFileName(pathName);
            KillProcess(fileName);

            // 删除原有文件
            if (System.IO.File.Exists(pathName)) System.IO.File.Delete(pathName);

            // 保存新的文件
            SaveFile(array, pathName);
        }

        /// <summary>
        /// 关闭名称为processName的所有进程
        /// </summary>
        public static void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                if (process.MainModule.FileName == processName)
                {
                    process.Kill();
                }
            }
        }

        /// <summary>
        /// 获取应用的当前工作路径
        /// </summary>
        public static String curDir()
        {
            string CurDir = System.AppDomain.CurrentDomain.BaseDirectory;
            return CurDir;
        }
    }
}
