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

        [HttpGet("GetSeriesWithDirector/{seriesID}")]
        public async Task<ActionResult> GetSeriesWithDirector(int seriesID)
        {
            var res = await service.GetSeriesWithDirector(seriesID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetDirectorSeries/{directorID}")]
        public async Task<ActionResult> GetDirectorSeries(int directorID)
        {
            var res = await service.GetDirectorSeries(directorID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetDirected/{id}")]
        public async Task<ActionResult> GetDirected(int id)
        {
            var res = await service.GetDirected(id);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddDirected/{directorID}/{seriesID}")]
        public async Task<ActionResult> AddDirected(int directorID, int seriesID)
        {
            bool res = await service.AddDirected(directorID, seriesID);
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

