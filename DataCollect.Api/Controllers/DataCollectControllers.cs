using DataCollect.Application;
using DataCollect.Application.Helper;
using DataCollect.Application.Service;
using DataCollect.Application.Service.OpcUa.Dtos;
using DataCollect.Core.Configure;
using Furion.DynamicApiController;
using Furion.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpcUaHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace DataCollect.Api.Controllers
{
    /// <summary>
    /// 数据采集
    /// </summary>
    [AppAuthorize]
    [AllowAnonymous]
    [ApiDescriptionSettings(ApiGroupConsts.CLIENT_CENTER)]
    public class DataCollectControllers : IDynamicApiController
    {
        private readonly ILogger<DataCollectControllers> _log;
        private OpcUaClientConnectServer _context;
        public DataCollectControllers(OpcUaClientConnectServer context, ILogger<DataCollectControllers> log)
        {
            
            _context = context;
            _log = log;
        }

        /// <summary>
        /// 导入信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        [HttpPost("ImportCustomer")]
        public string ImportCustomer(IFormFile file)
        {
            try
            {
                List<VariableKeys> scadas = new List<VariableKeys>();
                List<PlcInformation> ips = new List<PlcInformation>();
                var KeysData = RedisConn.Instance.rds.Get<List<VariableKeys>>("Keys");
                if (KeysData != null)
                {
                    //获取所有主键信息
                    scadas = KeysData;
                }

                var IpsData = RedisConn.Instance.rds.Get<List<PlcInformation>>("Ips");
                if (IpsData != null)
                {
                    //获取所有ip信息
                    ips = IpsData;
                }

                string resultData = string.Empty;

                if (file.Length > 0)
                {
                    DataTable dt = new DataTable();
                    string strMsg;
                    //利用IFormFile里面的OpenReadStream()方法直接读取文件流
                    dt = ExcelHelper.ExcelToDatatable(file.OpenReadStream(), Path.GetExtension(file.FileName), out strMsg, "Sheet1");
                    if (!string.IsNullOrEmpty(strMsg))
                    {
                        resultData = strMsg;
                        return resultData;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow val in dt.Rows)
                        {
                            Variable scada = new Variable();
                            scada.Id = val["Id"].ToString();
                            scada.IpAddress = val["IpAddress"].ToString();
                            scada.Message = val["Message"].ToString();
                            scada.DeviceCode = val["DeviceCode"].ToString();
                            scada.OpcValue = val["OpcValue"].ToString();
                            scada.IType = val["IType"].ToString();
                            scada.UpdateTime = DateTime.Now;
                            scada.DeviceType = val["DeviceType"].ToString();
                            scada.DeviceNumber = val["DeviceNumber"].ToString();
                            scada.GetPLc = val["GetPLc"].ToString();
                            scada.ComponentPropertyType = val["ComponentPropertyType"].ToString();
                            scada.ComponentProperty = val["ComponentProperty"].ToString();

                            RedisConn.Instance.rds.Set(val["IpAddress"].ToString()+"|"+val["OpcValue"].ToString(), scada);

                            //追加
                            if (ExcelHelper.isListScadaExists(scadas, val["IpAddress"].ToString()+"|"+val["OpcValue"].ToString()))
                            {
                                scadas.Add(new VariableKeys { OpcValue = val["IpAddress"].ToString() + "|" + val["OpcValue"].ToString()});
                            }

                         
                            if (ExcelHelper.isListExists(ips, val["IpAddress"].ToString()))
                            {
                                //追加
                                ips.Add(new PlcInformation { Address = val["IpAddress"].ToString() });
                            }

                        }
                        RedisConn.Instance.rds.Set("Keys", scadas);
                        RedisConn.Instance.rds.Set("Ips", ips);
                        resultData = L.Text["导入完成"];
                    }
                    else
                    {

                        resultData = L.Text["Excel导入表无数据"];
                    }

                }

                return resultData;
            }
            catch (Exception ex)
            {

                _log.LogError(ex.Message);
                return ex.Message;
            }

        }


        /// <summary>
        /// 启动采集
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
  
        [HttpGet("RegisterOPCUACollect")]
        public async Task<string> RegisterOPCUACollectAsync(string ip = null)
        {
            string resultData = string.Empty;

            try
            {
                List<PlcInformation> Newip = new List<PlcInformation>(); ;
                List<Variable> Scadas = new List<Variable>();
                //获取主键字典
                var ListKye = RedisConn.Instance.rds.Get<List<VariableKeys>>("Keys");
                foreach (var item in ListKye)
                {
                    //获取主键信息
                    var variable = RedisConn.Instance.rds.Get<Variable>(item.OpcValue);
                    
                    if (variable.GetPLc=="1")
                    {
                        Scadas.Add(variable);
                    }
                }
                //传ip按照ip启动，不传启动所有
                if (ip != null)
                {
                    Newip.Add(new PlcInformation { Address = ip });
                }
                else
                {

                    Newip = RedisConn.Instance.rds.Get<List<PlcInformation>>("Ips");
                }
                _log.LogInformation("getScadaCount===>"+ Scadas.Count);
                if (await _context.OpcUaClientConnectAsync(Scadas, Newip))
                {
                    resultData = L.Text["启动成功"]; 
                }
                else
                {
                    resultData = L.Text["启动失败"];
                }
                return resultData;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return ex.Message;
            }
        }
        /// <summary>
        /// 关闭数据采集
        /// </summary>
        /// <returns></returns>
     
        [HttpDelete("OpcUaClientClose")]
        public string OpcUaClientClose()
        {
            string resultData = string.Empty;

            try
            {
              
                if (_context.OpcUaClientClose())
                {
                    resultData = L.Text["关闭成功"];
                }
                return resultData;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return ex.Message;
            }
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="ip">例：opc.tcp://10.197.81.5:4840</param>
        /// <param name="Key">例：ns=3;s=\"P1001\".R_DV_VSD_Fault</param>
        /// <param name="data">例：0</param>
        /// <returns></returns>
        [HttpPut("OpcUaClientUpdate")]
        public string OpcUaClientUpdate(string ip,string Key, bool data)
        {
            string resultData = string.Empty;

            try
            {
                //创建注册对象
                OpcUaClient m_OpcUaClient = new OpcUaClient();
                //连接PLC
                m_OpcUaClient.ConnectServer(ip);
                //将信息写入PLC
                if (m_OpcUaClient.WriteNode(Key, data))
                {
                    resultData = L.Text["修改成功"];
                }
                return resultData;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return ex.Message;
            }
        }

        ///// <summary>
        ///// 启动采集
        ///// </summary>
        ///// <param name="ip"></param>
        ///// <returns></returns>

        //[HttpGet("GetVariable")]
        //public List<Variable> GetVariable(string ip = null)
        //{
        //    List<Variable> Scadas = new List<Variable>();
        //    try
        //    {
        //        //获取主键字典
        //        var ListKye = RedisConn.Instance.rds.Get<List<VariableKeys>>("Keys");
        //        int i = 0;
        //        foreach (var item in ListKye)
        //        {
        //            i++;
        //            //获取主键信息
        //            Scadas.Add(RedisConn.Instance.rds.Get<Variable>(item.OpcValue));
        //            if (i > 10)
        //            {
        //                return Scadas;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogError(ex.Message);
        //    }
        //    return Scadas;
        //}
    }

}