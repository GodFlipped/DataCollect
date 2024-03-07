using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Application.Helper
{
    public static class SorterMachineError
    {
        /// <summary>
        /// 分拣机信息转换
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> SorterError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "199";
            switch (index)
            {
                case "3":
                    error = "禁用格口卡塞信号";
                    errorLevel = "1";
                    errorNo = "103";
                    break;
                case "4":
                    error = "禁止分拣机启动";
                    errorLevel = "1";
                    errorNo = "104";
                    break;
                case "5":
                    error = "开机后速度尚未达标";
                    errorLevel = "2";
                    errorNo = "105";
                    break;
                case "6":
                    error = "开机后1号小车尚未检测到";
                    errorLevel = "2";
                    errorNo = "106";
                    break;
                case "7":
                    error = "开机号尚未检测到同步信号";
                    errorLevel = "2";
                    errorNo = "107";
                    break;
                case "8":
                    error = "运行时同步信号丢失";
                    errorLevel = "3";
                    errorNo = "108";
                    break;
                case "9":
                    error = "运行时速度异常";
                    errorLevel = "1";
                    errorNo = "109";
                    break;
                case "10":
                    error = "分拣机停机";
                    errorLevel = "1";
                    errorNo = "110";
                    break;
                case "11":
                    error = "测试模式激活";
                    errorLevel = "1";
                    errorNo = "111";
                    break;
                case "12":
                    error = "分拣机启动失败";
                    errorLevel = "3";
                    errorNo = "112";
                    break;
                case "14":
                    error = "运行时1号小车检测信号丢失";
                    errorLevel = "3";
                    errorNo = "114";
                    break;
                case "15":
                    error = "48V 直流总线断路器1触发";
                    errorLevel = "3";
                    break;
                case "16":
                    error = "48V 直流总线断路器2触发";
                    errorLevel = "3";
                    errorNo = "116";
                    break;
                case "17":
                    error = "48V 直流总线断路器3触发";
                    errorLevel = "3";
                    errorNo = "117";
                    break;
                case "18":
                    error = "48V 直流总线断路器4触发";
                    errorLevel = "3";
                    errorNo = "118";
                    break;
                case "19":
                    error = "48V 直流总线断路器5触发";
                    errorLevel = "3";
                    errorNo = "119";
                    break;
                case "20":
                    error = "48V 直流总线断路器6触发";
                    errorLevel = "3";
                    errorNo = "120";
                    break;
                case "21":
                    error = "48V 直流总线断路器7触发";
                    errorLevel = "3";
                    errorNo = "121";
                    break;
                case "22":
                    error = "48V 直流总线断路器8触发";
                    errorLevel = "3";
                    break;
                case "23":
                    error = "48V 直流总线断路器9触发";
                    errorLevel = "3";
                    errorNo = "123";
                    break;
                case "24":
                    error = "48V 直流总线断路器10触发";
                    errorLevel = "3";
                    errorNo = "124";
                    break;
                case "25":
                    error = "48V 直流总线断路器11触发";
                    errorLevel = "3";
                    errorNo = "125";
                    break;
                case "26":
                    error = "48V 直流总线断路器12触发";
                    errorLevel = "3";
                    errorNo = "126";
                    break;
                case "27":
                    error = "48V 直流总线断路器13触发";
                    errorLevel = "3";
                    errorNo = "127";
                    break;
                case "28":
                    error = "48V 直流总线断路器14触发";
                    errorLevel = "3";
                    errorNo = "128";
                    break;
                case "29":
                    error = "48V 直流总线断路器15触发";
                    errorLevel = "3";
                    errorNo = "129";
                    break;
                case "30":
                    error = "48V 直流总线断路器16触发";
                    errorLevel = "3";
                    errorNo = "130";
                    break;
                case "31":
                    error = "空盘检测传感器1堵塞";
                    errorLevel = "3";
                    errorNo = "131";
                    break;
                case "32":
                    error = "空盘检测传感器2堵塞";
                    errorLevel = "3";
                    errorNo = "132";
                    break;
                case "33":
                    error = "IOB1触发";
                    errorLevel = "3";
                    errorNo = "133";
                    break;
                case "34":
                    error = "IOB2触发";
                    errorLevel = "3";
                    errorNo = "134";
                    break;
                case "35":
                    error = "空盘检测传感器3堵塞";
                    errorLevel = "3";
                    errorNo = "135";
                    break;
                case "36":
                    error = "空盘检测传感器4堵塞";
                    errorLevel = "3";
                    errorNo = "136";
                    break;
                case "48":
                    error = "直线电机风扇过载";
                    errorLevel = "3";
                    errorNo = "148";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 导入台信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> InductionError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "399";
            switch (index)
            {
                case "1":
                    error = "被SCADA禁用";
                    errorLevel = "1";
                    errorNo = "301";
                    break;
                case "2":
                    error = "邮件超重";
                    errorLevel = "1";
                    errorNo = "302";
                    break;
                case "3":
                    error = "邮件超高";
                    errorLevel = "1";
                    errorNo = "303";
                    break;
                case "4":
                    error = "邮件超宽";
                    errorLevel = "1";
                    errorNo = "304";
                    break;
                case "5":
                    error = "邮件超长";
                    errorLevel = "1";
                    errorNo = "305";
                    break;
                case "6":
                    error = "尺寸测量数据出错";
                    errorLevel = "2";
                    errorNo = "306";
                    break;
                case "8":
                    error = "光幕堵塞";
                    errorLevel = "3";
                    errorNo = "308";
                    break;
                case "9":
                    error = "维护模式激活";
                    errorLevel = "2";
                    errorNo = "309";
                    break;
                case "10":
                    error = "邮件间距过小";
                    errorLevel = "1";
                    errorNo = "310";
                    break;
                case "11":
                    error = "同步信号丢失";
                    errorLevel = "3";
                    errorNo = "311";
                    break;
                case "12":
                    error = "1号电滚筒驱动故障";
                    errorLevel = "3";
                    errorNo = "312";
                    break;
                case "13":
                    error = "2号变频器故障或告警";
                    errorLevel = "3";
                    errorNo = "313";
                    break;
                case "14":
                    error = "3号变频器故障或告警";
                    errorLevel = "3";
                    errorNo = "314";
                    break;
                case "19":
                    error = "动态称错误";
                    errorLevel = "3";
                    errorNo = "319";
                    break;
                case "20":
                    error = "称重数据错误 （超轻）";
                    errorLevel = "1";
                    errorNo = "320";
                    break;
                case "21":
                    error = "4号变频器故障或告警";
                    errorLevel = "3";
                    errorNo = "321";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 环扫扫描仪提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> ScannerError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "299";
            switch (index)
            {
                case "1":
                    error = "与PLC的TCP/IP连接中断";
                    errorLevel = "3";
                    errorNo = "201";
                    break;
                case "4":
                    error = "1号相机故障";
                    errorLevel = "3";
                    errorNo = "204";
                    break;
                case "5":
                    error = "2号相机故障";
                    errorLevel = "3";
                    errorNo = "205";
                    break;
                case "6":
                    error = "3号相机故障";
                    errorLevel = "3";
                    errorNo = "206";
                    break;
                case "7":
                    error = "4号相机故障";
                    errorLevel = "3";
                    errorNo = "207";
                    break;
                case "8":
                    error = "5号相机故障";
                    errorLevel = "3";
                    errorNo = "208";
                    break;
                case "9":
                    error = "6号相机故障";
                    errorLevel = "3";
                    errorNo = "209";
                    break;
                case "10":
                    error = "7号相机故障";
                    errorLevel = "3";
                    errorNo = "210";
                    break;
                case "11":
                    error = "8号相机故障";
                    errorLevel = "3";
                    errorNo = "211";
                    break;
                case "12":
                    error = "9号相机故障";
                    errorLevel = "3";
                    errorNo = "212";
                    break;
                case "13":
                    error = "10号相机故障";
                    errorLevel = "3";
                    errorNo = "213";
                    break;
                case "14":
                    error = "11号相机故障";
                    errorLevel = "3";
                    break;
                case "15":
                    error = "12号相机故障";
                    errorLevel = "3";
                    errorNo = "215";
                    break;
                case "16":
                    error = "13号相机故障";
                    errorLevel = "3";
                    errorNo = "216";
                    break;
                case "17":
                    error = "识读率过低";
                    errorLevel = "2";
                    errorNo = "217";
                    break;
                case "18":
                    error = "14号相机故障";
                    errorLevel = "3";
                    errorNo = "218";
                    break;
                case "19":
                    error = "15号相机故障";
                    errorLevel = "3";
                    errorNo = "219";
                    break;
                case "20":
                    error = "16号相机故障";
                    errorLevel = "3";
                    errorNo = "220";
                    break;
                case "21":
                    error = "17号相机故障";
                    errorLevel = "3";
                    errorNo = "221";
                    break;
                case "22":
                    error = "18号相机故障";
                    errorLevel = "3";
                    errorNo = "222";
                    break;
                case "23":
                    error = "19号相机故障";
                    errorLevel = "3";
                    errorNo = "223";
                    break;
                case "24":
                    error = "20号相机故障";
                    errorLevel = "3";
                    errorNo = "224";
                    break;
                case "25":
                    error = "21号相机故障";
                    errorLevel = "3";
                    errorNo = "225";
                    break;
                case "26":
                    error = "22号相机故障";
                    errorLevel = "3";
                    errorNo = "226";
                    break;
                case "27":
                    error = "23号相机故障";
                    errorLevel = "3";
                    errorNo = "227";
                    break;
                case "28":
                    error = "24号相机故障";
                    errorLevel = "3";
                    errorNo = "228";
                    break;
                case "29":
                    error = "25号相机故障";
                    errorLevel = "3";
                    errorNo = "229";
                    break;
                case "30":
                    error = "26号相机故障";
                    errorLevel = "3";
                    errorNo = "230";
                    break;
                case "31":
                    error = "27号相机故障";
                    errorLevel = "3";
                    errorNo = "231";
                    break;
                case "32":
                    error = "28号相机故障";
                    errorLevel = "3";
                    errorNo = "232";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 格口信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> ChutesError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "499";
            switch (index)
            {
                case "1":
                    error = "被SCADA禁用";
                    errorLevel = "1";
                    errorNo = "401";
                    break;
                case "2":
                    error = "滑槽满";
                    errorLevel = "1";
                    errorNo = "402";
                    break;
                case "7":
                    error = "满袋检测触发封锁";
                    errorLevel = "1";
                    errorNo = "407";
                    break;
                case "8":
                    error = "滑槽口卡塞";
                    errorLevel = "2";
                    errorNo = "408";
                    break;
                case "10":
                    error = "被按钮开关禁用";
                    errorLevel = "1";
                    errorNo = "410";
                    break;
                case "14":
                    error = "来自st01的格口封锁";
                    errorLevel = "1";
                    errorNo = "414";
                    break;
                case "16":
                    error = "挡板抽出";
                    errorLevel = "1";
                    errorNo = "416";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 小车信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> CarriersError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "599";
            switch (index)
            {
                case "1":
                    error = "被SCADA禁用";
                    errorLevel = "1";
                    errorNo = "501";
                    break;
                case "2":
                    error = "禁止授权IBB";
                    errorLevel = "1";
                    errorNo = "502";
                    break;
                case "3":
                    error = "卸载验证失败超过最大次数导致小车禁用";
                    errorLevel = "2";
                    errorNo = "503";
                    break;
                case "4":
                    error = "通讯错误";
                    errorLevel = "3";
                    errorNo = "504";
                    break;
                case "5":
                    error = "电机霍尔错误";
                    errorLevel = "3";
                    errorNo = "505";
                    break;
                case "6":
                    error = "电机过流保护";
                    errorLevel = "3";
                    errorNo = "506";
                    break;
                case "8":
                    error = "小车卸载条件不满足无法进行卸载";
                    errorLevel = "2";
                    errorNo = "508";
                    break;
                case "10":
                    error = "小车卸载验证失败";
                    errorLevel = "2";
                    errorNo = "510";
                    break;
                case "12":
                    error = "IBB触发";
                    errorLevel = "2";
                    errorNo = "512";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 直线电机信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> MSError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "699";
            switch (index)
            {
                case "3":
                    error = "防撞开关触发";
                    errorLevel = "3";
                    errorNo = "603";
                    break;
                case "4":
                    error = "温度过高";
                    errorLevel = "3";
                    errorNo = "604";
                    break;
                case "5":
                    error = "过流保护";
                    errorLevel = "3";
                    errorNo = "605";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 急停按钮信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> EPError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "799";
            switch (index)
            {
                case "1":
                    error = "急停触发";
                    errorLevel = "2";
                    errorNo = "701";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 打印机信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string, string> PrinterError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorCode = "899";
            switch (index)
            {
                case "3":
                    error = "打印机断开连接";
                    errorLevel = "2";
                    errorCode = "813";
                    break;
                case "6":
                    error = "介质用尽";
                    errorLevel = "1";
                    errorCode = "811";
                    break;
                case "7":
                    error = "打印头开启";
                    errorLevel = "1";
                    errorCode = "814";
                    break;
                case "9":
                    error = "打印头温度过高";
                    errorLevel = "1";
                    errorCode = "821";
                    break;
                case "8":
                    error = "打印头故障";
                    errorLevel = "1";
                    errorCode = "824";
                    break;

            }
            return new Tuple<string, string, string>(error, errorLevel, errorCode);
        }
        /// <summary>
        /// 灰度相机信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> GreyDetectoreError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "999";
            switch (index)
            {
                case "1":
                    error = "灰度检测仪故障";
                    errorLevel = "3";
                    errorNo = "901";
                    break;
                case "2":
                    error = "与PLC的TCP/IP连接中断";
                    errorLevel = "2";
                    errorNo = "902";
                    break;
                case "3":
                    error = "通讯无响应";
                    errorLevel = "2";
                    errorNo = "903";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }
        /// <summary>
        /// 48v供电柜信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> SDError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "1099";
            switch (index)
            {
                case "1":
                    error = "故障";
                    errorLevel = "3";
                    errorNo = "1001";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel, errorNo);
        }
        /// <summary>
        /// 配电柜信息提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static new Tuple<string, string,string> PSError(string index)
        {
            string error = "未知错误";
            string errorLevel = "2";//1提示，2警告，3故障
            string errorNo = "1199";
            switch (index)
            {
                case "15":
                    error = "相序保护脱扣";
                    errorLevel = "3";
                    errorNo = "1115";
                    break;
            }
            return new Tuple<string, string,string>(error, errorLevel,errorNo);
        }

        public static string ChutesStateChange(string val)
        {
            string state = "1";
            var splitStr = val.ToCharArray();
            for (int i = 0; i < splitStr.Length; i++)
            {
                if (splitStr[i]=='1')
                {
                    switch (i)
                    {
                        case 0:
                            state = "2";
                            break;
                        case 1:
                            state = "3";
                            break;
                        case 6:
                            state = "2";
                            break;
                        case 7:
                            state = "4";
                            break;
                        case 9:
                            state = "2";
                            break;
                        case 13:
                            state = "2";
                            break;
                    }
                }
            }                      
            return state;
        }
    }

}
