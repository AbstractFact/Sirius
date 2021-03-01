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

        public SeriesController(SeriesService _service)
        {
            service = _service;
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
            Series res = await service.GetSeries(seriesID);

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Series s)
        {
            bool res = await service.Post(s);

            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] Series series, int id)
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
    }
}

