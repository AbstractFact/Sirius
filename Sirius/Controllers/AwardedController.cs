using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sirius.Services;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AwardedController : ControllerBase
    {
        private AwardedService service;

        public AwardedController(AwardedService _service)
        {
            service = _service;
        }

        [HttpGet("GetSeriesAwards/{seriesID}")]
        public async Task<ActionResult> GetSeriesAwards(int seriesID)
        {
            var res = await service.GetSeriesAwards(seriesID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetAwardedSeries/{awardID}")]
        public async Task<ActionResult> GetAwardedSeries(int awardID)
        {
            var res = await service.GetAwardedSeries(awardID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetRole/{id}")]
        public async Task<ActionResult> GetAwarded(int id)
        {
            var res = await service.GetAwarded(id);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddAwardSeries/{awardID}/{year}/{seriesID}")]
        public async Task<ActionResult> AddAwardSeries(int awardID, int year, int seriesID)
        {
            bool res = await service.AddAwardSeries(awardID, year, seriesID);
            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}/{year}")]
        public async Task<ActionResult> Put(int id, int year)
        {
            bool res = await service.Put(year, id);
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

