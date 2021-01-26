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
    public class ActorController : ControllerBase
    {
        private readonly ILogger<SeriesController> _logger;
        private readonly IGraphClient _client;
        private int maxID;

        public ActorController(ILogger<SeriesController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(a:Actor)")
                        .Return<int>(a => a.As<Actor>().ID)
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
                        .Match("(actor:Actor)")
                        .Return(actor => actor.As<Actor>())
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetActor/{actorID}")]
        public async Task<ActionResult> GetSeries(int actorID)
        {
            var res = await _client.Cypher
                        .Match("(a:Actor)")
                        .Where((Actor a) => a.ID == actorID)
                        .Return(a => a.As<Actor>())
                        .ResultsAsync;

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Actor a)
        {
            maxID = await MaxID();

            var newActor = new Actor { ID = maxID + 1, Name = a.Name, Sex=a.Sex, Birthplace = a.Birthplace, Birthday = a.Birthday, Biography = a.Biography };
            var res = _client.Cypher
                        .Create("(actor:Actor $newActor)")
                        .WithParam("newActor", newActor);

            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] Actor actor, int id)
        {
            //try
            //{
            var res = _client.Cypher
                              .Match("(a:Actor)")
                              //.Where("ID(s) = {id}")
                              .Where((Actor a) => a.ID == id)
                              //.Where("s.ID = $id")
                              .Set("a = $actor")
                              .WithParam("actor", actor);

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
                              .Match("(a:Actor)")
                              .Where((Actor a) => a.ID == id)
                              .Delete("a");


            await res.ExecuteWithoutResultsAsync();

            if (res != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}

