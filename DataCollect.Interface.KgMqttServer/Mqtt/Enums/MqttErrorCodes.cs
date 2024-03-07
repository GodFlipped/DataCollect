using Furion.FriendlyException;
using System.ComponentModel;

namespace DataCollect.Interface.KgMqttServer.Mqtt.Enums
{
    public enum MqttErrorCodes
    {
        /// <summary>
        /// 连接数据不存在
        /// </summary>
        [Description("连接数据不存在"), ErrorCodeItemMetadata("连接数据不存在")]
        m1000,
    }
}
