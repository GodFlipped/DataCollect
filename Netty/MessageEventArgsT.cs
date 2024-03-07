using System;

namespace Kengic.Was.Connector.NettyServer
{
    /// <summary>   MessageEventArgs. </summary>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    public class MessageEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs{T}" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageEventArgs(T message)
        {
            Message = message;
        }

        /// <summary>   Gets the message. </summary>
        ///
        /// <value> The message. </value>
        public T Message { get; private set; }
    }
}
