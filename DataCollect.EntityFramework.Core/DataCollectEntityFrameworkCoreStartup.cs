using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Furion;
using Furion.DatabaseAccessor;
using Microsoft.Extensions.DependencyInjection;
using WCS.EntityFramework;

namespace DataCollect.Api.Core.EntityFramework.Core
{
   public class DataCollectEntityFrameworkCoreStartup:AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseAccessor(options => {
                options.AddDb<DefaultDbContext>(DbProvider.SqlServer);
            }, "DataCollect.EntityFramework.Migrations");
        }
    }
}
