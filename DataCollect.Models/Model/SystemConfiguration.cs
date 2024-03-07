using DataCollect.Core.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Models.Model
{
   public class SystemConfiguration: KgAuditedModel
    {

        /// <summary>
        /// 接口系统标识
        /// </summary>
        public string interfaceSystemId;

        /// <summary>
        /// 接口系统描述
        /// </summary>
        public string interfaceSystemDescription;

        /// <summary>
        /// 系统类型代码
        /// </summary>
        public string systemTypeCode;

        /// <summary>
        /// 类型
        /// </summary>
        public string itype;

        /// <summary>
        /// 接口系统调用配置
        /// </summary>
        public string callConfig;

        /// <summary>
        /// 是否生效
        /// </summary>
        public int isUse;

        /// <summary>
        /// 接口参数
        /// </summary>
        public string parameter;
    }
}
