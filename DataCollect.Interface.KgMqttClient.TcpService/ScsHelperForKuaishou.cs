using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Interface.KgMqttClient.TcpService
{
    public class ScsHelperForKuaishou
    {
        internal byte[] ConvertBodyToBytes(string scsBody)
        {
            byte[] rtnBytes = { };

            rtnBytes = rtnBytes.Concat(ConvertStringToBytes(scsBody)).ToArray();
            return rtnBytes;
        }
        public byte[] ConvertStringToBytes(string value) => Encoding.UTF8.GetBytes(value);
        public static byte[] FillWithSpaceChar(int fillLength)
        {
            if (fillLength == 0)
            {
                return null;
            }

            var destFillBytes = new byte[fillLength];
            for (var i = 0; i < fillLength; i++)
            {
                destFillBytes[i] = Convert.ToByte(' ');
            }
            return destFillBytes;
        }

        public void GetReceiveJsonMessageForClient(byte[] infoBytes,
          ref ConcurrentDictionary<string, string> messagesDict, ref byte[] remainBytes)
        {
            const byte startByte = 123;
            const byte endByte = 125;
            var startIndex = 0;
            var endIndex = 0;

            while (true)
            {
                if (!infoBytes.Contains(startByte) || !infoBytes.Contains(endByte))
                {
                    remainBytes = new byte[infoBytes.Length];
                    Buffer.BlockCopy(infoBytes, 0, remainBytes, 0, infoBytes.Length);
                    return;
                }

                for (var i = 0; i < infoBytes.Length; i++)
                {
                    if (infoBytes[i] == startByte)
                    {
                        startIndex = i;
                    }

                    if (infoBytes[i] != endByte) continue;

                    endIndex = i;
                    break;
                }

                var receiveBytes = new byte[endIndex - startIndex - 1];
                Buffer.BlockCopy(infoBytes, startIndex + 1, receiveBytes, 0, endIndex - startIndex - 1);

                var messageBody = Encoding.Default.GetString(receiveBytes);

                messagesDict.TryAdd(Guid.NewGuid().ToString("N"), "{" + messageBody + "}");
                if (infoBytes.Length <= endIndex + 1)
                {
                    return;
                }
                var remainByte = new byte[infoBytes.Length - endIndex - 1];
                Buffer.BlockCopy(infoBytes, endIndex + 1, remainByte, 0, infoBytes.Length - endIndex - 1);
                infoBytes = remainByte;
            }
        }


        public void GetReceiveMessageForClient(byte[] infoBytes, ref string messagesDict,
            ref byte[] remainBytes)
        {
            //const byte startByte = 33;
            //const byte endByte = 35;
            const byte startByte = 2;
            const byte endByte = 3;
            var startIndex = 0;
            var endIndex = 0;

            while (true)
            {
                if (!infoBytes.Contains(startByte) || !infoBytes.Contains(endByte))
                {
                    remainBytes = new byte[infoBytes.Length];
                    Buffer.BlockCopy(infoBytes, 0, remainBytes, 0, infoBytes.Length);
                    return;
                }

                for (var i = 0; i < infoBytes.Length; i++)
                {
                    if (infoBytes[i] == startByte)
                    {
                        startIndex = i;
                    }

                    if (infoBytes[i] != endByte) continue;

                    endIndex = i;
                    break;
                }

                var receiveBytes = new byte[endIndex - startIndex - 1];
                Buffer.BlockCopy(infoBytes, startIndex + 1, receiveBytes, 0, endIndex - startIndex - 1);

                var messageBody = Encoding.ASCII.GetString(receiveBytes);
                messagesDict = messageBody;

                if (infoBytes.Length <= endIndex + 1)
                {
                    return;
                }
                var remainByte = new byte[infoBytes.Length - endIndex - 1];
                Buffer.BlockCopy(infoBytes, endIndex + 1, remainByte, 0, infoBytes.Length - endIndex - 1);

                infoBytes = remainByte;
            }
        }
    }
}
