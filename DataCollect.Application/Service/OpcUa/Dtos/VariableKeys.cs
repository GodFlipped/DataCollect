using Furion.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollect.Application.Service.OpcUa.Dtos
{
    [SkipScan]
    public class VariableKeys
    {
        public string OpcValue { get; set; }
    }
}
