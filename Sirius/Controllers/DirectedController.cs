using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sirius.Services;
using Sirius.Entities;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DirectedController : ControllerBase
    {
        private DirectedService service;

        public DirectedController(DirectedService _service)
        {
            service = _service;
        }

        [HttpGet("GetSeriesRoles/{seriesID}")]
        public async Task<ActionResult> GetSeriesDirector(int seriesID)
        {
            var res = await service.GetSeriesDirector(seriesID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetActorRoles/{actorID}")]
        public async Task<ActionResult> GetDirectorSeries(int actorID)
        {
            var res = await service.GetDirectorSeries(actorID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetRole/{id}")]
        public async Task<ActionResult> GetDirected(int id)
        {
            var res = await service.GetDirected(id);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddDirected/{actorID}/{seriesID}")]
        public async Task<ActionResult> AddDirected(int actorID, int seriesID)
        {
            bool res = await service.AddDirected(actorID, seriesID);
            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] Directed directed, int id)
        {
            bool res = await service.Put(directed, id);
            if (res)
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool res = await service.Delete(id);
            if (res)
                return Ok();
            else
                return BadRequest();
        }
    }
}

