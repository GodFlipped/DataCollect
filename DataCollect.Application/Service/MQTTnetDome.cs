using DataCollect.Application.Consts;
using DataCollect.Application.Helper;
using DataCollect.Application.Service.OpcUa.Dtos;
using DataCollect.Core.Configure;
using DataCollect.Core.Entities.Mqtt;
using DataCollect.Interface.MQTTnet;
using DataCollect.Interface.MQTTnet.EventTrigger;
using DataCollect.Interface.MQTTnet.Models;
using DataCollect.Interface.ReceivePSortMachineInformation;
using DataCollect.Interface.TCPServer.Models;
using Furion.DatabaseAccessor;
using Furion.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataCollect.Application.Service
{
    public class MQTTnetDome : ITransient
    {
        private MQTTnetEvent _mQTTnetEvent;
        private MQTTnetClient _mQTTnetClient;
        public string clientId = JDInfo.DeviceId;
        private readonly ILogger<MQTTnetDome> _log;
        public MQTTnetDome(MQTTnetEvent mQTTnetEvent, MQTTnetClient mQTTnetClient, ILogger<MQTTnetDome> log)
        {
            _mQTTnetEvent = mQTTnetEvent;
            _mQTTnetClient = mQTTnetClient;
            _log = log;
        }
        public void ClientDome()
        {
            //_mQTTnetEvent.EventMQTTnetEventInformation += _mQTTnetEvent_EventMQTTnetEventInformation;
            ReceiveSortMachineMessageEvent.CreateInstance().EventSortMessageData += ReturnDataEvevt_EventReturnData;

        }

        private void ReturnDataEvevt_EventReturnData(ReceiveSortMachineMessage returnData)
        {

            try
            {
                var timeToLong = DateTimeToLongS10(DateTime.Now);
                //var a = _mQTTnetClient._connectStatus;
               // _log.LogInformation("ReceivePlc::::::::::" + DateTime.Now.ToString() + ":::::::::::" + returnData.SortMachineMessage);
                var value = JsonConvert.DeserializeObject<Variable>(returnData.SortMachineMessage);

                var id = value.IpAddress + "|" + value.OpcValue;
                var iType = value.IType;
                var valueInformation = value.Value;
                var deviceType = value.DeviceType;
                var errorRetrunValue = new Tuple<string, string, string>("", "", "");
                var deviceNumber = value.DeviceNumber;
                var deviceCode = value.DeviceCode;
                //var faultInfRecord = value.faultInfRecord;
                string key = string.Empty;
                //分拣机故障信息上传
                if (iType == "Error" && deviceType == "SorterError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.SorterError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "sorting-module.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    //_log.LogInformation("uploadSorterErrorList::::::::::" + value.faultInfRecord.Count);

                                 //   _log.LogInformation("uploadSorterError::::::::::" + sortErrorReportJson);
                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                errorRetrunValue = SorterMachineError.SorterError(value.faultInfRecord[i].index);
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                key = "sorting-module.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                               // _log.LogInformation("uploadSorterRelErrorList::::::::::" + value.faultInfRecord.Count);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                             //   _log.LogInformation("uploadSorterRelError::::::::::" + sortErrorReportJson);
                            }
                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }

                }
                //导入台故障信息上传
                if (iType == "Error" && deviceType == "InductionError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.InductionError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "induction-of-sorter.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                  //  _log.LogInformation("uploadInductionErrorList::::::::::" + value.faultInfRecord.Count);

                                  //  _log.LogInformation("uploadInductionError::::::::::" + sortErrorReportJson);
                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                errorRetrunValue = SorterMachineError.InductionError(value.faultInfRecord[i].index);
                                key = "induction-of-sorter.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                              //  _log.LogInformation("uploadInductionRelErrorList::::::::::" + value.faultInfRecord.Count);

                              //  _log.LogInformation("uploadInductionRelError::::::::::" + sortErrorReportJson);
                            }

                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }

                }
                //灰度扫描仪故障信息上传
                if (iType == "Error" && deviceType == "GreyDetectoreError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.GreyDetectoreError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "grayscale-detector.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                  //  _log.LogInformation("uploadGlDErrorErrorList::::::::::" + value.faultInfRecord.Count);

                                   // _log.LogInformation("uploadGlDError::::::::::" + sortErrorReportJson);

                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                errorRetrunValue = SorterMachineError.GreyDetectoreError(value.faultInfRecord[i].index);
                                key = "grayscale-detector.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                            //    _log.LogInformation("uploadGLDRelErrorList::::::::::" + value.faultInfRecord.Count);

                              //  _log.LogInformation("uploadGLDRelError::::::::::" + sortErrorReportJson);
                            }

                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }
                //环扫相机故障信息上传
                if (iType == "Error" && deviceType == "ScannerError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //_log.LogInformation("ReceiveScanPlc::::::::::" + DateTime.Now.ToString() + ":::::::::::" + returnData.SortMachineMessage);
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.ScannerError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "scan-module.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                   // _log.LogInformation("uploadScanErrorList::::::::::" + value.faultInfRecord.Count);

                                  //  _log.LogInformation("uploadScanError::::::::::" + sortErrorReportJson);
                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {

                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };

                                errorRetrunValue = SorterMachineError.ScannerError(value.faultInfRecord[i].index);
                                key = "scan-module.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                               // _log.LogInformation("uploadScanRelError::::::::::" + sortErrorReportJson);
                             //   _log.LogInformation("uploadScanRelErrorList::::::::::" + value.faultInfRecord.Count);

                            }
                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }
                //小车故障信息上传
                if (iType == "Error" && deviceType == "CarriersError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.CarriersError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "car-of-sorter.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                  //  _log.LogInformation("uploadCarriersError::::::::::" + sortErrorReportJson);
                                   // _log.LogInformation("uploadCarriersErrorList::::::::::" + value.faultInfRecord.Count);

                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                errorRetrunValue = SorterMachineError.CarriersError(value.faultInfRecord[i].index);
                                key = "car-of-sorter.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                               // _log.LogInformation("uploadCarriersRelError::::::::::" + sortErrorReportJson);
                             //   _log.LogInformation("uploadCarriersRelErrorList::::::::::" + value.faultInfRecord.Count);

                            }
                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }
                //格口故障信息上传
                if (iType == "Error" && deviceType == "ChutesError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.ChutesError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "chute.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                   // _log.LogInformation("uploadChutesError::::::::::" + sortErrorReportJson);
                                  //  _log.LogInformation("uploadChutesErrorList::::::::::" + value.faultInfRecord.Count);

                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                key = "chute.error-release-report";
                                errorRetrunValue = SorterMachineError.ChutesError(value.faultInfRecord[i].index);
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                //_log.LogInformation("uploadChutesRelError::::::::::" + sortErrorReportJson);
                              //  _log.LogInformation("uploadChutesRelErrorList::::::::::" + value.faultInfRecord.Count);

                            }
                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }
                //急停按钮故障信息上传
                if (iType == "Error" && deviceType == "EPError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.EPError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "e-stop-buttons.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    //_log.LogInformation("uploadEPError::::::::::" + sortErrorReportJson);
                                 //   _log.LogInformation("uploadEPErrorList::::::::::" + value.faultInfRecord.Count);

                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {

                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                errorRetrunValue = SorterMachineError.EPError(value.faultInfRecord[i].index);

                                key = "e-stop-buttons.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                               // _log.LogInformation("uploadEPRelError::::::::::" + sortErrorReportJson);
                              //  _log.LogInformation("uploadEPRelErrorList::::::::::" + value.faultInfRecord.Count);

                            }
                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }
                //48V供电柜故障信息上传
                if (iType == "Error" && deviceType == "SDError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.SDError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "power-of-sorter.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);

                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                key = "power-of-sorter.error-release-report";
                                errorRetrunValue = SorterMachineError.SDError(value.faultInfRecord[i].index);
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);

                            }

                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }
                //直线电机故障信息上传
                if (iType == "Error" && deviceType == "MSError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.MSError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "motor-of-sorter.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                errorRetrunValue = SorterMachineError.MSError(value.faultInfRecord[i].index);
                                key = "motor-of-sorter.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);

                            }

                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }
                // 打印机故障信息上传
                if (iType == "Error" && deviceType == "PrinterError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        for (int i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                if (value.faultInfRecord == null)
                                {
                                    value.faultInfRecord = new List<errorInf>();
                                }
                                var indexNow = (i + 1).ToString();
                                var errorinfo = value.faultInfRecord.Where(n => n.index == indexNow).FirstOrDefault();
                                //当前index没有记录，就记录上
                                if (errorinfo == null)
                                {
                                    errorRetrunValue = SorterMachineError.PrinterError(indexNow);
                                    string guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var sortErrorReport = new MqttReportEvent
                                    {
                                        deviceId = _mQTTnetClient.clientId,
                                        timestamp = timeToLong,
                                        messageId = Guid.NewGuid().ToString("N"),
                                        events = new List<ThingModelEvent>()
                                    };

                                    key = "printer.error-occur-report";
                                    var events = new List<ThingModelEvent>
                                    {
                                        new ThingModelEvent
                                        {
                                            key = key,
                                            parameters =
                                            new SortingExceptionReport{
                                            createTime = DateTimeToLong(DateTime.Now),
                                            alarmNo =guid,
                                            componentNo = deviceNumber,
                                            alarmDesc = errorRetrunValue.Item1,
                                            errorCode=errorRetrunValue.Item3,
                                            errodrDesc=errorRetrunValue.Item1,
                                            errorLevel = errorRetrunValue.Item2 }
                                        }
                                    };
                                    sortErrorReport.events = events;
                                    var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                    var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                    .WithPayload(sortErrorReportJson)
                                    .Build();
                                    _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                }
                            }
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var sortErrorReport = new MqttReportEvent
                                {
                                    deviceId = _mQTTnetClient.clientId,
                                    timestamp = timeToLong,
                                    messageId = Guid.NewGuid().ToString("N"),
                                    events = new List<ThingModelEvent>()
                                };
                                errorRetrunValue = SorterMachineError.PrinterError(value.faultInfRecord[i].index);
                                key = "printer.error-release-report";
                                var events = new List<ThingModelEvent>
                                {
                                    new ThingModelEvent
                                    {
                                        key = key,
                                        parameters =
                                        new SortingExceptionReport{
                                        createTime = DateTimeToLong(DateTime.Now),
                                        alarmNo =value.faultInfRecord[i].errorNo,
                                        errorCode=errorRetrunValue.Item3,
                                        componentNo = deviceNumber}
                                    }
                                };
                                sortErrorReport.events = events;
                                var sortErrorReportJson = JsonConvert.SerializeObject(sortErrorReport);
                                var sortErrorReportMessage = new MqttApplicationMessageBuilder()
                                .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                                .WithPayload(sortErrorReportJson)
                                .Build();
                                _mQTTnetClient.managedClient.PublishAsync(sortErrorReportMessage, CancellationToken.None);
                                var model = value.faultInfRecord.Where(m => m.errorNo == value.faultInfRecord[i].errorNo).FirstOrDefault();
                                value.faultInfRecord.Remove(model);
                                RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);

                            }

                        }
                        if (!valueInformation.Contains("1"))
                        {
                            value.faultInfRecord.Clear();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                _log.LogInformation("occureMachineInfromationFault::::::::::" + DateTime.Now.ToString() + ":::::::::::" + ex.ToString());
            }
        }

        public void TcpServerEvent_EventTcpServerReceive(TcpServerMessage tcpMessage)
        {
            try
            {
                if (_mQTTnetClient._connectStatus)
                {
                    //var tcpMessage = message as TcpServerMessage;
                    if (tcpMessage.MessageType == MessageType.ScanDataPush)
                    {

                        //发送分拣机扫描事件
                        var scanInforReport = new MqttReportEvent
                        {
                            deviceId = _mQTTnetClient.clientId,
                            timestamp = _mQTTnetClient.DateTimeToInt(),
                            messageId = Guid.NewGuid().ToString("N"),
                            events = new List<ThingModelEvent>
                            {
                                new ThingModelEvent
                                {
                                    key = "scan-module.scan-info-report",
                                    parameters = new ScanInfoModel
                                    {
                                        packageNo = tcpMessage.ObjectToHandle,
                                        scanTime = DateTimeToLong(tcpMessage.ScannerTime),
                                        inductNo = scanInductNo(tcpMessage.InductNo),
                                        supplyType = tcpMessage.SupplyType,
                                        status = TranslateScanStatus(tcpMessage.ObjectToHandle),
                                        scannerType = "1",//相机CAM
                                    }
                                }
                            }
                        };
                        var scanInfoJson = JsonConvert.SerializeObject(scanInforReport);
                        var scanInfoMessage = new MqttApplicationMessageBuilder()
                           .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                           .WithPayload(scanInfoJson)
                           .Build();
                        _mQTTnetClient.managedClient.PublishAsync(scanInfoMessage, CancellationToken.None);
                        _log.LogInformation("ScanInfo::" + scanInfoJson);

                    }
                    if (tcpMessage.MessageType == MessageType.ResultDataPush)
                    {
                        //发送扫描器扫描事件
                        var taskStatusReport = new MqttReportEvent
                        {
                            deviceId = _mQTTnetClient.clientId,
                            timestamp = _mQTTnetClient.DateTimeToInt(),
                            messageId = Guid.NewGuid().ToString("N"),
                            events = new List<ThingModelEvent>
                            {
                                new ThingModelEvent
                                {
                                    key = "sorting-module.sort-info-report",
                                    parameters = new TaskStatusModel
                                    {
                                        packageNo = tcpMessage.ObjectToHandle,
                                        distinctSupplyNo = tcpMessage.TackingId,
                                        inductionNo = scanInductNo(tcpMessage.InductNo),
                                        carNo = CarriersChange(tcpMessage.CarrierNo,tcpMessage.InductNo),
                                        inductGroup = InductionGroupChange(tcpMessage.ChuteNo),
                                        length = tcpMessage.Length,
                                        width = tcpMessage.Width,
                                        heigh = tcpMessage.Height,
                                        weight = tcpMessage.Weight,
                                        scanTime = DateTimeToLong(tcpMessage.ScannerTime),
                                        scannerType = "1",
                                        scannerNo = ScannerNoChange(tcpMessage.ScannerNo),
                                        supplyType = tcpMessage.SupplyType,
                                        sortTime = DateTimeToLong(tcpMessage.CreateTime),
                                        requestChuteNo = tcpMessage.RequestChuteNo,
                                        chuteNo = "ST01-Chutes"+tcpMessage.ChuteNo,
                                        chuteType = TranslateChuteType(tcpMessage.Results),
                                        result = TranslateResult(tcpMessage.Results),
                                        executeCount = tcpMessage.CycleTime,
                                        resultDescription = ""

                                    }
                                }
                            }
                        };
                        var taskStatusJson = JsonConvert.SerializeObject(taskStatusReport);
                        //发送分拣机扫描事件
                        var taskStatusMessage = new MqttApplicationMessageBuilder()
                           .WithTopic("$iot/v1/device/" + clientId + "/events/post")
                           .WithPayload(taskStatusJson)
                           .Build();
                        _mQTTnetClient.managedClient.PublishAsync(taskStatusMessage, CancellationToken.None);
                        _log.LogInformation("SorterInfo::" + taskStatusJson);
                    }
                }
            }
            catch (Exception ex)
            {

                _log.LogInformation("ReceiveInfromationToWas::::::::::" + DateTime.Now.ToString() + ":::::::::::" + ex.ToString());
            }

        }
        /// <summary>
        /// DateTime to timestamp
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string DateTimeToLong(DateTime dateTime)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0, 0)); // 当地时区
            long timeStamp = (long)(dateTime.ToUniversalTime() - startTime).TotalMilliseconds; // 相差秒数
            return timeStamp.ToString();
        }

        private long DateTimeToLongS(DateTime dateTime)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0, 0)); // 当地时区
            long timeStamp = (long)(dateTime.ToUniversalTime() - startTime).TotalMilliseconds; // 相差秒数
            return timeStamp;
        }
        private long DateTimeToLongS10(DateTime dateTime)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0, 0)); // 当地时区
            long timeStamp = (long)(dateTime.ToUniversalTime() - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }
        public string InductionGroupChange(string chutes)
        {
            string inductionGroup = "";
            if (!string.IsNullOrEmpty(chutes))
            {
                inductionGroup = chutes.Substring(0, 1);
            }
            return inductionGroup;
        }

        public string ScannerNoChange(string scanner)
        {
            string scannerNo = "scannerGun";
            if (!string.IsNullOrEmpty(scanner))
            {
                switch (scanner)
                {
                    case "000061":
                        scannerNo = "ST01-SC01";
                        break;
                    case "000062":
                        scannerNo = "ST01-SC02";
                        break;
                    case "000063":
                        scannerNo = "ST02-SC03";
                        break;
                    case "000064":
                        scannerNo = "ST02-SC04";
                        break;
                }
            }
            return scannerNo;
        }
        private string TranslateScanStatus(string objectToHandle)
        {
            var status = "1";
            if (objectToHandle == "NoValidBarcode")
            {
                status = "2";
            }
            if (objectToHandle.Contains(";"))
            {
                status = "3";
            }
            return status;
        }

        private string TranslateChuteType(string result)
        {
            var resultCode = "2";
            switch (result)
            {
                case "ND":
                    resultCode = "1";
                    break;
            }
            return resultCode;
        }


        private string TranslateResult(string result)
        {
            var resultCode = "001-001";
            switch (result)
            {
                case "NR":
                    resultCode = "002-002";
                    break;
                case "MB":
                    resultCode = "003-408";
                    break;
                case "DE":
                    resultCode = "007-407";
                    break;
                case "ID":
                    resultCode = "008-402";
                    break;
                case "MR":
                    resultCode = "010-013";
                    break;
                case "IB":
                    resultCode = "005-005";
                    break;
                case "MT":
                    resultCode = "004-004";
                    break;
                    //case "CF":
                    //    resultCode = "016-019";
                    //    break;
                    //case "CD":
                    //    resultCode = "017-020";
                    //    break;
                    //case "SP":
                    //    resultCode = "018-021";
                    //    break;
                    //case "OF":
                    //    resultCode = "019-022";
                    //    break;
                    //case "BC":
                    //case "BF":
                    //    resultCode = "021-024";
                    //    break;
                    //case "DD":
                    //    resultCode = "022-025";
                    //    break;
                    //case "DJ":
                    //    resultCode = "023-026";
                    //    break;
                    //case "TO":
                    //    resultCode = "011-014";
                    //    break;

            }
            return resultCode;
        }
        public string CarriersChange(string carriers, string induction)
        {
            string carriersNo = "";
            string excuter = "";
            if (!string.IsNullOrEmpty(induction))
            {
                excuter = induction.Split('-')[0];
                if (excuter == "ScsServer02")
                {
                    excuter = "ST02-Carriers";
                }
                else
                {
                    excuter = "ST01-Carriers";
                }
            }
            if (!string.IsNullOrEmpty(carriers))
            {
                carriersNo = carriers.Substring(3);//003157   ST02-Carriers387
            }
            return excuter + carriersNo;
        }
        public string scanInductNo(string induct)
        {

            string inductNo = "";
            if (!string.IsNullOrEmpty(induct))
            {
                inductNo = "";
            }
            var allInduction = induct.Split('-');

            if (allInduction[0] == "ScsServer01")
            {
                var scsExcuter = "ST01-";
                switch (allInduction[1])
                {
                    case "000401":
                        inductNo = scsExcuter + "ID11";
                        break;
                    case "000402":
                        inductNo = scsExcuter + "ID12";
                        break;
                    case "000403":
                        inductNo = scsExcuter + "ID13";
                        break;
                    case "000404":
                        inductNo = scsExcuter + "ID14";
                        break;
                    case "000405":
                        inductNo = scsExcuter + "ID15";
                        break;
                    case "000406":
                        inductNo = scsExcuter + "ID16";
                        break;
                    case "000501":
                        inductNo = scsExcuter + "ID21";
                        break;
                    case "000502":
                        inductNo = scsExcuter + "ID22";
                        break;
                    case "000503":
                        inductNo = scsExcuter + "ID23";
                        break;
                    case "000504":
                        inductNo = scsExcuter + "ID24";
                        break;
                    case "000505":
                        inductNo = scsExcuter + "ID25";
                        break;
                    case "000506":
                        inductNo = scsExcuter + "ID26";
                        break;

                }
            }
            if (allInduction[0] == "ScsServer02")
            {
                var scsExcuter = "ST02-";
                switch (allInduction[1])
                {
                    case "000401":
                        inductNo = scsExcuter + "ID11";
                        break;
                    case "000402":
                        inductNo = scsExcuter + "ID12";
                        break;
                    case "000403":
                        inductNo = scsExcuter + "ID13";
                        break;
                    case "000404":
                        inductNo = scsExcuter + "ID14";
                        break;
                    case "000405":
                        inductNo = scsExcuter + "ID15";
                        break;
                    case "000406":
                        inductNo = scsExcuter + "ID16";
                        break;
                    case "000501":
                        inductNo = scsExcuter + "ID21";
                        break;
                    case "000502":
                        inductNo = scsExcuter + "ID22";
                        break;
                    case "000503":
                        inductNo = scsExcuter + "ID23";
                        break;
                    case "000504":
                        inductNo = scsExcuter + "ID24";
                        break;
                    case "000505":
                        inductNo = scsExcuter + "ID25";
                        break;
                    case "000506":
                        inductNo = scsExcuter + "ID26";
                        break;

                }
            }
            return inductNo;
        }

        /// <summary>
        /// 业务写在此方法
        /// </summary>
        /// <param name="e"></param>
        /// <param name="Data"></param>
        private void _mQTTnetEvent_EventMQTTnetEventInformation(MqttApplicationMessageReceivedEventArgs e, string Data)
        {
            //收到的消息
            var aa = Data;

            ////发送消息
            //var scanInforReport = new MqttReportEvent
            //{
            //    deviceId = _mQTTnetClient.clientId,
            //    timestamp = _mQTTnetClient.DateTimeToInt(),
            //    messageId = Guid.NewGuid().ToString("N"),
            //    events = new List<ThingModelEvent>
            //    {
            //        new ThingModelEvent
            //        {
            //            key = "scanner.scan-status-report",
            //            parameters = new ScanStatusModel
            //            {
            //                code = "JD1234567" + DateTime.Now.Millisecond,
            //                scanTime = _mQTTnetClient.DateTimeToInt(),
            //                status = "01"
            //            }
            //        }
            //    }
            //};
            //var scanInfoJson = JsonConvert.SerializeObject(scanInforReport);
            //var scanMessage = new MqttApplicationMessageBuilder()
            //   .WithTopic("$iot/v1/device/" + clientId + "/events/post")
            //   .WithPayload(scanInfoJson)
            //   .Build();
            //Task.Run(async () => { await _mQTTnetClient.managedClient.PublishAsync(scanMessage, CancellationToken.None); });
        }
    }
}
