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
    public class MQTTnetInduction : BackgroundService
    {
        public ILogger _logger;
        private MQTTnetClient _mQTTnetClient;
        public bool _uploadEveryday;
        public int _version = JDInfo.Version;
        public string _deviceId = JDInfo.DeviceId;
        public string _companyName = JDInfo.CompanyName;
        public DateTime _crrentTime;
        public DateTime _oldTime = DateTime.Now;
        public int _actionCount;
        readonly Timer aTimer = new Timer(4000);
        public MQTTnetInduction(ILogger<MQTTnetInduction> logger, MQTTnetClient mQTTnetClient)
        {
            this._logger = logger;
            _mQTTnetClient = mQTTnetClient;
            this.aTimer.Elapsed += this.OnTimedEvent;
            this.aTimer.Enabled = true;
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                if (_mQTTnetClient == null || _mQTTnetClient._connectStatus == false)
                {
                    return;
                }
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
                //1天上传一次
                if (ListKye != null && ListKye.Count > 0 && _uploadEveryday && _actionCount == 0)
                {
                    var propertiesHeader = new MqttReportInductionProperties1D
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationInduction1D
                        {
                            inductionDeviceType = new List<InductionDeviceType>(),
                            inductionBaseInfo = new List<InductionBaseInfo>(),
                        }
                    };

                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //设备总数上传
                        if (variable.DeviceType == "InductionProperty" && variable.OpcValue == "ST-ID-设备总数")
                        {
                            propertiesHeader.properties.inductionDeviceAmount = Convert.ToInt16(variable.ComponentProperty);
                        }
                        //设备类型上传
                        if (variable.DeviceType == "InductionProperty" && variable.ComponentPropertyType == "设备类型")
                        {
                            propertiesHeader.properties.inductionDeviceType.Add(new InductionDeviceType
                            {
                                componentNo = variable.DeviceNumber,
                                componentType = variable.ComponentProperty
                            });
                        };
                        //设备基础信息上传
                        if (variable.DeviceType == "InductionProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.inductionBaseInfo.Add(new InductionBaseInfo
                            {
                                componentNo = variable.DeviceNumber,
                                productionDate = Helper.TimeHelper.DateTimeToLongS(Convert.ToDateTime(ComponentPropertys[0])).ToString(),
                                manufacturerName = ComponentPropertys[1],
                                softwareVersion = "",
                                deviceSn = "",
                                modelNumber = ""
                            });

                        }
                    }
                    _uploadEveryday = false;
                    _actionCount = 1;
                    var machinePropertiesJsonFirst = JsonConvert.SerializeObject(propertiesHeader);
                    var machinePropertiesMessageFirst = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + _deviceId + "/properties/post")
                                    .WithPayload(machinePropertiesJsonFirst)
                                    .Build();
                    _mQTTnetClient.managedClient.PublishAsync(machinePropertiesMessageFirst, CancellationToken.None);
                }
                //4S上传一次
                if (ListKye != null && ListKye.Count > 0)
                {
                    var propertiesHeader = new MqttReportInductionProperties4S
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationInduction4S
                        {
                            inductionFaultStatus = new List<InductionFaultStatus>(),
                            inductionControlMode = new List<InductionControlMode>(),
                            inductionScannerStatus = new List<InductionScannerStatus>(),
                            inductionStartStopStatus = new List<InductionStartStopStatus>(),
                            inductionInduceCapacity = new List<InductionInduceCapacity>(),
                            inductionEstopStatus = new List<InductionEstopStatus>()
                        }
                    };
                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //上包机设备是否故障上传
                        if (variable.DeviceType == "InductionError")
                        {
                            var haveError = "0";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value.Contains("1"))
                            {
                                haveError = "1";
                            }
                            propertiesHeader.properties.inductionFaultStatus.Add(new InductionFaultStatus
                            {
                                componentNo = variable.DeviceNumber,
                                componentFaultStatus = haveError
                            });
                        }
                        //上包机设备控制模式
                        if (variable.DeviceType == "InductionProperty" && variable.ComponentPropertyType == "设备控制模式")
                        {
                            propertiesHeader.properties.inductionControlMode.Add(new InductionControlMode
                            {
                                componentNo = variable.DeviceNumber,
                                componentControlMmode = variable.ComponentProperty
                            });
                        }
                        //上包机手动扫码状态
                        if (variable.DeviceType == "InductionProperty" && variable.ComponentPropertyType == "上包机手动扫码状态")
                        {
                            var componentHandheldStatus = "0";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value == "2")//人工扫描
                            {
                                componentHandheldStatus = "1";
                            }
                            propertiesHeader.properties.inductionScannerStatus.Add(new InductionScannerStatus
                            {
                                componentNo = variable.DeviceNumber,
                                componentHandheldStatus = componentHandheldStatus
                            });
                        }
                        //上包机启停状态
                        if (variable.DeviceType == "InductionProperty" && variable.ComponentPropertyType == "启停状态")
                        {
                            var componentStartStopStatus = "3";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value == "1")
                            {
                                componentStartStopStatus = "1";
                            }
                            propertiesHeader.properties.inductionStartStopStatus.Add(new InductionStartStopStatus
                            {
                                componentNo = variable.DeviceNumber,
                                componentStartStopStatus = componentStartStopStatus
                            });
                        }
                        //上包机供件效率
                        if (variable.DeviceType == "InductionProperty" && variable.ComponentPropertyType == "供件效率")
                        {
                            propertiesHeader.properties.inductionInduceCapacity.Add(new InductionInduceCapacity
                            {
                                componentNo = variable.DeviceNumber,
                                induceCapacity = Convert.ToInt32(variable.Value==""?"1": variable.Value)
                            });
                        }
                        //上包机设备急停状态
                        if (variable.DeviceType == "EPError" && variable.DeviceNumber.Contains("ID"))
                        {
                            string isStop = "1";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value == "1")
                            {
                                isStop = "0";
                            }
                            propertiesHeader.properties.inductionEstopStatus.Add(new InductionEstopStatus
                            {
                                componentNo = variable.ComponentPropertyType,
                                eStopStatus = isStop
                            });
                        }
                    }
                    var machinePropertiesJsonFirst = JsonConvert.SerializeObject(propertiesHeader);
                    var machinePropertiesMessageFirst = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + _deviceId + "/properties/post")
                                    .WithPayload(machinePropertiesJsonFirst)
                                    .Build();
                    _mQTTnetClient.managedClient.PublishAsync(machinePropertiesMessageFirst, CancellationToken.None);




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
    }
}
