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
    public class MQTTnetStopButton : BackgroundService
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
        public MQTTnetStopButton(ILogger<MQTTnetStopButton> logger, MQTTnetClient mQTTnetClient)
        {
            this._logger = logger;
            _mQTTnetClient = mQTTnetClient;
            this.aTimer.Elapsed += this.OnTimedEvent;
            this.aTimer.Enabled = true;
        }
        readonly Timer aTimer = new Timer(4000);

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
                    var propertiesHeader = new MqttReportESButtonProperties1D
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationESButton1D
                        {
                            eStopButtonsDeviceBaseInfo = new List<EStopButtonsDeviceBaseInfo>(),
                            eStopButtonsDeviceNumber = new List<EStopButtonsDeviceNumber>()
                        }
                    };

                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //设备总数上传
                        if (variable.DeviceType == "EPProperty" && variable.OpcValue == "ST-EP-设备总数")
                        {
                            propertiesHeader.properties.eStopButtonsDeviceAmount = Convert.ToInt16(variable.ComponentProperty);
                        }
                        //设备基础信息上传
                        if (variable.DeviceType == "EPProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.eStopButtonsDeviceBaseInfo.Add(new EStopButtonsDeviceBaseInfo
                            {
                                componentNo = variable.DeviceNumber,
                                productionDate = Helper.TimeHelper.DateTimeToLongS(Convert.ToDateTime(ComponentPropertys[0])).ToString(),
                                manufacturerName = ComponentPropertys[1],
                                deviceSn = "",
                                modelNumber = ""
                            });
                        }
                        //设备种类数量
                        if (variable.DeviceType == "EPProperty" && variable.ComponentPropertyType == "设备种类数量")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.eStopButtonsDeviceNumber.Add(new EStopButtonsDeviceNumber
                            {
                                componentType = ComponentPropertys[0],
                                componentNumber = Convert.ToInt32(ComponentPropertys[1])
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
                    var propertiesHeader = new MqttReportESButtonProperties4S
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationESButton4S
                        {
                            eStopButtonsMasterEStopStatus = new List<EStopButtonsMasterEStopStatus>(),
                            eStopButtonsEStopStatus = new List<EStopButtonsEStopStatus>()
                        }
                    };
                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //设备主控急停状态
                        if (variable.DeviceType == "EPError" && (variable.DeviceNumber == "ST01-MCC-EP" || variable.DeviceNumber == "ST02-MCC-EP"))
                        {
                            string isStop = "1";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value.Contains("1"))
                            {
                                isStop = "0";
                            }
                            propertiesHeader.properties.eStopButtonsMasterEStopStatus.Add(new EStopButtonsMasterEStopStatus
                            {
                                componentNo = variable.DeviceNumber,
                                eStopStatus = isStop
                            });
                        }
                        //设备急停状态
                        if (variable.DeviceType == "EPError" && variable.IType == "Error")
                        {
                            string isStop = "1";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value.Contains("1"))
                            {
                                isStop = "0";
                            }
                            propertiesHeader.properties.eStopButtonsEStopStatus.Add(new EStopButtonsEStopStatus
                            {
                                componentNo = variable.DeviceNumber,
                                componentType = variable.ComponentProperty,
                                componentEstopStatus = isStop
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
