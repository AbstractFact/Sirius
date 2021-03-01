using System;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Sirius.DTOs;
using Sirius.Hubs;
using StackExchange.Redis;

namespace Sirius.Services.Redis
{
    public class RedisService : IRedisService
    {
        private static IConnectionMultiplexer _connection = null;
        private static object _objectLock = new object();
        private static string ConnectionString;
        private readonly IHubContext<MessageHub> _hub;

        public RedisService(IConfiguration config, IHubContext<MessageHub> hub)
        {
            string _redisHost = config["Redis:Host"];
            int _redisPort = Convert.ToInt32(config["Redis:Port"]);
            ConnectionString = _redisHost + ":" +_redisPort;
            _hub = hub;
        }

        public IConnectionMultiplexer Connection
        {
            get
            {
                if (_connection == null)
                {
                    lock (_objectLock)
                    {
                        if (_connection == null)
                        {
                            _connection = ConnectionMultiplexer.Connect(ConnectionString);
                            var redisPubSub = _connection.GetSubscriber();

                            redisPubSub.Subscribe("friendship.requests").OnMessage(message =>
                            {
                                FriendRequestNotificationDTO deserializedMessage = JsonSerializer.Deserialize<FriendRequestNotificationDTO>(message.Message);
                                string groupName = $"channel:{deserializedMessage.ReceiverId}";
                                _ = _hub.Clients.Group(groupName).SendAsync("ReceiveFriendRequests", deserializedMessage);
                            });

                            redisPubSub.Subscribe("genre.recommendations").OnMessage(message =>
                            {
                                RecommendationDTO deserializedMessage = JsonSerializer.Deserialize<RecommendationDTO>(message.Message);
                                string groupName = $"channel:{deserializedMessage.Genre}";
                                _ = _hub.Clients.Group(groupName).SendAsync("ReceiveRecommendations", deserializedMessage);
                            });
                        }
                    }
                }
                return _connection;
            }
        }
    }
}

