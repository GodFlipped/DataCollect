using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.MQTTnet.Models
{
 public   class scanBasicInformation
    {
        [JsonProperty("scan-module.manufacturer-name")]

        public string scanModelMfName { get; set; }

        [JsonProperty("scan-module.device-sn")]

        public string scanModelSn { get; set; }
        [JsonProperty("scan-module.fault-status")]
        public List<scanModuleFaultStatus> scanFaultStatus { get; set; }


        [JsonProperty("scan-module.control-mode")]
        public List<scanMode> scanControlMode { get; set; }
        

        [JsonProperty("scan-module.device-type")]
        public List<scanType> scanModuleDcType { get; set; }

        [JsonProperty("scan-module.device-speed")]
        public string scanModuleSpeed { get; set; }



        [JsonProperty("scan-module.operation-status")]
        public List<scanOpStatus> scanMdOpStatus { get; set; }


       

        [JsonProperty("scan-module.e-stop-status")]
        public List<scanEstop> scanEStop { get; set; }


        [JsonProperty("scan-module.loading-device-status")]
        public List<inductStatus> scanLoadStatus { get; set; }




    }
    public class scanModuleFaultStatus
    {
        [JsonProperty("component-no")]
        public string componentNo { get; set; }
        [JsonProperty("component-fault-status")]
        public string componentFaultStatus { get; set; }
    }


    public class scanType
    {
        [JsonProperty("component-no")]
        public string componentNo { get; set; }
        [JsonProperty("component-type")]
        public string componentType { get; set; }
    }

    public class scanMode
    {
        [JsonProperty("component-no")]
        public string componentNo { get; set; }
        [JsonProperty("component-control-mode")]
        public string componentMode { get; set; }
    }

    public class scanOpStatus
    {
        [JsonProperty("component-no")]
        public string componentNo { get; set; }
        [JsonProperty("component-status")]
        public string componentStatus { get; set; }
    }

    public class scanEstop
    {
        [JsonProperty("component-no")]
        public string componentNo { get; set; }
        [JsonProperty("component-e-stop-status")]
        public string componentEstopStaus { get; set; }
    }

    public class inductStatus
    {
        [JsonProperty("component-no")]
        public string componentNo { get; set; }
        [JsonProperty("component-status")]
        public string componentStatus { get; set; }
    }
         
}
