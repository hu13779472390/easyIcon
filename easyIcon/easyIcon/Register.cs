using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace easyIcon
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Register_Load(object sender, EventArgs e)
        {
            label_MachineSerial.Text = MachineInfo.MachineSerial(); // 显示机器码
            setPicePicture(Price());    // 设置注册价格
        }

        // 设置注册价格
        private void setPicePicture(int price)
        {
            if (price == 1) pictureBox1.Image = Properties.Resources.price_1;
            else if (price == 2) pictureBox1.Image = Properties.Resources.price_2;
            else if (price == 3) pictureBox1.Image = Properties.Resources.price_3;
            else if (price == 10) pictureBox1.Image = Properties.Resources.price_10;
            else if (price == 20) pictureBox1.Image = Properties.Resources.price_20;
        }

        // 软件注册，记录注册码到注册表中
        private void button1_Click(object sender, EventArgs e)
        {
            string serial = textBox_SerialNum.Text.Trim();
            serial = MachineInfo.format(serial);
            if(serial.Equals(MachineInfo.RegSerial()))
            {
                mainForm.F.RegistrySave(@"Scimence\easyIcon\Set", "Serial", serial);
                this.Close();   
                MessageBox.Show("恭喜，您已成功注册！");
            }
            else MessageBox.Show("抱歉，注册码无效！");
        }


        // 判断用户是否已经注册
        public static bool IsRegUser()
        {
            // 联网控制开关状态为，不需要进行注册
            bool containsSerial = mainForm.F.RegistryCotains(@"Scimence\easyIcon\Set", "Serial");
            //mainForm.F.RegistryRemove(@"Scimence\easyIcon\Set", "Serial");
            if (!containsSerial && !needRegister()) return true;   

            // 优先判断本地注册表数据
            string serial = mainForm.F.RegistryStrValue(@"Scimence\easyIcon\Set", "Serial");
            if (!serial.Equals("") && serial.Equals(MachineInfo.RegSerial())) return true;

            // 判断联网数据中，是否有本机的注册码
            loadOnlineSerial();
            if(!OnlineSerial.Equals("") && OnlineSerial.Equals(MachineInfo.RegSerial()))
            {
                mainForm.F.RegistrySave(@"Scimence\easyIcon\Set", "Serial", OnlineSerial);
                MessageBox.Show("恭喜，软件作者已通过联网方式，成功为您注册！");
                return true;
            }
            else return false;
        }


        # region 获取在线设置数据

        private static string SettingData;      // 在线设置信息
        public static void loadSettingData()
        {
            // 获取设置数据
            if (SettingData == null)
            {
                string url = "https://git.oschina.net/scimence/easyIcon/raw/master/files/settingPage.txt";
                SettingData = mainForm.F.getWebData(url);
                if (!SettingData.Equals("")) SettingData = getNodeData(SettingData, "scimence", false);
            }
        }

        private static string OnlineSerial;     // 在线注册码
        public static void loadOnlineSerial()
        {
            // 联网获取本机的注册码
            if (OnlineSerial == null)
            {
                string url = "https://git.oschina.net/scimence/easyIcon/raw/master/files/onlineSerial.txt";
                OnlineSerial = mainForm.F.getWebData(url);
                if (!OnlineSerial.Equals("")) OnlineSerial = getNodeData(OnlineSerial, "scimence", false);
                if (!OnlineSerial.Equals("")) OnlineSerial = getNodeData(OnlineSerial, MachineInfo.MachineSerial(), true);
            }
        }

        // 根据网页控制开关，判定是否需要注册
        public static bool needRegister()
        {
            loadSettingData();  // 载入设置数据
            string NeedToRegister = getNodeData(SettingData, "NeedToRegister", true);
            return !NeedToRegister.Equals("false");

            //string RegisterPrice = getNodeData(SettingData, "RegisterPrice", false);
        }

        // 根据网页控制开关，判定判定注册价格
        public static int Price()
        {
            loadSettingData();  // 载入设置数据
            try
            {
                string RegisterPrice = getNodeData(SettingData, "RegisterPrice", true);
                return Int32.Parse(RegisterPrice);
            }
            catch (Exception) { return 5; }
        }

        // 根据网页设置，获取当前推荐显示的url
        public static string RecommendUrl()
        {
            loadSettingData();  // 载入设置数据
            try
            {
                string recommendUrl = getNodeData(SettingData, "RecommendUrl", true);
                return recommendUrl;
                //return toRealUrl(recommendUrl);
            }
            catch (Exception) { return ""; }
        }

        // 从自定义格式的数据data中，获取nodeName对应的节点数据
        //p>scimence(&#x000A;NeedToRegister(false)NeedToRegister&#x000A;RegisterPrice(1)RegisterPrice&#x000A;)scimence</p>&#x000A;</div>
        // NeedToRegister(false)&#x000A;RegisterPrice(1)   finalNode的数据格式
        private static string getNodeData(string data, string nodeName, bool finalNode)
        {
            try
            {
                string S = nodeName + "(", E = ")" + (finalNode ? "" : nodeName);
                int indexS = data.IndexOf(S) + S.Length;
                int indexE = data.IndexOf(E, indexS);

                return data.Substring(indexS, indexE - indexS);
            }
            catch (Exception) { return data; }
        }

        //// 转化自定义格式数据串，为url
        //// "http://store.cocos.com/stuff/show/178765.html"
        //public static string toRealUrl(string urlData)
        //{
        //    string url = urlData.Replace("|", "/").Replace(";", ":").Replace(",", ".");
        //    return url;
        //}

        //// 转化url格式串，为自定义格式
        //// "http;||store,cocos,com|stuff|show|178765,html"
        //public static string formatUrl(string url)
        //{
        //    string data = url.Replace("/", "|").Replace(":", ";").Replace(".", ",");
        //    return data;
        //}

        # endregion

    }
}