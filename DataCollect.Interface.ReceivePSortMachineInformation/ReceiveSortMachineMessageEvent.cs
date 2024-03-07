using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollect.Interface.ReceivePSortMachineInformation
{
    public delegate void DelegateSortMachineTaskData(ReceiveSortMachineMessage returnData);
  public  class ReceiveSortMachineMessageEvent
    {
        private ReceiveSortMachineMessageEvent()
        {

        }
        private static ReceiveSortMachineMessageEvent _ReturnSortDataEvevt = null;
        public static ReceiveSortMachineMessageEvent CreateInstance()
        {
            if (_ReturnSortDataEvevt == null)
            {
                _ReturnSortDataEvevt = new ReceiveSortMachineMessageEvent();
            }
            return _ReturnSortDataEvevt;
        }
        public event DelegateSortMachineTaskData EventSortMessageData;
        public void OnEventSortMessageTaskData(ReceiveSortMachineMessage returnData)
        {
            if (EventSortMessageData != null)
            {
                EventSortMessageData(returnData);
            }

        }
        /// <summary>
        /// 订阅信息
        /// </summary>
        private ReceiveSortMachineMessage data;
        public ReceiveSortMachineMessage PlcData
        {
            set
            {
                data = value;
                OnEventSortMessageTaskData(value);
            }
            get { return data; }
        }

    }
}
