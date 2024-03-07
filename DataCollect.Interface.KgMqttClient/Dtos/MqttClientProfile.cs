using Furion.DependencyInjection;

namespace DataCollect.Interface.KgMqttClient.Dtos
{
    [SkipScan]
    public class MqttClientProfile
    {
        /// <summary>
        /// Ip地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 订阅名称
        /// </summary>
        public string SubscribeName { get; set; }

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ClientId { get; set; }
    }
}
