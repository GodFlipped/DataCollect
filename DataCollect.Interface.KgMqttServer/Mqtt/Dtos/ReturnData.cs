using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.KgMqttServer.Mqtt.Dtos
{
   public class ReturnData
    {
        /// <summary>
        /// 订阅关键字
        /// </summary>
        public string Subscribe { get; set; }
        /// <summary>
        /// 订阅关键字
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string CliendId { get; set; }
        
    }
}
