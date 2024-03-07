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
    public class MQTTnetSorter : BackgroundService
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

        public MQTTnetSorter(ILogger<MQTTnetSorter> logger, MQTTnetClient mQTTnetClient)
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
                    var propertiesHeader = new MqttReportSortingProperties1D
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationSorting1D
                        {
                            sortingDeviceBaseInfo = new List<SortingDeviceBaseInfo>(),
                            sortingDeviceSpeedScope = new SortingDeviceSpeedScope(),
                            sortingPackageLengthScope = new SortingPackageLengthScope(),
                            sortingPackageWidthScope = new SortingPackageWidthScope(),
                            sortingPackageHeightScope = new SortingPackageHeightScope(),
                            sortingPackageWeightScope = new SortingPackageWeightScope(),
                            sortingCapacityScope = new List<SortingCapacityScope>()
                        }
                    };

                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //设备总数上传
                        if (variable.DeviceType == "SorterProperty" && variable.OpcValue == "ST-Sorter-设备总数")
                        {
                            propertiesHeader.properties.sortingDeviceAmount = Convert.ToInt16(variable.ComponentProperty);
                        }
                        //设备基础信息上传
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "设备基础信息")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.sortingDeviceBaseInfo.Add(new SortingDeviceBaseInfo
                            {
                                componentNo = variable.DeviceNumber,
                                productionDate = Helper.TimeHelper.DateTimeToLongS(Convert.ToDateTime(ComponentPropertys[0])).ToString(),
                                manufacturerName = ComponentPropertys[1],
                                softwareVersion = "",
                                modelNumber = "",
                                deviceSn = ""


                            });
                        }
                        //设备速度范围
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "设备速度范围")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.sortingDeviceSpeedScope = new SortingDeviceSpeedScope()
                            {
                                minSpeed = float.Parse(ComponentPropertys[0]),
                                maxSpeed = float.Parse(ComponentPropertys[1])
                            };
                        }
                        //设备类型
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "设备类型")
                        {
                            propertiesHeader.properties.sortingDeviceType = variable.ComponentProperty;
                        }
                        //货物长度范围
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "货物长度范围")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.sortingPackageLengthScope = new SortingPackageLengthScope()
                            {
                                minLength = Convert.ToInt32(ComponentPropertys[0]),
                                maxLength = Convert.ToInt32(ComponentPropertys[1])
                            };
                        }
                        //货物宽度范围
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "货物宽度范围")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.sortingPackageWidthScope = new SortingPackageWidthScope()
                            {
                                minWidth = Convert.ToInt32(ComponentPropertys[0]),
                                maxWidth = Convert.ToInt32(ComponentPropertys[1])
                            };
                        }
                        //货物高度范围
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "货物宽度范围")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.sortingPackageHeightScope = new SortingPackageHeightScope()
                            {
                                minHeight = Convert.ToInt32(ComponentPropertys[0]),
                                maxHeight = Convert.ToInt32(ComponentPropertys[1])
                            };
                        }
                        //货物重量范围
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "货物重量范围")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.sortingPackageWeightScope = new SortingPackageWeightScope()
                            {
                                minWeight = float.Parse(ComponentPropertys[0]),
                                maxWeight = float.Parse(ComponentPropertys[1])
                            };
                        }
                        //分拣效率范围
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "分拣效率范围")
                        {
                            var ComponentPropertys = variable.ComponentProperty.Split(';');
                            propertiesHeader.properties.sortingCapacityScope.Add(new SortingCapacityScope()
                            {
                                componentNo = variable.DeviceNumber,
                                minCapacity = Convert.ToInt32(ComponentPropertys[0]),
                                maxCapacity = Convert.ToInt32(ComponentPropertys[1])
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
                   // _logger.LogInformation("uploadSorterProperties" + machinePropertiesJsonFirst);
                }
                //4S上传一次
                if (ListKye != null && ListKye.Count > 0)
                {
                    var propertiesHeader = new MqttReportSortingProperties4S
                    {

                        version = _version,
                        messageId = Guid.NewGuid().ToString("N"),
                        deviceId = _deviceId,
                        timestamp = timeToLong10,
                        properties = new MachineBasicInformationSorting4S
                        {
                            sortingCondition = new List<SortingCondition>(),
                            sortingFirstCarSynStatus = new List<SortingFirstCarSynStatus>(),
                            sortingSpeedStatus = new List<SortingSpeedStatus>(),
                            sortingInitializeStatus = new List<SortingInitializeStatus>(),
                            sortingTotalMileage = new List<SortingTotalMileage>(),
                            sortingTodayMileage = new List<SortingTodayMileage>(),
                            sortingTotalRunningTime = new List<SortingTotalRunningTime>(),
                            sortingTodayRunningTime = new List<SortingTodayRunningTime>(),
                            sortingInduceCapacity = new List<SortingInduceCapacity>(),
                            sortingLoadFactor = new List<SortingLoadFactor>(),
                            sortingDeviceSpeed = new List<SortingDeviceSpeed>(),
                            sortingControlMode = new List<SortingControlMode>(),
                            sortingFaultStatus = new List<SortingFaultStatus>(),
                            sortingStartStopStatus = new List<SortingStartStopStatus>(),
                            sortingEStopStatus = new List<SortingEStopStatus>()
                        }
                    };
                    foreach (var item in ListKye)
                    {
                        var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                        //设备分拣条件
                        if (variable.DeviceType == "SorterError")
                        {
                            var isReady = "1";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value.Contains("1"))
                            {
                                isReady = "0";
                            }
                            propertiesHeader.properties.sortingCondition.Add(new SortingCondition
                            {
                                componentNo = variable.DeviceNumber,
                                sorterCondition = isReady
                            });
                        }
                        //首车同步状态
                        if (variable.DeviceType == "SorterError")
                        {
                            var isReady = "1";
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                var sorter = variable.Value.ToCharArray();
                                if (sorter[5] == '1' || sorter[6] == '1' || sorter[7] == '1')
                                {
                                    isReady = "0";
                                }
                                propertiesHeader.properties.sortingFirstCarSynStatus.Add(new SortingFirstCarSynStatus
                                {
                                    componentNo = variable.DeviceNumber,
                                    firstCarSynStatus = isReady
                                });
                            }

                        }
                        //速度达标状态
                        if (variable.DeviceType == "SorterError")
                        {
                            var isReady = "1";
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                var sorter = variable.Value.ToCharArray();
                                if (sorter[4] == '1')
                                {
                                    isReady = "0";
                                }
                                propertiesHeader.properties.sortingSpeedStatus.Add(new SortingSpeedStatus
                                {
                                    componentNo = variable.DeviceNumber,
                                    speedStatus = isReady
                                });
                            }

                        }
                        //设备控制模式
                        if (variable.DeviceType == "SorterError")
                        {
                            var isReady = "1";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value.Contains("1"))
                            {
                                isReady = "0";
                            }
                            propertiesHeader.properties.sortingInitializeStatus.Add(new SortingInitializeStatus
                            {
                                componentNo = variable.DeviceNumber,
                                initializeStatus = isReady
                            });
                        }
                        //总运行里程
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "总运行时间")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingTotalMileage.Add(new SortingTotalMileage
                                {
                                    componentNo = variable.DeviceNumber,
                                    totalMileage = (float)(Convert.ToDouble(variable.Value) * 2.1 * 60 * 60 / 1000)
                                });
                            }

                        }
                        //今日运行里程
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "今日运行时间")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingTodayMileage.Add(new SortingTodayMileage
                                {
                                    componentNo = variable.DeviceNumber,
                                    todayMileage = (float)(Convert.ToDouble(variable.Value) * 2.1)
                                });
                            }

                        }
                        //总运行时间
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "总运行时间")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingTotalRunningTime.Add(new SortingTotalRunningTime
                                {
                                    componentNo = variable.DeviceNumber,
                                    totalRunningTime = float.Parse(variable.Value)
                                });
                            }
                        }
                        //今日运行时间
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "今日运行时间")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingTodayRunningTime.Add(new SortingTodayRunningTime
                                {
                                    componentNo = variable.DeviceNumber,
                                    todayRunningTime = float.Parse(variable.Value)
                                });
                            }
                        }
                        //总供件效率
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "总供件效率")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingInduceCapacity.Add(new SortingInduceCapacity
                                {
                                    componentNo = variable.DeviceNumber,
                                    induceCapacity = Convert.ToInt32(variable.Value)
                                });
                            }

                        }
                        //小车满载率
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "小车满载率")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingLoadFactor.Add(new SortingLoadFactor
                                {
                                    componentNo = variable.DeviceNumber,
                                    loadFactor = (float)(Math.Round(Convert.ToDouble(variable.Value) / 100, 2))

                                });
                            }

                        }
                        //设备速度
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "设备速度")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingDeviceSpeed.Add(new SortingDeviceSpeed
                                {
                                    componentNo = variable.DeviceNumber,
                                    speed = float.Parse(variable.Value)
                                });
                            }
                        }
                        ////设备控制模式
                        if (variable.DeviceType == "SorterError")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                var isReady = "1";
                                var sorter = variable.Value.ToCharArray();
                                if (sorter[10] == '1')
                                {
                                    isReady = "2";
                                }
                                propertiesHeader.properties.sortingControlMode.Add(new SortingControlMode
                                {
                                    componentNo = variable.DeviceNumber,
                                    componentControlMode = isReady
                                });
                            }
                        }
                        //设备故障状态
                        if (variable.DeviceType == "SorterError")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                var isError = "0";
                                if (variable.Value.Contains("1"))
                                {
                                    isError = "1";
                                }
                                propertiesHeader.properties.sortingFaultStatus.Add(new SortingFaultStatus
                                {
                                    componentNo = variable.DeviceNumber,
                                    componentFaultStatus = isError
                                });
                            }
                        }
                        //启停状态
                        if (variable.DeviceType == "SorterProperty" && variable.ComponentPropertyType == "启停状态")
                        {
                            if (!string.IsNullOrEmpty(variable.Value))
                            {
                                propertiesHeader.properties.sortingStartStopStatus.Add(new SortingStartStopStatus
                                {
                                    componentNo = variable.DeviceNumber,
                                    componentStartStopStatus = variable.Value
                                });
                            }
                        }
                        //设备急停状态
                        if (variable.DeviceType == "EPError" && variable.DeviceNumber.Contains("MCC"))
                        {
                            string isStop = "1";
                            if (!string.IsNullOrEmpty(variable.Value) && variable.Value == "1")
                            {
                                isStop = "0";
                            }
                            propertiesHeader.properties.sortingEStopStatus.Add(new SortingEStopStatus
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
