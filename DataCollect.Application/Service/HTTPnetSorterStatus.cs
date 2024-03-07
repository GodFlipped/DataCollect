using DataCollect.Application.Consts;
using DataCollect.Application.Helper;
using DataCollect.Application.Service.OpcUa.Dtos;
using DataCollect.Core.Configure;
using DataCollect.Interface.MQTTnet;
using DataCollect.Interface.MQTTnet.Models;
using Kengic.Was.Connector.Common;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
namespace DataCollect.Application.Service
{
    public class HTTPnetSorterStatus : BackgroundService
    {
        public ILogger _logger;
        //private MQTTnetClient _mQTTnetClient;
        public bool _uploadEveryday;
        public int _version = JDInfo.Version;
        public string _deviceId = JDInfo.DeviceId;
        public string _companyName = JDInfo.CompanyName;
        public DateTime _crrentTime;
        public DateTime _oldTime = DateTime.Now;
        public int _actionCount;
        readonly Timer aTimer = new Timer(5000);

        public HTTPnetSorterStatus(ILogger<HTTPnetSorterStatus> logger)
        {
            this._logger = logger;
            this.aTimer.Elapsed += this.OnTimedEvent;
            this.aTimer.Enabled = true;
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            try
            {
                
                //获取redis中所有keys的值
                var ListKye = RedisConn.Instance.rds.Get<List<VariableKeys>>("Keys");
                var timeToLong10 = Helper.TimeHelper.DateTimeToLongS10(DateTime.Now);
                _crrentTime = DateTime.Now;
                _uploadEveryday = true;
                if (_actionCount == 1 && _oldTime.Day != _crrentTime.Day)
                {
                    _uploadEveryday = true;
                    _actionCount = 0;
                    _oldTime = DateTime.Now;
                }
               
                //5S上传一次
                if (ListKye is {Count: > 0})
                {
                    //var propertiesHeader = new MqttReportChutesProperties4S
                    //{

                    //    version = _version,
                    //    messageId = Guid.NewGuid().ToString("N"),
                    //    deviceId = _deviceId,
                    //    timestamp = timeToLong10,
                    //    properties = new MachineBasicInformationChutes4S
                    //    {
                    //        chuteFaultStatus = new List<ChuteFaultStatus>(),
                    //        chuteDeviceStatus = new List<ChuteDeviceStatus>()
                    //    }
                    //};

                    // 分拣状态模型
                    var sorterStatus = new SorterStatus()
                    {
                        DwsList = new List<Base>(),
                        EmergencyStopList = new List<Base>(),
                        MotorList = new List<Base>(),
                        TrolleyList = new List<Trolley>(),
                        GridList = new List<Grid>(),
                        OverallStatistics = new OverallStatistics()

                    };

                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);

                        // dws 供包台状态列表
                        if (variable.DeviceType == "InductionProperty" && variable.ComponentPropertyType == "启停状态")
                        {
                            var deviceNumber = variable.DeviceNumber;
                            var value = variable.Value;
                            var dwsBase = new Base();
                            //todo 为dwsBase赋值 以下设备同理
                            sorterStatus.DwsList.Add(dwsBase);
                        }

                        // emergency 急停状态列表
                        if (variable.DeviceType == "EPProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var deviceNumber = variable.DeviceNumber;
                            var value = variable.Value;
                        }

                        // motor 电机
                        if (variable.DeviceType == "MSProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var deviceNumber = variable.DeviceNumber;
                            var value = variable.Value;
                        }

                        // trolley 分拣小车状态列表
                        if (variable.DeviceType == "CarriersProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var deviceNumber = variable.DeviceNumber;
                            var value = variable.Value;
                        }

                        // grid 分拣格口状态列表
                        if (variable.DeviceType == "ChutesProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var deviceNumber = variable.DeviceNumber;
                            var value = variable.Value;
                        }

                        
                        var response = HttpClientSingleton.PostAsync("",sorterStatus);

                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                //错误处理
                _logger.LogError("设备故障定时执行失败：" + ex.ToString());
            }


        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await new TaskFactory().StartNew(() =>
            {
            });

        }

        #region 分拣机状态

        /// <summary>
        /// 分拣机状态
        /// </summary>
        public class SorterStatus
        {
            /// <summary>
            /// 供包台状态列表
            /// </summary>
            [JsonProperty("dwsList")]
            public List<Base> DwsList { get; set; }

            /// <summary>
            /// 急停状态列表
            /// </summary>
            [JsonProperty("emergencyStopList")]
            public List<Base> EmergencyStopList { get; set; }

            /// <summary>
            /// 电机状态列表
            /// </summary>
            [JsonProperty("motorList")]
            public List<Base> MotorList { get; set; }

            /// <summary>
            /// 分拣小车状态列表
            /// </summary>
            [JsonProperty("trolleyList")]
            public List<Trolley> TrolleyList { get; set; }

            /// <summary>
            /// 分拣格口状态列表
            /// </summary>
            [JsonProperty("gridList")]
            public List<Grid> GridList { get; set; }

            /// <summary>
            /// 设备实时运行总体数据
            /// </summary>
            [JsonProperty("overallStatistics")]
            public OverallStatistics OverallStatistics { get; set; }
        }


        public class Base
        {
            /// <summary>
            /// 编号
            /// </summary>
            [JsonProperty("index")]
            public int Index { get; set; }

            /// <summary>
            /// 状态
            /// </summary>
            [JsonProperty("status")]
            public int Status { get; set; }
        }

        public class Trolley : Grid
        {
            /// <summary>
            /// 分拣小车所处轨道位置，从 1 开始
            /// </summary>
            [JsonProperty("position")]
            public int Position { get; set; }

            /// <summary>
            /// 小车故障状态，取值见“表 3. 小车故障状态列表”
            /// </summary>
            [JsonProperty("faultStatus")]
            public int FaultStatus { get; set; }

            /// <summary>
            /// 分拣小车当前分拣任务的目的格口，从 1 开始
            /// </summary>
            [JsonProperty("gridIndex")]
            public int GridIndex { get; set; }
        }

        public class Grid
        {
            /// <summary>
            /// 分拣格口编号，从 1 开始
            /// </summary>
            [JsonProperty("index")]
            public int Index { get; set; }

            /// <summary>
            /// 格口分拣状态，取值见“表 4. 格口分拣状态列表
            /// </summary>
            [JsonProperty("sortStatus")]
            public int SortStatus { get; set; }
        }

        /// <summary>
        /// 设备实时运行总体数据
        /// </summary>
        public class OverallStatistics
        {
            /// <summary>
            /// 主线状态，分拣机主线运行状态，0：停止、1：运行、2：维护
            /// </summary>
            [JsonProperty("mainlineStatus")]
            public int MainlineStatus { get; set; }

            /// <summary>
            /// 主线速度，主线实际运行速度，单位：m/s
            /// </summary>
            [JsonProperty("mainlineSpeed")]
            public int MainlineSpeed { get; set; }

            /// <summary>
            /// 电机频率，电机实际频率，单位：Hz
            /// </summary>
            [JsonProperty("motorFrequency")]
            public double MotorFrequency { get; set; }

            /// <summary>
            /// 小车占用率(百分比)，（1 -（无货小车 数量÷总小车数量））×100%，四舍五入保留两位小数
            /// </summary>
            [JsonProperty("trolleyOccupancy")]
            public double TrolleyOccupancy { get; set; }

            /// <summary>
            /// WCS 启动时间，WCS 最近一次启动时间，格式：年-月-日 时:分:秒
            /// </summary>
            [JsonProperty("wcsStartTime")]
            public string WcsStartTime { get; set; }

            /// <summary>
            /// 主线启动时间，主线本次运行启动时间，格式：年-月-日 时:分:秒
            /// </summary>
            [JsonProperty("mainlineStartTime")]
            public string MainlineStartTime { get; set; }

            /// <summary>
            /// 主线停止时间，主线本次运行停止时间，格式：年-月-日 时:分:秒， 仅主线状态是停止时有值
            /// </summary>
            [JsonProperty("mainlineStopTime")]
            public string MainlineStopTime { get; set; }

            /// <summary>
            /// 主线运行总时长，∑WCS最近一次启动以来每段主线启动到停止的时长,格式：xx 时 xx 分 xx 秒
            /// </summary>
            [JsonProperty("mainlineWorkTime")]
            public string MainlineWorkTime { get; set; }

            /// <summary>
            /// 分拣总量，WCS最近一次启动后分拣总数量
            /// </summary>
            [JsonProperty("totalSortingCount")]
            public long TotalSortingCount { get; set; }

            /// <summary>
            /// 正常读码数量，WCS 最近一次启动后正常读码数量
            /// </summary>
            [JsonProperty("recognitionSuccessCount")]
            public long RecognitionSuccessCount { get; set; }

            /// <summary>
            /// 未识别数量，分拣总量-正常读码数量
            /// </summary>
            [JsonProperty("recognitionFailureCount")]
            public long RecognitionFailureCount { get; set; }

            /// <summary>
            /// 补码数量，使用人工扫描枪扫码供包 数量
            /// </summary>
            [JsonProperty("manualScanCount")]
            public long ManualScanCount { get; set; }


            /// <summary>
            /// 识别成功率(百分比)，（正常读码数量÷分拣总量）×100%，四舍五入保留两位小数
            /// </summary>
            [JsonProperty("recognitionSuccessRate")]
            public double RecognitionSuccessRate { get; set; }

            /// <summary>
            /// 补码占比率(百分比)，（补码数量÷分拣总量）×100%，保留两位小数
            /// </summary>
            [JsonProperty("manualScanRate")]
            public double ManualScanRate { get; set; }

            /// <summary>
            /// 分拣效率，分拣总量÷实际分拣时长（线体运行时间中扣除分拣记录间隔超过 30 分钟的时长），四舍五入保留0 位小数
            /// </summary>
            [JsonProperty("sortingEfficiency")]
            public double SortingEfficiency { get; set; }
        }

        public enum CarSortingStatusEnum
        {
            无故障 = 0,  // 无故障
            停止 = 1,    // 未开始运行
            禁用 = 2,   // 系统禁用小车
            占用 = 3,   // 应空盘但实际有货
            无货 = 4,   // 正常无货
            超边 = 5,  // 有货但超边
            空盘 = 6,  // 应有货但实际空盘
            有分拣口 = 7, // 有货且有分拣口
            超无分拣口边 = 8, // 有货但无分拣口
        }
        #endregion

    }
}
