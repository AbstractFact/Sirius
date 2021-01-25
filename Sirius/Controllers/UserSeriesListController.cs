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
    public class UserSeriesListController : ControllerBase
    {
        private readonly ILogger<UserSeriesListController> _logger;
        private readonly IGraphClient _client;
        private int maxID;

        public UserSeriesListController(ILogger<UserSeriesListController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Return(l => l.As<UserSeriesList>().ID)
                        .OrderByDescending("l.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        [HttpGet("GetUserSeriesList/{userID}")]
        public async Task<ActionResult> GetUserSeriesList(int userID)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((User u) => u.ID == userID)
                        .Return((u, l, s) => new
                        {
                            Series = s.As<Series>(),
                            l.As<UserSeriesList>().Status
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetSeriesPopularity/{seriesID}")]
        public async Task<ActionResult> GetSeriesPopularity(int seriesID)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((Series s) => s.ID == seriesID)
                        .Return(s => new 
                        {
                            Popularity = All.Count()
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetListed/{id}")]
        public async Task<ActionResult> GetListed(int id)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((UserSeriesList l) => l.ID == id)
                        .Return((u, l, s) => new
                        {
                            l.As<UserSeriesList>().ID,
                            User = u.As<User>(),
                            Series = s.CollectAs<Series>(),
                            l.As<UserSeriesList>().Status
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddSeriesToList/{userID}/{seriesID}/{status}")]
        public async Task<ActionResult> AddSeriesToList(string status, int userID, int seriesID)
        {
            maxID = await MaxID();

            var res = _client.Cypher
                    .Match("(user:User)", "(series:Series)")
                    .Where((User user) => user.ID == userID)
                    .AndWhere((Series series) => series.ID == seriesID)
                    .Create("(user)-[:LISTED { ID: $id, Status: $status }]->(series)")
                    .WithParam("status", status)
                    .WithParam("id", maxID + 1);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPut("{id}/{status}")]
        public async Task<ActionResult> Put(int id, string status)
        {
            var res = _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((UserSeriesList l) => l.ID == id)
                        .Set("l.Status = $status")
                        .WithParam("status", status);

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
                              .Match("(u:User)-[l:LISTED]->(s:Series)")
                              .Where((UserSeriesList l) => l.ID == id)
                              .Delete("l");

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}

