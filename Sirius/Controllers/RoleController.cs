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
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Return(r => r.As<Role>().ID)
                        .OrderByDescending("r.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        [HttpGet("GetSeriesRoles/{seriesID}")]
        public async Task<ActionResult> GetSeriesRoles(int seriesID)
        {
            var res = await _client.Cypher
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Where((Series s) => s.ID == seriesID)
                        .Return((a, r, s) => new
                        {
                            r.As<Role>().ID,
                            Actor = a.As<Actor>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Role>().InRole
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetActorRoles/{actorID}")]
        public async Task<ActionResult> GetActorRoles(int actorID)
        {
            var res = await _client.Cypher
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Where((Actor a)=>a.ID==actorID)
                        .Return((a, r, s) => new
                        {
                            r.As<Role>().ID,
                            Actor = a.As<Actor>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Role>().InRole
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetRole/{id}")]
        public async Task<ActionResult> GetRole(int id)
        {
            var res = await _client.Cypher
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Where((Role r) => r.ID == id)
                        .Return((a, r, s) => new
                        {
                            r.As<Role>().ID,
                            Actor = a.As<Actor>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Role>().InRole
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddRole/{actorID}/{role}/{seriesID}")]
        public async Task<ActionResult> AddRole(int actorID, String role, int seriesID)
        {
            maxID = await MaxID();

            var res = _client.Cypher
                    .Match("(actor:Actor)", "(series:Series)")
                    .Where((Actor actor) => actor.ID == actorID)
                    .AndWhere((Series series) => series.ID == seriesID)
                    .Create("(actor)-[:IN_ROLE { ID: $id, InRole: $role }]->(series)")
                    .WithParam("role", role)
                    .WithParam("id", maxID+1);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] string role, int id)
        {
            var res = _client.Cypher
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Where((Role r) => r.ID == id)
                        .Set("r.InRole = $role")
                        .WithParam("role", role);

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
                              .Match("(a:Actor)-[r:IN_ROLE]->(s:Series)")
                              .Where((Role r) => r.ID == id)
                              .Delete("r");

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}

