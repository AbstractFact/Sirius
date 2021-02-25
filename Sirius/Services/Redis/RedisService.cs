﻿using System;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Sirius.DTOs;
using Sirius.Entities;
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
                            //redisPubSub.Subscribe("sirius.messages").OnMessage(message =>
                            //{
                            //    Message deserializedMessage = JsonSerializer.Deserialize<Message>(message.Message);
                            //    string groupName = $"channel:{deserializedMessage.ReceiverId}";
                            //    _ = _hub.Clients.Group(groupName).SendAsync("ReceiveMessage", deserializedMessage);
                            //});

                            //var subPatternChannel = new RedisChannel("__keyevent@0__:*", RedisChannel.PatternMode.Pattern);
                            //redisPubSub.Subscribe(subPatternChannel).OnMessage(message =>
                            //{
                            //    string str = message.Channel;
                            //    if (str == "__keyevent@0__:expired" || str == "__keyevent@0__:del")
                            //    {
                            //        string keyName = message.Message;
                            //        string[] keyNameParts = keyName.Split(':');
                            //        if (keyNameParts.Length == 4 && keyNameParts[0] == "messages" && keyNameParts[3] == "sirius")
                            //        {
                            //            int biggerId = int.Parse(keyNameParts[1]), smallerId = int.Parse(keyNameParts[2]);
                            //            string setKeyBigger = $"user:{biggerId}:chats";
                            //            string setKeySmaller = $"user:{smallerId}:chats";
                            //            IDatabase redisDB = _connection.GetDatabase();
                            //            var setEntriesBigger = redisDB.SortedSetRangeByRank(setKeyBigger, 0, -1, Order.Descending);
                            //            var setEntriesSmaller = redisDB.SortedSetRangeByRank(setKeySmaller, 0, -1, Order.Descending);

                            //            foreach (var entry in setEntriesBigger)
                            //            {
                            //                User user = JsonSerializer.Deserialize<User>(entry);
                            //                if (user.ID == smallerId)
                            //                {
                            //                    redisDB.SortedSetRemove(setKeyBigger, JsonSerializer.Serialize(user));
                            //                    break;
                            //                }
                            //            }

                            //            foreach (var entry in setEntriesSmaller)
                            //            {
                            //                User user = JsonSerializer.Deserialize<User>(entry);
                            //                if (user.ID == biggerId)
                            //                {
                            //                    redisDB.SortedSetRemove(setKeySmaller, JsonSerializer.Serialize(user));
                            //                    break;
                            //                }
                            //            }
                            //        }

                            //    }
                            //});

                            redisPubSub.Subscribe("friendship.requests").OnMessage(message =>
                            {
                                FriendRequestNotificationDTO deserializedMessage = JsonSerializer.Deserialize<FriendRequestNotificationDTO>(message.Message);
                                string groupName = $"channel:{deserializedMessage.ReceiverId}";
                                _ = _hub.Clients.Group(groupName).SendAsync("ReceiveFriendRequests", deserializedMessage);
                            });

                            redisPubSub.Subscribe("genre.recommendations").OnMessage(message =>
                            {
                                NewSeriesNotificationDTO deserializedMessage = JsonSerializer.Deserialize<NewSeriesNotificationDTO>(message.Message);
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
