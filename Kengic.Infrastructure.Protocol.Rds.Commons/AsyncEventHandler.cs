using System.Threading.Tasks;

namespace Kengic.Infrastructure.Protocol.Rds.Commons
{
    public delegate Task AsyncEventHandler<in T>(object source, T e);
}
