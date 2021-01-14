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
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IGraphClient _client;
        private int maxID;

        public RoleController(ILogger<RoleController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(a:Role)")
                        .Return<int>(a => a.As<Role>().ID)
                        .OrderByDescending("a.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var res = await _client.Cypher
                        .Match("(actor:Role)")
                        .Return(actor => actor.As<Role>())
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddRole/{actorID}/{role}/{seriesID}")]
        public async Task<ActionResult> AddRole(int actorID, String role, int seriesID)
        {
            var res = _client.Cypher
                    .Match("(actor:Actor)", "(series:Series)")
                    .Where((Actor actor) => actor.ID == actorID)
                    .AndWhere((Series series) => series.ID == seriesID)
                    .Create("(actor)-[:IN_ROLE { InRole: $role }]->(series)")
                    .WithParam("role", role);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }


        //[HttpPut("{id}")]
        //public async Task<ActionResult> Put([FromBody] Actor actor, int id)
        //{
        //    //try
        //    //{
        //    var res = _client.Cypher
        //                      .Match("(a:Actor)")
        //                      //.Where("ID(s) = {id}")
        //                      .Where((Actor a) => a.ID == id)
        //                      //.Where("s.ID = $id")
        //                      .Set("a = $actor")
        //                      .WithParam("actor", actor);

        //    await res.ExecuteWithoutResultsAsync();

        //    //}
        //    //catch(Exception e)
        //    //{
        //    //    return BadRequest();
        //    //}
        //    if (res != null)
        //        return Ok();
        //    else
        //        return BadRequest();
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var res = _client.Cypher
        //                      .Match("(a:Actor)")
        //                      .Where((Actor a) => a.ID == id)
        //                      .Delete("a");


        //    await res.ExecuteWithoutResultsAsync();

        //    if (res != null)
        //        return Ok();
        //    else
        //        return BadRequest();
        //}
    }
}

