using System;

namespace Kengic.Infrastructure.Protocol.Rds.Commons
{
    public class ExceptionEventArgs<T> : EventArgs where T : Exception
    {
        public ExceptionEventArgs(T exception)
        {
            Exception = exception;
        }

        public T Exception { get; private set; }
    }
}
