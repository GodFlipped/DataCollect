using Kengic.Opcua.Demo.Service;
using System;


namespace Kengic.Opcua.Demo.Entry
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                OpcuaManagement server = new OpcuaManagement();
                server.CreateServerInstance();
                Console.WriteLine("OPC-UA服务已启动，地址 opc.tcp://localhost:8020/ ");
                Console.ReadLine();
            }
            catch (Exception ex)
            {

                Console.WriteLine("异常："+ ex.Message);
                Console.ReadLine();
            }
          


          
        }
    }
}
