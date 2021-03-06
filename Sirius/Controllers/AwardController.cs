using Microsoft.AspNetCore.Mvc;
using Sirius.Entities;
using Sirius.DTOs;
using Sirius.Services;
using System.Threading.Tasks;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AwardController : ControllerBase
    {
        private AwardService service;

        public AwardController(AwardService _service)
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

        [HttpGet("GetAward/{awardID}")]
        public async Task<ActionResult> GetAward(int awardID)
        {
            AwardDTO res = await service.GetAward(awardID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Award a)
        {
            bool res = await service.Post(a);

            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] AwardDTO award, int id)
        {
            bool res = await service.Put(award, id);

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

        [HttpPost("GetAwardsFiltered/{name}")]
        public async Task<ActionResult> GetAwardsFiltered(string name)
        {
            var res = await service.GetAwardsFiltered(name);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }
    }
}

