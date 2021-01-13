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

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<SeriesController> _logger;
        private readonly IGraphClient _client;
        private int maxID;

        public UserController(ILogger<SeriesController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
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
                        .Match("(user:User)")
                        .Return(user => user.As<User>())
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] User u)
        {
            maxID = await MaxID();

            var newUser = new User { ID = maxID + 1, Username = u.Username, Password = u.Password };
            var res = _client.Cypher
                        .Create("(user:User $newUser)")
                        .WithParam("newUser", newUser);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
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
    }
}

