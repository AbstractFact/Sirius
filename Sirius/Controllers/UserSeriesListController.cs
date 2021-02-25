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
using Sirius.DTOs;

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
                            l.As<UserSeriesList>().ID,
                            Series = s.As<Series>(),
                            l.As<UserSeriesList>().Status,
                            l.As<UserSeriesList>().Stars,
                            l.As<UserSeriesList>().Comment,
                            l.As<UserSeriesList>().Favourite
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetUserFavourites/{userID}")]
        public async Task<ActionResult> GetUserFavourites(int userID)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((User u) => u.ID == userID)
                        .AndWhere((UserSeriesList l) => l.Favourite == true)
                        .Return((s, u) => new
                        {
                            Series = s.As<Series>(),
                            u.As<User>().Username
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetRecommendations/{userID}")]
        public async Task<ActionResult<List<Object>>> GetRecommendations(int userID)
        {
            var frs = await _client.Cypher
                       .OptionalMatch("(user:User)-[FRIENDS]-(friend:User)")
                       .Where((User user) => user.ID == userID)
                       .Return(friend => friend.As<User>())
                       .ResultsAsync;

            var friends = frs.ToList();
            List<Object> res = new List<Object>();
            foreach(User fr in friends)
            {
                var rs = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((User u) => u.ID == fr.ID)
                        .AndWhere((UserSeriesList l) => l.Favourite == true)
                        .Return((s, u) => new
                        {
                            Series = s.As<Series>(),
                            u.As<User>().Username
                        })
                        .ResultsAsync;

                rs.ToList().ForEach(rec =>
                {
                    res.Add(rec);
                });
            };

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
                            l.As<UserSeriesList>().Status,
                            l.As<UserSeriesList>().Stars,
                            l.As<UserSeriesList>().Comment,
                            l.As<UserSeriesList>().Favourite
                        })
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        public async Task<int> GetListedSeries(int userID, int seriesID)
        {
            var res = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((Series s) => s.ID == seriesID)
                        .AndWhere((User u) => u.ID == userID)
                        .Return((u, l, s) => new
                        {
                            l.As<UserSeriesList>().ID,
                        })
                        .ResultsAsync;

            if (res.Count()!=0)
                return res.FirstOrDefault().ID;
            else
                return -1;
        }

        [HttpPost("AddSeriesToList/{userID}/{seriesID}/{status}/{fav}")]
        public async Task<ActionResult> AddSeriesToList(string status, bool fav, int userID, int seriesID)
        {
            maxID = await MaxID();

            int tmp = await GetListedSeries(userID, seriesID);

            if (tmp == -1)
            {
                var res = _client.Cypher
                        .Match("(user:User)", "(series:Series)")
                        .Where((User user) => user.ID == userID)
                        .AndWhere((Series series) => series.ID == seriesID)
                        .Create("(user)-[:LISTED { ID: $id, Status: $status, Stars: 0, Comment: \"\", Favourite: $fav}]->(series)")
                        .WithParam("status", status)
                        .WithParam("fav", fav)
                        .WithParam("id", maxID + 1);

                await res.ExecuteWithoutResultsAsync();

                if (res != null)
                    return Ok();
                else
                    return BadRequest();
            }
            else
                return BadRequest();
        }

        [HttpPut("{id}/{seriesID}")]
        public async Task<ActionResult> Put([FromBody] FavouriteSeriesEditDTO data, int id, int seriesID)
        {
            var res = _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((UserSeriesList l) => l.ID == id)
                        .Set("l.Status = $status")
                        .Set("l.Stars = $stars")
                        .Set("l.Comment = $comment")
                        .Set("l.Favourite = $favourite")
                        .WithParam("status", data.Status)
                        .WithParam("stars", data.Stars)
                        .WithParam("comment", data.Comment)
                        .WithParam("favourite", data.Favourite);

            await res.ExecuteWithoutResultsAsync();

            if(data.Stars!=0)
            {
                float avgrtng = await GetSeriesAvgRating(seriesID);
                await UpdateRating(seriesID, avgrtng);
            }
          
            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

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

        [HttpGet("GetSeriesAvgRating/{seriesID}")]
        public async Task<float> GetSeriesAvgRating(int seriesID)
        {
            var res = await _client.Cypher
                            .Match("(u:User)-[l:LISTED]-(s:Series)")
                            .Where((Series s) => s.ID == seriesID)
                            .AndWhere((UserSeriesList l) => l.Stars != 0)
                            .Return((l) => Return.As<string>("avg(l.Stars)"))
                            .ResultsAsync;

            if (res.FirstOrDefault() != "null")
                return float.Parse(res.FirstOrDefault());
            else
                return 0;
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

