using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services.Redis
{
    public interface IRedisService
    {
        IConnectionMultiplexer Connection { get; }
    }
}
