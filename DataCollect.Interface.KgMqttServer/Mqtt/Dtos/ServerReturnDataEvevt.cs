using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.KgMqttServer.Mqtt.Dtos
{
    public delegate void DelegateReturnData(ReturnData returnData);
    public class ServerReturnDataEvevt
    {
        private ServerReturnDataEvevt()
        {

        }
        private static ServerReturnDataEvevt _ReturnDataEvevt = null;
        public static ServerReturnDataEvevt CreateInstance()
        {
            if (_ReturnDataEvevt == null)
            {
                _ReturnDataEvevt = new ServerReturnDataEvevt();
            }
            return _ReturnDataEvevt;
        }
        public event DelegateReturnData EventReturnData;
        public  void OnEventReturnData(ReturnData returnData)
        {
            if (EventReturnData != null)
            {
                EventReturnData(returnData);
            }
            
        }
        /// <summary>
        /// 订阅信息
        /// </summary>
        private ReturnData data;
        public ReturnData Data
        {
            set
            {
                data = value;
                OnEventReturnData(value);
            }
            get { return data; }
        }

    }
}
