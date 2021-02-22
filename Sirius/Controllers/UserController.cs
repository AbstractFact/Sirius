using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirius.Entities;
using Neo4jClient.Cypher;
using Sirius.Services;
using StackExchange.Redis;
using Sirius.DTOs;
using System.Text.Json;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<SeriesController> _logger;
        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redisConnection;
        private int maxID;

        public UserController(ILogger<SeriesController> logger, IGraphClient client, IRedisService builder)
        {
            _logger = logger;
            _client = client;
            _redisConnection = builder.Connection;

            maxID = 0;
        }

        private async Task<int> MaxID()
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

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var res = await _client.Cypher
                        .OptionalMatch("(user:User)-[FRIENDS]-(friend:User)")
                        .Return((user, friend) => new
                        {
                            User = user.As<User>(),
                            Friends = friend.CollectAs<User>()
                        }).ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            //await _redis_client.Set($"{id}","How many claps per person should this article get?");
            //var definitely = await _redis_client.Get($"{id}");
            //return definitely;

            var res = await _client.Cypher
                        .Match("(user:User)")
                        .Where((User user) => user.ID == id)
                        .Return(user => user.As<User>())
                        .ResultsAsync;

            return res.FirstOrDefault();
        }

        [HttpGet("GetUserID/{username}")]
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

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] User user)
        {
            var res = await _client.Cypher
                        .Match("(u:User)")
                        .Where((User u) => u.Username == user.Username)
                        .AndWhere((User u) => u.Password == user.Password)
                        .Return(u => u.As<User>())
                        .ResultsAsync;

            if (res.Count() != 0)
                return Ok(res.FirstOrDefault());
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] User u)
        {
            maxID = await MaxID();

            int id = await GetUserID(u.Username);

            if (id == -1)
            {
                var newUser = new User { ID = maxID + 1, Username = u.Username, Password = u.Password };

                var res = _client.Cypher.Create("(user:User $newUser)")
                                        .WithParam("newUser", newUser);

                await res.ExecuteWithoutResultsAsync();

                if (res != null)
                {
                    id = await GetUserID(u.Username);
                    return Ok(id);
                }
                else
                    return BadRequest();
            }
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] User user, int id)
        {
            var res = _client.Cypher
                              .Match("(u:User)")
                              .Where((Series u) => u.ID == id)
                              .Set("u = $user")
                              .WithParam("user", user);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var res = _client.Cypher
                              .Match("(u:User)")
                              .Where((User u) => u.ID == id)
                              .Delete("u");

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpGet("GetAllFriends/{userID}")]
        public async Task<List<User>> GetAllFriends(int userID)
        {
            var res = await _client.Cypher
                        .OptionalMatch("(user:User)-[FRIENDS]-(friend:User)")
                        .Where((User user) => user.ID == userID)
                        .Return(friend => friend.As<User>())
                        .ResultsAsync;

            if (res != null)
                return res.ToList();
            else
                return null;
        }

        [HttpPost("Befriend/{user1ID}/{user2Username}")]
        public async Task<ActionResult> Befriend(int user1ID, string user2Username)
        {
            int user2ID = await GetUserID(user2Username);

            if (user2ID != -1)
            {
                var res = _client.Cypher
                        .Match("(user1:User)", "(user2:User)")
                        .Where((User user1) => user1.ID == user1ID)
                        .AndWhere((User user2) => user2.ID == user2ID)
                        .Merge("(user1)-[:FRIENDS]->(user2)");

                await res.ExecuteWithoutResultsAsync();

                if (res != null)
                    return Ok();
                else
                    return BadRequest();
            }
            else
                return BadRequest();
        }

        [HttpDelete("Unfriend/{user1ID}/{user2ID}")]
        public async Task<ActionResult> Unfriend(int user1ID, int user2ID)
        {
            var res = _client.Cypher
                    .Match("(user1:User)-[f:FRIENDS]->(user2:User)")
                    .Where((User user1) => user1.ID == user1ID)
                    .AndWhere((User user2) => user2.ID == user2ID)
                    .Delete("f");

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("SendFriendRequest/{senderId}/{receiverUsername}")]
        public async Task SendFriendRequest([FromBody] Request sender, int senderId, string receiverUsername)
        {
            int receiverId = await GetUserID(receiverUsername);
            string channelName = $"messages:{receiverId}:friend_request";

            var values = new NameValueEntry[]
            {
                new NameValueEntry("sender_id", senderId),
                new NameValueEntry("sender_username", sender.Username)
            };

            IDatabase redisDB = _redisConnection.GetDatabase();
            var messageId = await redisDB.StreamAddAsync(channelName, values);

            //Dodatna funkcija
            await redisDB.SetAddAsync("friend:" + senderId + ":request", receiverId);

            // objekat za notifikaciju
            FriendRequestNotificationDTO message = new FriendRequestNotificationDTO
            {
                ReceiverId = receiverId,
                RequestDTO = new RequestDTO { ID = messageId, Request = sender }
            };

            //Push notifikacija
            var jsonMessage = JsonSerializer.Serialize(message);
            ISubscriber chatPubSub = _redisConnection.GetSubscriber();
            await chatPubSub.PublishAsync("friendship.requests", jsonMessage);
        }


        [HttpGet("GetFriendRequests/{receiverId}")]
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

        [HttpGet("GetFriendRequestSends/{senderId}")]
        public async Task<IEnumerable<int>> GetFriendRequestSends(int senderId)
        {
            IDatabase redisDB = _redisConnection.GetDatabase();
            var result = await redisDB.SetMembersAsync("friend:" + senderId + ":request");

            IList<int> arr = new List<int>();

            foreach (var res in result)
                arr.Add(Convert.ToInt32(res));

            return arr;
        }

        [HttpDelete("DeleteFriendRequest/{receiverId}/{requestId}/{senderId}")]
        public async Task DeleteFriendRequest(int receiverId, string requestId, int senderId)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            IDatabase redisDB = _redisConnection.GetDatabase();
            long deletedMessages = await redisDB.StreamDeleteAsync(channelName, new RedisValue[] { new RedisValue(requestId) });
            await redisDB.SetRemoveAsync("friend:" + senderId + ":request", receiverId);
        }
    }
}
