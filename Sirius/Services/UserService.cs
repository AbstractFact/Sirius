using Microsoft.AspNetCore.SignalR;
using Neo4jClient;
using Sirius.DTOs;
using Sirius.Entities;
using Sirius.Hubs;
using Sirius.Services.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class UserService
    {
        private readonly IGraphClient _client;
        private readonly IHubContext<MessageHub> _hub;
        private readonly IConnectionMultiplexer _redisConnection;
        private int maxID;

        public UserService(IGraphClient client, IRedisService builder, IHubContext<MessageHub> hub)
        {
            _client = client;
            _redisConnection = builder.Connection;
            _hub = hub;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            try
            {
                var query = await _client.Cypher
                        .Match("(u:User)")
                        .Return<int>(u => u.As<User>().ID)
                        .OrderByDescending("u.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public async Task<Object> GetAll()
        {
            try
            {
                var res = await _client.Cypher
                        .OptionalMatch("(user:User)-[FRIENDS]-(friend:User)")
                        .Return((user, friend) => new
                        {
                            User = user.As<User>(),
                            Friends = friend.CollectAs<User>()
                        }).ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }    
        }

        public async Task<User> Get(int id)
        {
            try
            {
                var res = await _client.Cypher
                     .Match("(user:User)")
                     .Where((User user) => user.ID == id)
                     .Return(user => user.As<User>())
                     .ResultsAsync;

                return res.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<int> GetUserID(string username)
        {
            var res = await _client.Cypher
                        .Match("(user:User)")
                        .Where((User user) => user.Username == username)
                        .Return(user => user.As<User>())
                        .ResultsAsync;

            if (res.Count() != 0)
                return res.FirstOrDefault().ID;
            else
                return -1;
        }
        public async Task<string> GetUserUsername(int userID)
        {

            var res = await _client.Cypher
                        .Match("(user:User)")
                        .Where((User user) => user.ID == userID)
                        .Return(user => user.As<User>())
                        .ResultsAsync;

            if (res.Count() != 0)
                return res.FirstOrDefault().Username;
            else
                return null;
        }

        public async Task<User> Login(User user)
        {
            try
            {
                var res = await _client.Cypher
                      .Match("(u:User)")
                      .Where((User u) => u.Username == user.Username)
                      .AndWhere((User u) => u.Password == user.Password)
                      .Return(u => u.As<User>())
                      .ResultsAsync;

                return res.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<User> Post(User u)
        {
            maxID = await MaxID();
            int id = await GetUserID(u.Username);

            if (id == -1 && maxID!=-1)
            {
                try
                {
                    var newUser = new User { ID = maxID + 1, Username = u.Username, Password = u.Password };
                    var res = _client.Cypher.Create("(user:User $newUser)")
                                            .WithParam("newUser", newUser);

                    await res.ExecuteWithoutResultsAsync();

                    return newUser;
                }
                catch(Exception e)
                {
                    return null;
                }
            }
            else
                return null;
        }

        public async Task<User> Put(User user, int id)
        {
            try
            {
                var res = _client.Cypher
                             .Match("(u:User)")
                             .Where((Series u) => u.ID == id)
                             .Set("u = $user")
                             .WithParam("user", user);

                await res.ExecuteWithoutResultsAsync();
                return user;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<int> Delete(int id)
        {
            try
            {
                var res = _client.Cypher
                             .Match("(u:User)")
                             .Where((User u) => u.ID == id)
                             .Delete("u");

                await res.ExecuteWithoutResultsAsync();
                return id;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public async Task<List<User>> GetAllFriends(int userID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(user:User)-[FRIENDS]-(friend:User)")
                       .Where((User user) => user.ID == userID)
                       .Return(friend => friend.As<User>())
                       .ResultsAsync;
                return res.ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<object> Befriend(int senderID, int receiverID, string requestID)
        {
            try
            {
                string username = await GetUserUsername(receiverID);
                string groupName = $"channel:{senderID}";
                _ = _hub.Clients.Group(groupName).SendAsync("FriendRequestAccepted", username + " accepted request!");

                await DeleteFriendRequest(receiverID, requestID, senderID);
                var res = _client.Cypher
                        .Match("(user1:User)", "(user2:User)")
                        .Where((User user1) => user1.ID == senderID)
                        .AndWhere((User user2) => user2.ID == receiverID)
                        .Merge("(user1)-[:FRIENDS]->(user2)");

                await res.ExecuteWithoutResultsAsync();

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
          
        }

        public async Task<object> Unfriend(int user1ID, int user2ID)
        {
            try
            {
                var res = _client.Cypher
                    .Match("(user1:User)-[f:FRIENDS]-(user2:User)")
                    .Where((User user1) => user1.ID == user1ID)
                    .AndWhere((User user2) => user2.ID == user2ID)
                    .Delete("f");

                await res.ExecuteWithoutResultsAsync();

                return res;
            }
            catch (Exception e)
            {
                return null;
            }  
        }

        public async Task<bool> SendFriendRequest(Request sender, int receiverId)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            var values = new NameValueEntry[]
            {
                new NameValueEntry("sender_id", sender.ID),
                new NameValueEntry("sender_username", sender.Username)
            };

            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();
                var messageId = await redisDB.StreamAddAsync(channelName, values);

                await redisDB.SetAddAsync("friend:" + sender.ID + ":request", receiverId);

                FriendRequestNotificationDTO message = new FriendRequestNotificationDTO
                {
                    ReceiverId = receiverId,
                    RequestDTO = new RequestDTO { ID = messageId, Request = sender }
                };

                var jsonMessage = JsonSerializer.Serialize(message);
                ISubscriber chatPubSub = _redisConnection.GetSubscriber();
                await chatPubSub.PublishAsync("friendship.requests", jsonMessage);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<IEnumerable<RequestDTO>> GetFriendRequests(int receiverId)
        {
            string channelName = $"messages:{receiverId}:friend_request";
            IDatabase redisDB = _redisConnection.GetDatabase();

            var requests = await redisDB.StreamReadAsync(channelName, "0-0");

            IList<RequestDTO> result = new List<RequestDTO>();

            foreach (var request in requests)
            {
                result.Add(
                    new RequestDTO
                    {
                        ID = request.Id,
                        Request = new Request
                        {
                            ID = int.Parse(request.Values.FirstOrDefault(value => value.Name == "sender_id").Value),
                            Username = request.Values.FirstOrDefault(value => value.Name == "sender_username").Value
                        }

                    }
                );
            }

            return result;
        }

        public async Task<bool> DeleteFriendRequest(int receiverId, string requestId, int senderId)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();
                long deletedMessages = await redisDB.StreamDeleteAsync(channelName, new RedisValue[] { new RedisValue(requestId) });
                await redisDB.SetRemoveAsync("friend:" + senderId + ":request", receiverId);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SubsribeToGenre(int userID, string genre)
        {
            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();
                await redisDB.SetAddAsync("genre:" + genre + ":subsriber", userID);
                await redisDB.SetAddAsync("user:" + userID + ":subsciptions", genre);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> UnsubscribeFromGenre(int userID, string genre)
        {
            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();
                await redisDB.SetRemoveAsync("genre:" + genre + ":subsriber", userID);
                await redisDB.SetRemoveAsync("user:" + userID + ":subsciptions", genre);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRecommendation(int userID, RecommendationDTO message)
        {
            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();
                await redisDB.SetRemoveAsync("user:" + userID + ":recommendations", JsonSerializer.Serialize(message));

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<string>> GetUserSubsciptions(int userID)
        {
            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();
                var result = await redisDB.SetMembersAsync("user:" + userID + ":subsciptions");

                List<string> arr = new List<string>();

                foreach (var res in result)
                    arr.Add(Convert.ToString(res));

                return arr;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<RecommendationDTO>> GetUserRecommendations(int userID)
        {
            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();
                var result = await redisDB.SetMembersAsync("user:" + userID + ":recommendations");

                List<RecommendationDTO> arr = new List<RecommendationDTO>();

                foreach (var res in result)
                    arr.Add(JsonSerializer.Deserialize<RecommendationDTO>(res));

                return arr;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
