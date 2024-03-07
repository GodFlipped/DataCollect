
using DataCollect.Application.Service;
using Kengic.Was.Connector.Common;
using Kengic.Was.Connector.NettyCheckeServer;
using Kengic.Was.Connector.NettyClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DataCollect.Api
{
    public class Startup
    {
        public  IConnector nettyClient = new NettyClient();

        public IConnector nettyServer = new NettyServer();
        public Startup(IConfiguration configuration)
        {
            //ConnectorsRepository.LoadConnectorConfiguration("nettyClient", nettyClient);
            //ConnectorsRepository.InitializeConnector(nettyClient);


           // ConnectorsRepository.LoadConnectorConfiguration("nettyServer", nettyServer);
           // ConnectorsRepository.InitializeConnector(nettyServer);

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetInduction>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnet48Power>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetCarriers>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetChutes>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetGLD>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetMotor>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetPrinter>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetScanner>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetSorter>();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MQTTnetStopButton>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
