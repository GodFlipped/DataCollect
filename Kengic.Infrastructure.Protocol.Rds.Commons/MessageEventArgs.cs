using System;

namespace Kengic.Infrastructure.Protocol.Rds.Commons
{
    public class MessageEventArgs<T> : EventArgs
    {
        public MessageEventArgs(T message)
        {
            Message = message;
        }

        public T Message { get; private set; }
    }
}
