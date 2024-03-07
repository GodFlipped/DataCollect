using Furion.RemoteRequest;
using Furion.UnifyResult;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCollect.Core.IHttpRequest
{
    [Host("http://localhost/", 63051)]
    public interface IHttp : IHttpDispatchProxy
    {
        [Get("GetAllMapData")]
        Task<Dictionary<string, object>> GetAllMapData(string map);
    }
}
