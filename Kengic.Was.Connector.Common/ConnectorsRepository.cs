using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kengic.Was.Connector.Common
{
    public class ConnectorsRepository
    {
        private const string ConnectorSection = "connectorSection";
        private const string ConnectorsRepositoryName = "ConnectorsRepository";

        private static readonly ConcurrentDictionary<string, IConnector> ConnectorDictionary =
            new ConcurrentDictionary<string, IConnector>();


        private static List<IConnector> GetConnectorsList() => ConnectorDictionary.Values.ToList();

        private static bool ConnectorExecute(
            Func<IConnector, bool> executeMethod)
        {
            var connectorList = GetConnectorsList();
            if ((connectorList == null) || (connectorList.Count <= 0))
            {
                return false;
            }

            foreach (var connector in connectorList)
            {
                executeMethod(connector);
            }

            return true;
        }


        public static void LoadConnectorConfiguration(string connectId,IConnector connector)
        {
            try
            {
                connector.Id = connectId;
                ConnectorDictionary.TryAdd(connectId, connector);


            }
            catch (Exception ex)
            {
               
            }
        }



        public static bool InitializeConnector() => ConnectorExecute(InitializeConnector);

        public static bool InitializeConnector(IConnector tConnector)
        {
            try
            {
                var connectorInstance = GetConnectorInstance(tConnector.Id);
                if (connectorInstance == null)
                {
                    

                    return false;
                }

                if (connectorInstance.Initialize())
                {


                    return true;
                }
               

                return false;
            }
            catch (Exception ex)
            {
              

                return false;
            }
        }

        public static bool StartConnector() => ConnectorExecute(StartConnector);

        public static bool StartConnector(IConnector tConnector)
        {
            try
            {
                var connectInstance = GetConnectorInstance(tConnector.Id);
                if (connectInstance == null)
                {
                    return false;
                }

                if (connectInstance.Connect())
                {
                   

                    return true;
                }
               
                return false;
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }

        public static bool CloseConnector(IConnector tConnector)
        {
            try
            {
                var connectInstance = GetConnectorInstance(tConnector.Id);
                if (connectInstance == null)
                {
                    return false;
                }

                if (connectInstance.DisConnect())
                {
                    connectInstance.RecSendMsgStatus = false;

                 
                    return true;
                }

           

                return false;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        public static bool CloseConnector() => ConnectorExecute(CloseConnector);
        private static bool IsExistConnector(string connectorId) => ConnectorDictionary.ContainsKey(connectorId);

        public static IConnector GetConnectorInstance(string connectorId)
        {
            if (!IsExistConnector(connectorId))
            {
                return null;
            }

            IConnector tConnector;
            if (ConnectorDictionary.TryGetValue(connectorId, out tConnector))
            {
                return tConnector;
            }

       
            return null;
        }
    }
}
