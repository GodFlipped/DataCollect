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
    public class MQTTnetCarriers : BackgroundService
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

        public MQTTnetCarriers(ILogger<MQTTnetCarriers> logger, MQTTnetClient mQTTnetClient)
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
                    var propertiesHeader = new MqttReportCarriersProperties1D
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationCarriers1D
                        {
                            carDeviceBaseInfo = new List<CarDeviceBaseInfo>()
                        }
                    };

                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //设备总数上传
                        if (variable.DeviceType == "CarriersProperty" && variable.OpcValue == "ST-Carriers-设备总数")
                        {
                            propertiesHeader.properties.carDeviceAmount = Convert.ToInt16(variable.ComponentProperty);
                        }
                        //小车节距
                        if (variable.DeviceType == "CarriersProperty" && variable.OpcValue == "ST-Carriers-小车节距")
                        {
                            propertiesHeader.properties.carLength = Convert.ToInt16(variable.ComponentProperty);
                        }
                        //带面宽度
                        if (variable.DeviceType == "CarriersProperty" && variable.OpcValue == "ST-Carriers-带面宽度")
                        {
                            propertiesHeader.properties.carBeltWidth = Convert.ToInt16(variable.ComponentProperty);
                        }
                        //带面长度
                        if (variable.DeviceType == "CarriersProperty" && variable.OpcValue == "ST-Carriers-带面长度")
                        {
                            propertiesHeader.properties.carBeltLength = Convert.ToInt16(variable.ComponentProperty);
                        }
                        //设备基础信息上传
                        if (variable.DeviceType == "CarriersProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.carDeviceBaseInfo.Add(new CarDeviceBaseInfo
                            {
                                componentNo = variable.DeviceNumber,
                                productionDate = Helper.TimeHelper.DateTimeToLongS(Convert.ToDateTime(ComponentPropertys[0])).ToString(),
                                manufacturerName = ComponentPropertys[1],
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
                    var propertiesHeader = new MqttReportCarriersProperties4S
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationCarriers4S
                        {
                            carFaultStatus = new List<CarFaultStatus>(),
                            carDeviceSerialNumber = new List<CarDeviceSerialNumber>(),
                            carDeviceRunStatus = new List<CarDeviceRunStatus>()
                        }
                    };
                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //设备故障状态上传
                        if (variable.DeviceType == "CarriersError")
                        {
                            var haveError = "0";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value.Contains("1"))
                            {
                                haveError = "1";
                            }
                            propertiesHeader.properties.carFaultStatus.Add(new CarFaultStatus
                            {
                                componentNo = variable.DeviceNumber,
                                componentFaultStatus = haveError
                            });
                        }
                        //设备编号
                        if (variable.DeviceType == "CarriersProperty" && variable.ComponentPropertyType == "小车类型")
                        {
                            var carriersType = variable.ComponentProperty.Split(";");
                            propertiesHeader.properties.carDeviceSerialNumber.Add(new CarDeviceSerialNumber
                            {
                                componentType = carriersType[0],
                                componentNumber = Convert.ToInt32(carriersType[1]),
                                componentNoList = carriersType[2]
                            });
                        }
                        //设备状态
                        if (variable.DeviceType == "CarriersProperty" && variable.ComponentPropertyType == "小车状态")
                        {
                            var carriers = "4";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value == "1")
                            {
                                carriers = "3";
                            }
                            propertiesHeader.properties.carDeviceRunStatus.Add(new CarDeviceRunStatus
                            {
                                componentNo = variable.DeviceNumber,
                                componentStatus = carriers,
                            });
                        }
                    }
                    var machinePropertiesJsonFirst = JsonConvert.SerializeObject(propertiesHeader);
                    var machinePropertiesMessageFirst = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + _deviceId + "/properties/post")
                                    .WithPayload(machinePropertiesJsonFirst)
                                    .Build();
                    _mQTTnetClient.managedClient.PublishAsync(machinePropertiesMessageFirst, CancellationToken.None);

                    // _logger.LogInformation("carriersProperty::" + machinePropertiesJsonFirst);


                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                //错误处理
                _logger.LogError("设备故障定时执行失败：" + ex.ToString());
            }
            //定时任务休眠

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await new TaskFactory().StartNew(() =>
            {
            });

        }
    }
}
