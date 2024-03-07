using Furion.DependencyInjection;
using OpcUaHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollect.Application.Service.OpcUa.Dtos
{
    [SkipScan]
    public class OpcUaConnect
    {
        public OpcUaClient OpcUaClientConnect { get; set; }

        public List<Variable> Variables { get; set; }
        public List<string> PlcScadaKey { get; set; }
    }
}
