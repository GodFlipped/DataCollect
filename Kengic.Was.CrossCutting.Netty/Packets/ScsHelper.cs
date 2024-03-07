namespace Kengic.Was.CrossCuttings.Netty.Packets
{
    public class ScsHelper
    {
       
        public static string StringFormat(string messageType,string message)
        {
            var messageFormat = messageType + message;
            return messageFormat;
        }

    }
}
