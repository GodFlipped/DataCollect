using DataCollect.Application.Helper;
using DataCollect.Application.Service.OpcUa.Dtos;
using DataCollect.Core.Configure;
using DataCollect.Interface.ReceivePlcTask;
using Furion.DependencyInjection;
using Kengic.Was.CrossCuttings.Netty.Packets;
using Newtonsoft.Json;
using OpcUaHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataCollect.Application.Service
{
    public  class HandlePlcTaskDome : ITransient
    {

        public void HandTaskDome()
        {
            ReceivePlcMessageEvent.CreateInstance().EventPlcnData += ReturnDataEvevt_EventReturnData;

        }

        private void ReturnDataEvevt_EventReturnData(ReturnPlcData returnData)
        {
           

        }

        private string EquipmentLine(Byte value, string lineNo)
        {
            string equipmentLine = "";
            switch (value)
            {
                case 1:
                    equipmentLine= "ns=3;s=\""+ lineNo +" "+"-Control\""+".Start";
                    break;
                case 2:
                    equipmentLine = "ns=3;s=\"" + lineNo + " " + "-Control\"" + ".Stop";
                    break;



            }
            return equipmentLine;
        }


        public string ChangeToOperatorType(ushort value)
        {
            var operatorValue = "";
            switch (value)
            {
                case 1:
                    operatorValue = ".Reset";
                    break;
                case 2:
                    operatorValue = ".JamShileding";
                    break;
                case 4:
                    operatorValue = ".ModelChange";
                    break;
            }
            return operatorValue;
        }




    }
}
