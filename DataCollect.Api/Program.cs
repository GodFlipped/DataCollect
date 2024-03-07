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
                    string date = DateTime.Now.ToString("yyyy-MM-dd");//��ʱ�䴴���ļ���
                    string outputTemplate = "{NewLine}��{Level:u3}��{Timestamp:yyyy-MM-dd HH:mm:ss.fff}" +
                                            "{NewLine}#Msg#{Message:lj}" +
                                            "{NewLine}#Pro #{Properties:j}" +
                                            "{NewLine}#Exc#{Exception}" +
                                            new string('-', 50);//���ģ��

                    config

        .WriteTo.Console(outputTemplate: outputTemplate)
        .WriteTo.File($"D:/_log/{date}/application.log",
               outputTemplate: outputTemplate,
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollingInterval: RollingInterval.Hour,//��־���ձ��棬���������ļ����ƺ��Զ��������ں�׺
                                                     //rollOnFileSizeLimit: true,          // ���Ƶ����ļ�����󳤶�
                                                     //retainedFileCountLimit: 10,         // ��󱣴��ļ���,����nullʱ��Զ�����ļ���
                                                     //fileSizeLimitBytes: 10 * 1024,      // ��󵥸��ļ���С
                encoding: Encoding.UTF8            // �ļ��ַ�����
            );

                });
    }
}
