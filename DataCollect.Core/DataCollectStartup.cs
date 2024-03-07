using DataCollect.Core.Configure;
using Furion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace DataCollect.Core
{
   public class DataCollectStartup: AppStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSignalR();
            
            services.AddCors(op =>
            {
                op.AddPolicy("CorsPolicy", set =>
                {
                    set.SetIsOriginAllowed(origin => true)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
                });
            });
            services.AddRemoteRequest();
            services.AddControllers().AddInjectWithUnifyResult();
            services.AddControllersWithViews().AddAppLocalization();  // 注册多语言
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("CorsPolicy");
            // 配置多语言，必须在 路由注册之前
            app.UseAppLocalization();
            app.UseUnifyResultStatusCodes();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseInject(string.Empty);
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub", options =>
                  options.Transports = HttpTransports.All);
                endpoints.MapControllers();
              

            });
        }
    }
}
