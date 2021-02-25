using StackExchange.Redis;

namespace Sirius.Services.Redis
{
    public interface IRedisService
    {
        IConnectionMultiplexer Connection { get; }
    }
}
