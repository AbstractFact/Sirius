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
    public class SeriesController : ControllerBase
    {
        private readonly ILogger<SeriesController> _logger;
        private readonly IGraphClient _client;
        private int maxID;

        public SeriesController(ILogger<SeriesController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(s:Series)")
                        .Return<int>(s => s.As<Series>().ID)
                        .OrderByDescending("s.ID")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var res = await _client.Cypher
                        .Match("(series:Series)")
                        .Return(series => series.As<Series>())
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetSeries/{seriesID}")]
        public async Task<ActionResult> GetSeries(int seriesID)
        {
            var res = await _client.Cypher
                        .Match("(s:Series)")
                        .Where((Series s) => s.ID == seriesID)
                        .Return(s => s.As<Series>())
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Series s)
        {
            maxID = await MaxID();

            var newSeries = new Series { ID = maxID + 1, Title = s.Title, Year = s.Year, Genre= s.Genre, Plot=s.Plot, Seasons=s.Seasons, Rating=0.0f};
            var res = _client.Cypher
                        .Create("(series:Series $newSeries)")
                        .WithParam("newSeries", newSeries);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] Series series, int id)
        {
            var res = _client.Cypher
                                .Match("(s:Series)")
                                .Where((Series s) => s.ID == id)
                                .Set("s = $series")
                                .WithParam("series", series);

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
                              .Match("(s:Series)")
                              .Where((Series s) => s.ID == id)
                              .Delete("s");
                              
            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}

