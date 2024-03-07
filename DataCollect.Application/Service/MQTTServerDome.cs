using DataCollect.Interface.KgMqttServer.Mqtt.Dtos;
using Furion.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Application.Service
{
   public class MQTTServerDome : ITransient
    {
        public void ServerDome()
        {
            ServerReturnDataEvevt.CreateInstance().EventReturnData += MQTTServerDome_EventReturnData;
        }

        private void MQTTServerDome_EventReturnData(ReturnData returnData)
        {
            var aa = returnData;
        }
    }
}
