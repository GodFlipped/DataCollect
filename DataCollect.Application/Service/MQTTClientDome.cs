using DataCollect.Interface.KgMqttClient;
using DataCollect.Interface.KgMqttClient.Dtos;
using DataCollect.Interface.TCPServer;
using Furion.DependencyInjection;
using HslCommunication.MQTT;
using System;
using System.Text;

namespace DataCollect.Application.Service
{
   public class MQTTClientDome : ITransient
    {
        private KgMqttClient _kgMqttClient;

        public MQTTClientDome(KgMqttClient kgMqttClient)
        {
            _kgMqttClient = kgMqttClient;
           
        }

        public void ClientDome()
        {
            ClientReturnDataEvevt.CreateInstance().EventReturnData += ReturnDataEvevt_EventReturnData;
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

        private void ReturnDataEvevt_EventReturnData(ReturnData returnData)
        {
            var aa = returnData;
            MqttApplicationMessage message = new MqttApplicationMessage();
            message.QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce;
            message.Payload = Encoding.UTF8.GetBytes("cccc");
            _kgMqttClient.mqttClient.PublishMessage(message);
        }
      
    }
}
