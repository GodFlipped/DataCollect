using System.Threading.Tasks;

namespace Kengic.Infrastructure.Protocol.Rds.Commons
{
    public class CommandAsyncEvent<T>
    {
        private readonly TaskCompletionSource<object> _source;

        public CommandAsyncEvent(T message)
        {
            Message = message;
            _source = new TaskCompletionSource<object>();
        }

        public T Message { get; }

        public Task<object> Task => _source.Task;

        public void Complete(object response)
        {
            _source.TrySetResult(response);
        }
    }
}
