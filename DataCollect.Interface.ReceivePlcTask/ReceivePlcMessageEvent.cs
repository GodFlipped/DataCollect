using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.ReceivePlcTask
{
    public delegate void DelegatePlcTaskData(ReturnPlcData returnData);
    public  class ReceivePlcMessageEvent
    {
        private ReceivePlcMessageEvent()
        {

        }
        private static ReceivePlcMessageEvent _ReturnDataEvevt = null;
        public static ReceivePlcMessageEvent CreateInstance()
        {
            if (_ReturnDataEvevt == null)
            {
                _ReturnDataEvevt = new ReceivePlcMessageEvent();
            }
            return _ReturnDataEvevt;
        }
        public event DelegatePlcTaskData EventPlcnData;
        public void OnEventPlcTaskData(ReturnPlcData returnData)
        {
            if (EventPlcnData != null)
            {
                EventPlcnData(returnData);
            }

        }
        /// <summary>
        /// 订阅信息
        /// </summary>
        private ReturnPlcData data;
        public ReturnPlcData PlcData
        {
            set
            {
                data = value;
                OnEventPlcTaskData(value);
            }
            get { return data; }
        }

    }
}
