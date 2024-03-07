using Furion.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollect.Application.Service.OpcUa.Dtos
{
    [SkipScan]
    public class PlcInformation
    {
        public string Address { get; set; }
        public int ConnectNumber { get; set; }
    }
}
