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
    public class RatingController : ControllerBase
    {
        private readonly ILogger<RatingController> _logger;
        private readonly IGraphClient _client;
        private int maxID;

        public RatingController(ILogger<RatingController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(u:User)-[r:RATING]-(s:Series)")
                        .Return(r => r.As<Rating>().ID)
                        .OrderByDescending("r.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        [HttpGet("GetUserRatings/{userID}")]
        public async Task<ActionResult> GetUserRatings(int userID)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[r:RATING]-(s:Series)")
                        .Where((User u) => u.ID == userID)
                        .Return((u, r, s) => new
                        {
                            r.As<Rating>().ID,
                            User = u.As<User>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Rating>().Stars,
                            r.As<Rating>().Comment
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetSeriesAvgRating/{seriesID}")]
        public async Task<float> GetSeriesAvgRating(int seriesID)
        {
            var res = await _client.Cypher
                            .Match("(u:User)-[r:RATING]-(s:Series)")
                            .Where((Series s) => s.ID == seriesID)
                            .Return((r) => Return.As<float>("avg(r.Stars)"))
                            .ResultsAsync;

            return res.FirstOrDefault();
        }

        [HttpGet("GetSeriesRatings/{seriesID}")]
        public async Task<ActionResult> GetSeriesRatings(int seriesID)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[r:RATING]-(s:Series)")
                        .Where((Series s) => s.ID == seriesID)
                        .Return((u, r, s) => new
                        {
                            r.As<Rating>().ID,
                            User = u.As<User>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Rating>().Stars,
                            r.As<Rating>().Comment
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetRating/{id}")]
        public async Task<ActionResult> GetRating(int id)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[r:RATING]-(s:Series)")
                        .Where((Rating r) => r.ID == id)
                        .Return((u, r, s) => new
                        {
                            r.As<Rating>().ID,
                            User = u.As<User>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Rating>().Stars,
                            r.As<Rating>().Comment,
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddRating/{userID}/{seriesID}/{stars}")]
        public async Task<ActionResult> AddRating([FromBody] string comment, int userID, int seriesID, int stars)
        {
            maxID = await MaxID();

            var res = _client.Cypher
                    .Match("(user:User)", "(series:Series)")
                    .Where((User user) => user.ID == userID)
                    .AndWhere((Series series) => series.ID == seriesID)
                    .Merge("(user)-[:RATING { ID: $id, Stars: $stars, Comment: $comment }]->(series)")
                    .WithParam("comment", comment)
                    .WithParam("stars", stars)
                    .WithParam("id", maxID+1);

            await res.ExecuteWithoutResultsAsync();

            float avgrtng = await GetSeriesAvgRating(seriesID);

            await UpdateRating(seriesID, avgrtng);

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPut("{id}/{stars}")]
        public async Task<ActionResult> Put([FromBody] string comment, int stars, int id)
        {
            var res = _client.Cypher
                        .Match("(u:User)-[r:RATING]-(s:Series)")
                        .Where((Rating r) => r.ID == id)
                        .Set("r.Comment = $comment")
                        .Set("r.Stars = $stars")
                        .WithParam("comment", comment)
                        .WithParam("stars", stars);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("UpdateRating/{id}/{rating}")]
        public async Task<ActionResult> UpdateRating(int id, float rating)
        {
            var res = _client.Cypher
                                .Match("(s:Series)")
                                .Where((Series s) => s.ID == id)
                                .Set("s.Rating = $rating")
                                .WithParam("rating", rating);

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
                              .Match("(u:User)-[r:RATING]->(s:Series)")
                              .Where((Rating r) => r.ID == id)
                              .Delete("r");

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}

