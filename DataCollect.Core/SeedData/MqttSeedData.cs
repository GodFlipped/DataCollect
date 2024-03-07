using DataCollect.Core.Entities.Mqtt;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Core.SeedData
{
    public class MqttSeedData : IEntitySeedData<MqttConnect>
    {
     
        /// <summary>
        /// 种子数据
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbContextLocator"></param>
        /// <returns></returns>
        public IEnumerable<MqttConnect> HasData(DbContext dbContext, Type dbContextLocator)
        {
            return new[]
            {
                new MqttConnect
                {
                    Id=System.Guid.NewGuid().ToString("N"),port="1003",IsHeartCheck=false,Ispassword=false,UserName="Kengic",password="Kengic@123",SubscribeName="Kengic"
                }
            };
        }
    }
}
