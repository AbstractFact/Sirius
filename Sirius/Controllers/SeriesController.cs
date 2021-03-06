using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sirius.Entities;
using Sirius.Services;
using Sirius.DTOs;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeriesController : ControllerBase
    {
        private SeriesService service;
        private DirectedService directedService;

        public SeriesController(SeriesService _service, DirectedService _directedService)
        {
            service = _service;
            directedService = _directedService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var res = await service.GetAll();
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetSeries/{seriesID}")]
        public async Task<ActionResult> GetSeries(int seriesID)
        {
            SeriesDTO res = await service.GetSeries(seriesID);

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("{directorID}")]
        public async Task<ActionResult> Post([FromBody] Series s, int directorID)
        {
            int seriesID = await service.Post(s);
            bool res = false;
            if(seriesID!=-1)
                res = await directedService.AddDirected(directorID, seriesID);

            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] SeriesDTO series, int id)
        {
            bool res = await service.Put(series, id);

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

        [HttpGet("GetBestRatedSeries")]
        public async Task<ActionResult> GetBestRatedSeries()
        {
            var res = await service.GetBestRatedSeries();

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("GetSeriesFiltered")]
        public async Task<ActionResult> GetSeriesFiltered([FromBody] SeriesFilterDTO filter)
        {
            var res = await service.GetSeriesFiltered(filter);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }
    }
}

