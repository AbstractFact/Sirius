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

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<SeriesController> _logger;
        private readonly IGraphClient _client;
        private readonly RedisService _redis_client;
        private int maxID;

        public UserController(ILogger<SeriesController> logger, IGraphClient client, RedisService redis_client)
        {
            _logger = logger;
            _client = client;
            _redis_client = redis_client;

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
                        //.Where((User user) => user.ID == 1234)
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
            //await _redis_client.Set($"{id}","How many claps per person should this article get?");
            //var definitely = await _redis_client.Get($"{id}");
            //return definitely;

            var res = await _client.Cypher
                        .Match("(user:User)")
                        .Where((User user) => user.Username == username)
                        .Return(user => user.As<User>())
                        .ResultsAsync;

            if (res.Count()!=0)
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
            //try
            //{
            var res = _client.Cypher
                              .Match("(u:User)")
                              //.Where("ID(s) = {id}")
                              .Where((Series u) => u.ID == id)
                              //.Where("s.ID = $id")
                              .Set("u = $user")
                              .WithParam("user", user);

            await res.ExecuteWithoutResultsAsync();

            //}
            //catch(Exception e)
            //{
            //    return BadRequest();
            //}
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

        [HttpPost("Befriend/{user1ID}/{user2ID}")]
        public async Task<ActionResult> Befriend(int user1ID, int user2ID)
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
    }
}
