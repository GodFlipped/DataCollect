using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.KgMqttClient.Dtos
{
    public delegate void DelegateReturnData(ReturnData returnData);
    public class ClientReturnDataEvevt
    {
        private ClientReturnDataEvevt()
        {

        }
        private static ClientReturnDataEvevt _ReturnDataEvevt = null;
        public static ClientReturnDataEvevt CreateInstance()
        {
            if (_ReturnDataEvevt == null)
            {
                _ReturnDataEvevt = new ClientReturnDataEvevt();
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
