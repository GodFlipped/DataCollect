using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Inject().UseStartup<Startup>();
                    webBuilder.UseKestrel()
                    .UseUrls("http://*:5001");
                }).UseSerilogDefault(config =>
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd");//按时间创建文件夹
                    string outputTemplate = "{NewLine}【{Level:u3}】{Timestamp:yyyy-MM-dd HH:mm:ss.fff}" +
                                            "{NewLine}#Msg#{Message:lj}" +
                                            "{NewLine}#Pro #{Properties:j}" +
                                            "{NewLine}#Exc#{Exception}" +
                                            new string('-', 50);//输出模板

                    config

        .WriteTo.Console(outputTemplate: outputTemplate)
        .WriteTo.File($"D:/_log/{date}/application.log",
               outputTemplate: outputTemplate,
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollingInterval: RollingInterval.Hour,//日志按日保存，这样会在文件名称后自动加上日期后缀
                                                     //rollOnFileSizeLimit: true,          // 限制单个文件的最大长度
                                                     //retainedFileCountLimit: 10,         // 最大保存文件数,等于null时永远保留文件。
                                                     //fileSizeLimitBytes: 10 * 1024,      // 最大单个文件大小
                encoding: Encoding.UTF8            // 文件字符编码
            );

                });
    }
}
