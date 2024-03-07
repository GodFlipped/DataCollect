using OpcUaHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using DataCollect.Core.Configure;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Furion.DependencyInjection;
using DataCollect.Application.Service.OpcUa.Dtos;
using DataCollect.Application.Helper;
using DataCollect.Interface.ReceivePSortMachineInformation;
using System.Collections.Concurrent;
using Furion.DatabaseAccessor;
using DataCollect.Core.Entities.Mqtt;
using WCS.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DataCollect.Application.DataBaseService;

namespace DataCollect.Application.Service
{
    public class OpcUaClientConnectServer:ISingleton
    {
        private readonly ILogger<OpcUaClientConnectServer> _log;
        private List<OpcUaConnect> opcUaClients = new List<OpcUaConnect>();
        private IHubContext<ChatHub> _hub;
        
        public OpcUaClientConnectServer(IHubContext<ChatHub> hub,
            ILogger<OpcUaClientConnectServer> log
         
            )
        {
            _hub = hub;
            _log = log;
            
        }

        /// <summary>
        /// 关闭数据采集
        /// </summary>
        /// <returns></returns>
        public bool OpcUaClientClose()
        {
            foreach (var item in opcUaClients)
            {
                item.OpcUaClientConnect.RemoveAllSubscription();
            }
            opcUaClients.Clear();
            return true;
        }

        /// <summary>
        /// 启动数据采集
        /// </summary>
        /// <param name="scadas">数据采集节点</param>
        /// <param name="ips">PLC的地址</param>
        /// <returns></returns>
        public async Task<bool> OpcUaClientConnectAsync(List<Variable> scadas, List<PlcInformation> ips)
        {
            OpcUaClientClose();
            try
            {
                //循环ip列表
                foreach (var ip in ips)
                {

                    var Count = 0;

                    //获取一台PLC的信息
                    var NewScadas = scadas.FindAll(d => d.IpAddress == ip.Address);
                    int MaxSubscriptions = 1000;//批量注册最大值
                    int CurrentsubScriptions = 0;//批量注册当前值
                    int count = 0;//创建注册信息标识
                    for (int i = 0; i < NewScadas.Count; i++)
                    {
                        //固定属性的获取不到值，直接在excel中配置
                        //if (NewScadas[i].GetPLc=="0")
                        //{
                        //    continue;
                        //}
                        if (count == 0)
                        {
                            //创建注册信息
                            OpcUaConnect opcUaClient = new OpcUaConnect();
                            //创建注册信息字段
                            opcUaClient.OpcUaClientConnect = new OpcUaClient();
                            opcUaClient.Variables = new List<Variable>();
                            opcUaClient.PlcScadaKey = new List<string>();
                            opcUaClients.Add(opcUaClient);
                            //拿到字典的下标
                            Count = opcUaClients.Count() - 1;
                            //获取ip列表
                            var NewPLCAddress = RedisConn.Instance.rds.Get<List<PlcInformation>>("Ips");
                            foreach (var item in NewPLCAddress)
                            {
                                if (item.Address == ip.Address)
                                {
                                    item.ConnectNumber = Count;
                                }
                            }
                            //更新ip列表
                            RedisConn.Instance.rds.Set("Ips", NewPLCAddress);
                            await opcUaClients[Count].OpcUaClientConnect.ConnectServer(ip.Address);
                            opcUaClients[Count].OpcUaClientConnect.OpcStatusChange += M_OpcUaClientcc2_01_OpcStatusChange;
                            count = 1;
                            
                        }
                        //将信息写入到注册信息字典
                        CurrentsubScriptions++;
                        opcUaClients[Count].Variables.Add(NewScadas[i]);//信息
                        opcUaClients[Count].PlcScadaKey.Add(NewScadas[i].OpcValue);//信息地址
                        if (CurrentsubScriptions == MaxSubscriptions)
                        {
                            //满足最大注册数量开始注册
                            var keys = opcUaClients[Count].PlcScadaKey.ToArray();
                            //订阅
                            opcUaClients[Count].OpcUaClientConnect.AddSubscription(ip.Address, keys, SubCallback);
                            CurrentsubScriptions = 0;
                            count = 0;
                        }
                        if (i == NewScadas.Count - 1)
                        {
                            //不满足数量的信息注册
                            var keys = opcUaClients[Count].PlcScadaKey.ToArray();
                            opcUaClients[Count].OpcUaClientConnect.AddSubscription(ip.Address, keys, SubCallback);
                        }
                    }



                }
                return true;

            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
                return false;
            }
        }

        private void M_OpcUaClientcc2_01_OpcStatusChange(object sender, OpcUaStatusEventArgs e)
        {
            if (e.Error)
            {

                StringBuilder br = new StringBuilder("");
                br.AppendLine("监控器检测到异常！" + e.Text);
                _log.LogError(br.ToString());
            }


        }

        private async void SubCallback(string key, MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args)
        {
            try
            {
                // 如果有多个的订阅值都关联了当前的方法，可以通过key和monitoredItem来区分
                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                var ScadaData = RedisConn.Instance.rds.Get<Variable>(key + "|" + monitoredItem.DisplayName);
                //判断如果是Byte信息需要解析
                StringBuilder byteData = new StringBuilder();
                StringBuilder byteDataOld = new StringBuilder();
                //var errorList = new List<errorInf>();
                //var errorList = new ConcurrentBag<errorInf>();
                //if (ScadaData.faultInfRecord != null)
                //{
                //    foreach (var item in ScadaData.faultInfRecord)
                //    {
                //        errorList.Add(item);
                //    }

                //}
                //读点传的是false和true，需要转换0和1
                if (notification.Value != null && notification.Value.Value != null && notification.Value.Value.ToString().ToUpper() == "TRUE")
                {
                    byteData.Append('1');
                }
                else if (notification.Value != null && notification.Value.Value != null && notification.Value.Value.ToString().ToUpper() == "FALSE")
                {
                    byteData.Append('0');
                }
                else
                {
                    if (notification.Value != null && notification.Value.WrappedValue.TypeInfo != null && notification.Value.WrappedValue.TypeInfo.ToString() == "ExtensionObject")
                    {

                        var reverse = (byte[])((Opc.Ua.ExtensionObject)notification.Value.WrappedValue.Value).Body;
                        //32位解析高字节低字位
                        if (reverse.Length == 32)
                        {
                            var newArr = new byte[reverse.Length];
                            var stringConvertLength = reverse.Length / 8 - 1;
                            var j = 0;
                            for (int i = 0; i < newArr.Length; i++)
                            {
                                byteDataOld.Append(reverse[i]);
                                if (i != 0 && i % 8 == 0)
                                {
                                    stringConvertLength--;
                                    j = 0;
                                }
                                var a = stringConvertLength * 8 + j;
                                newArr[i] = reverse[a];
                                j++;
                            }
                            for (int i = 0; i < newArr.Length; i++)
                            {
                                byteData.Append(newArr[i]);
                                //if (newArr[i] == 1 && ScadaData.faultInfRecord == null)
                                //{
                                //    ScadaData.faultInfRecord = new ConcurrentBag<errorInf>();
                                //    ScadaData.faultInfRecord.Add((new errorInf() { index = (i + 1).ToString() }));
                                //}
                                //else if (newArr[i] == 1 && ScadaData.faultInfRecord != null)
                                //{
                                //    var scadaDataCurrent = ScadaData.faultInfRecord.Where(n => n.index == (i + 1).ToString()).FirstOrDefault();
                                //    if (scadaDataCurrent == null)
                                //    {
                                //        ScadaData.faultInfRecord.Add((new errorInf() { index = (i + 1).ToString() }));
                                //    }

                                //}


                            }
                        }
                        //64位解析高字节低字位
                        else if (reverse.Length == 64)
                        {
                            int n = 1;
                            var newArr = new byte[reverse.Length];
                            var stringConvertLength = reverse.Length / 32 - 1;
                            var j = 8;
                            for (int i = 0; i < newArr.Length; i++)
                            {
                                byteDataOld.Append(reverse[i]);
                                if (i != 0 && i % 32 == 0)
                                {
                                    n = 0;
                                    stringConvertLength++;
                                    j = 8 * n;
                                }
                                if (i != 0 && i % 8 == 0)
                                {
                                    n++;
                                    j = 8 * n;
                                }
                                var a = stringConvertLength * 32 - j;
                                newArr[i] = reverse[a];
                                j--;
                            }
                            for (int i = 0; i < newArr.Length; i++)
                            {
                                byteData.Append(newArr[i]);
                                //if (newArr[i] == 1 && ScadaData.faultInfRecord == null)
                                //{
                                //    ScadaData.faultInfRecord = new ConcurrentBag<errorInf>();
                                //    ScadaData.faultInfRecord.Add((new errorInf() { index = (i + 1).ToString() }));
                                //}
                                //else if (newArr[i] == 1 && ScadaData.faultInfRecord != null)
                                //{
                                //    var scadaDataCurrent = ScadaData.faultInfRecord.Where(n => n.index == (i + 1).ToString()).FirstOrDefault();
                                //    if (scadaDataCurrent == null)
                                //    {
                                //        ScadaData.faultInfRecord.Add((new errorInf() { index = (i + 1).ToString() }));
                                //    }

                                //}


                            }
                        }
                        //96位解析高字节低字位
                        else if (reverse.Length == 96)
                        {
                            int n = 1;
                            var newArr = new byte[reverse.Length];
                            var stringConvertLength = reverse.Length / 32 - 2;
                            var j = 8;
                            for (int i = 0; i < newArr.Length; i++)
                            {
                                byteDataOld.Append(reverse[i]);
                                if (i != 0 && i % 32 == 0)
                                {
                                    n = 0;
                                    stringConvertLength++;
                                    j = 8 * n;
                                }
                                if (i != 0 && i % 8 == 0)
                                {
                                    n++;
                                    j = 8 * n;
                                }
                                var a = stringConvertLength * 32 - j;
                                newArr[i] = reverse[a];
                                j--;
                            }
                            for (int i = 0; i < newArr.Length; i++)
                            {
                                byteData.Append(newArr[i]);
                                //if (newArr[i] == 1 &&ScadaData.faultInfRecord==null)
                                //{
                                //    ScadaData.faultInfRecord = new ConcurrentBag<errorInf>();
                                //    ScadaData.faultInfRecord.Add((new errorInf() { index = (i + 1).ToString() }));
                                //}
                                //else if(newArr[i] == 1 && ScadaData.faultInfRecord!=null)
                                //{
                                //    var scadaDataCurrent = ScadaData.faultInfRecord.Where(n => n.index == (i + 1).ToString()).FirstOrDefault();
                                //    if (scadaDataCurrent == null)
                                //    {
                                //        ScadaData.faultInfRecord.Add((new errorInf() { index = (i + 1).ToString() }));
                                //    }

                                //}

                            }
                        }
                        else
                        {
                            foreach (var item in reverse)
                            {
                                byteData.Append(item);
                            }
                        }
                    }
                    else if (notification.Value != null)
                    {
                        byteData.Append(notification.Value.WrappedValue.Value);
                    }
                }
                //获取完整信息

                ScadaData.Value = byteData.ToString();//更新信息
                ScadaData.OldValue = byteDataOld.ToString();
                ScadaData.UpdateTime = DateTime.Now;

                //保存信息
                RedisConn.Instance.rds.Set(key + "|" + monitoredItem.DisplayName, ScadaData);
                string JsonScadaData = JsonDataContractJsonSerializer.ObjectToJson(ScadaData);
                var message = new ReceiveSortMachineMessage
                {
                    SortMachineMessage = JsonScadaData
                };
                ReceiveSortMachineMessageEvent.CreateInstance().PlcData = message;
                RedisConn.Instance.rds.Publish("alarm", JsonScadaData);
               // _log.LogInformation("getPLcData::::::::::==>" + JsonScadaData);
            }
            catch (Exception ex)
            {
                _log.LogError("getPLcDateError::::::::::==>" + ex.Message.ToString());
            }
           
        }
        public static byte[] Serialize(object obj)      
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bf.Serialize(stream, obj);
            byte[] datas = stream.ToArray();
            stream.Dispose();
            return datas;
        }
    }
}
