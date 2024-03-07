using DataCollect.Interface.KgMqttClient;
using DataCollect.Interface.KgMqttClient.Dtos;
using DataCollect.Interface.TCPServer;
using Furion.DependencyInjection;
using HslCommunication.MQTT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DataCollect.Application.Helper;
using DataCollect.Application.Service.OpcUa.Dtos;
using DataCollect.Core.Configure;
using DataCollect.Interface.MQTTnet.Models;
using DataCollect.Interface.ReceivePSortMachineInformation;
using Microsoft.Extensions.Logging;
using MQTTnet;
using static DataCollect.Application.Service.HTTPnetSorterAlert;

namespace DataCollect.Application.Service
{
    public class HTTPnetSorterAlert : ITransient
    {
      
        private readonly ILogger<HTTPnetSorterAlert> _log;
        public HTTPnetSorterAlert(ILogger<HTTPnetSorterAlert> log)
        {
            _log = log;
        }

        public void ClientDome()
        {
            ReceiveSortMachineMessageEvent.CreateInstance().EventSortMessageData += ReturnDataEvevt_EventReturnData;
        }

        public void Receive(object sender, string message)
        {
            try
            {
                var scanBarcode = message.Trim();
                scanBarcode = scanBarcode.Replace(@"\u000", "");

            }
            catch (Exception)
            {
            }
        }

        private void ReturnDataEvevt_EventReturnData(ReceiveSortMachineMessage returnData)
        {
            try
            {
                var value = JsonConvert.DeserializeObject<Variable>(returnData.SortMachineMessage);
                var id = value.IpAddress + "|" + value.OpcValue;
                var iType = value.IType;
                var valueInformation = value.Value;
                var deviceType = value.DeviceType;
                var errorRetrunValue = new Tuple<string, string, string>("", "", "");
                var deviceNumber = value.DeviceNumber;
                var deviceCode = value.DeviceCode;

                //var faultInfRecord = value.faultInfRecord;
                var key = string.Empty;
                //分拣机故障信息上传
                if (iType == "Error" && deviceType == "SorterError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                              
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.SorterError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                              
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "SorterError" // 设备类型
                                    };
                          
                                    alerts.Add(alert);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                //导入台故障信息上传
                if (iType == "Error" && deviceType == "InductionError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.InductionError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "InductionError" // 设备类型
                                    };

                                    alerts.Add(alert);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    //  _log.LogInformation("uploadInductionErrorList::::::::::" + value.faultInfRecord.Count);

                                    //  _log.LogInformation("uploadInductionError::::::::::" + sortErrorReportJson);
                                }
                            }
                            var response = HttpClientSingleton.PostAsync("", alerts);
                        }
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                //灰度扫描仪故障信息上传
                if (iType == "Error" && deviceType == "GreyDetectoreError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.GreyDetectoreError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "GreyDetectoreError" // 设备类型
                                    };

                                    alerts.Add(alert);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    //  _log.LogInformation("uploadGlDErrorErrorList::::::::::" + value.faultInfRecord.Count);

                                    // _log.LogInformation("uploadGlDError::::::::::" + sortErrorReportJson);

                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                //环扫相机故障信息上传
                if (iType == "Error" && deviceType == "ScannerError" && !string.IsNullOrEmpty(valueInformation))
                {
                    //_log.LogInformation("ReceiveScanPlc::::::::::" + DateTime.Now.ToString() + ":::::::::::" + returnData.SortMachineMessage);
                    //出现故障上传
                    if (valueInformation.Contains("1"))
                    {
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.ScannerError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "ScannerError" // 设备类型
                                    };

                                    alerts.Add(alert);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    // _log.LogInformation("uploadScanErrorList::::::::::" + value.faultInfRecord.Count);

                                    //  _log.LogInformation("uploadScanError::::::::::" + sortErrorReportJson);
                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {

                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.CarriersError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "CarriersError" // 设备类型
                                    };

                                    alerts.Add(alert);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    //  _log.LogInformation("uploadCarriersError::::::::::" + sortErrorReportJson);
                                    // _log.LogInformation("uploadCarriersErrorList::::::::::" + value.faultInfRecord.Count);

                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.ChutesError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "ChutesError" // 设备类型
                                    };

                                    alerts.Add(alert);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    // _log.LogInformation("uploadChutesError::::::::::" + sortErrorReportJson);
                                    //  _log.LogInformation("uploadChutesErrorList::::::::::" + value.faultInfRecord.Count);

                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.EPError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "EPError" // 设备类型
                                    };

                                    alerts.Add(alert);

                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                    //_log.LogInformation("uploadEPError::::::::::" + sortErrorReportJson);
                                    //   _log.LogInformation("uploadEPErrorList::::::::::" + value.faultInfRecord.Count);

                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {

                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.SDError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "SDError" // 设备类型
                                    };
                                    alerts.Add(alert);
                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);

                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (int i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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
                        var alerts = new List<Alert>();
                        for (var i = 0; i < valueInformation.Length; i++)
                        {
                            //包含1说明有故障采集到
                            if (valueInformation[i] == '1')
                            {
                                value.faultInfRecord ??= new List<errorInf>();
                                var indexNow = (i + 1).ToString();
                                var errorinf = value.faultInfRecord.FirstOrDefault(n => n.index == indexNow);
                                //当前index没有记录，就记录上
                                if (errorinf == null)
                                {
                                    errorRetrunValue = SorterMachineError.MSError(indexNow);
                                    var guid = Guid.NewGuid().ToString("N");
                                    var errorInfo = new errorInf()
                                    {
                                        index = indexNow,
                                        errorNo = guid,
                                        errorMessage = errorRetrunValue.Item1,
                                    };
                                    value.faultInfRecord.Add(errorInfo);
                                    var alert = new Alert
                                    {
                                        FaultContent = errorRetrunValue.Item1, // 内容
                                        FaultLevel = errorRetrunValue.Item2, // 等级
                                        FaultLocation = deviceNumber, // 位置
                                        FaultTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FaultType = "MSError" // 设备类型
                                    };
                                    alerts.Add(alert);

                                    RedisConn.Instance.rds.Set(value.IpAddress + "|" + value.OpcValue, value);
                                }
                            }
                        }
                        var response = HttpClientSingleton.PostAsync("", alerts);
                    }
                    //解除故障上传
                    if (value.faultInfRecord != null)
                    {
                        for (var i = 0; i < value.faultInfRecord.Count; i++)
                        {
                            //如果是空的id,并且有故障需要上传
                            if (!string.IsNullOrEmpty(value.faultInfRecord[i].errorNo) && !string.IsNullOrEmpty(value.faultInfRecord[i].index) && valueInformation.Substring(Convert.ToInt16(value.faultInfRecord[i].index) - 1, 1) == "0")
                            {
                                var model = value.faultInfRecord.FirstOrDefault(m => m.errorNo == value.faultInfRecord[i].errorNo);
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

        #region 分拣机告警

        /// <summary>
        /// 分拣机告警
        /// </summary>
        public class Alert
        {
            /// <summary>
            /// 报警时间，设备故障报警时间，格式：年-月-日 时:分:秒 2023-10-01 10:10:11
            /// </summary>
            [JsonProperty("faultTime")]
            public string FaultTime { get; set; }

            /// <summary>
            /// 设备故障类型
            /// </summary>
            [JsonProperty("faultType")]
            public string FaultType { get; set; }

            /// <summary>
            /// 故障设备位置
            /// </summary>
            [JsonProperty("faultLocation")]
            public string FaultLocation { get; set; }

            /// <summary>
            /// 故障内容，内容包括 xx编号的 xx 类型设备出现了什么问题
            /// </summary>
            [JsonProperty("faultContent")]
            public string FaultContent { get; set; }

            /// <summary>
            /// 故障级别，B0-B9
            /// </summary>
            [JsonProperty("faultLevel")]
            public string FaultLevel { get; set; }
        }
        #endregion
    }
}
