
using Furion.DatabaseAccessor;
using System;

namespace Kengic.Shared
{

    public abstract class KgAuditedModel
    {
        public string Id { get; set; }
        public string creator{ get; set; }
        public DateTime creatTime{ get; set; }
        public string createMessage{ get; set; }
        public string updater{ get; set; }
        public DateTime updateTime{ get; set; }
        public string updateMessage{ get; set; }
        public string finisher{ get; set; }
        public DateTime finishTime{ get; set; }
        public string finishMessage{ get; set; }
        public int objectStatus{ get; set; }
        public int workStatus{ get; set; }
        public bool deleteFlag{ get; set; }
        public string dataVersion{ get; set; }
        public string standby1{ get; set; }
        public string standby2{ get; set; }
        public string standby3{ get; set; }
        public string standby4{ get; set; }
        public string standby5{ get; set; }
        public string comments{ get; set; }
    }
}
