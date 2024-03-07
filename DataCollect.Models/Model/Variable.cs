using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollect.Models.Model
{
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
        /// PLC备注
        /// </summary>
        public string PlcComments { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string IType { get; set; }
        /// <summary>
        /// DB块地址
        /// </summary>
        public string DbValue { get; set; }
        /// <summary>
        /// 当前信息
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
