﻿using Furion.DependencyInjection;

namespace DataCollect.Core.Entities.Mqtt
{
    [SkipScan]
    public class MqttConnectProfile
    {
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
        /// 是否启用密码
        /// </summary>
        public bool Ispassword { get; set; }
        /// <summary>
        /// 是否发送心跳
        /// </summary>
        public bool IsHeartCheck { get; set; }
        /// <summary>
        /// 心跳内容
        /// </summary>
        public string HeartContent { get; set; }
        /// <summary>
        /// 订阅名称
        /// </summary>
        public string SubscribeName { get; set; }
     
    }
}
