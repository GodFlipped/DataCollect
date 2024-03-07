using DataCollect.Core.Entities.Mqtt;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Application.DataBaseService
{
    public class CollectPlcDataService: IDynamicApiController
    {
        private readonly IRepository<CollectPlcData> _collectPlcDataRepository;
        public CollectPlcDataService(IRepository<CollectPlcData> collectPlcDataRepository)
        {
            _collectPlcDataRepository = collectPlcDataRepository;
        }
        /// <summary>
        /// 新增一条
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public void Insert(CollectPlcData input)
        {
            // 如果不需要返回自增Id，使用 InsertAsync即可
            var newEntity =  _collectPlcDataRepository.Insert(input);
           
        }
    }
}
