using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Core.Configure
{
    public class SampleAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 拦截之前

            var resultContext = await next();

            // 拦截之后

            // 异常拦截
            if (resultContext.Exception != null)
            {

            }
        }
    }
}
