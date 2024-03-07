using Furion.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollect.Application.Service.OpcUa.Dtos
{
    [SkipScan]
    public class Variable
    {
        public string Id { get; set; }
        /// <summary>
        /// OPCUA地址
        /// </summary>
        public string OpcValue { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string IpAddress { get; set; }       
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string IType { get; set; }
        /// <summary>
        /// 转换消息
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 原来消息
        /// </summary>
        public string OldValue { get; set; }

        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNumber { get; set; }

        public string ComponentPropertyType { get; set; }
        public string ComponentProperty { get; set; }

        public List<errorInf> faultInfRecord { get; set; }
        /// <summary>
        /// 是否有opc地址
        /// </summary>
        public string GetPLc { get; set; }
    }
    public class errorInf
    {
        public string index { get; set; }
        public string errorMessage { get; set; }
        public string errorNo { get; set; }
    }
}
