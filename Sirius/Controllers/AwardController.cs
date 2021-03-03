using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sirius.Entities;
using Sirius.Services;

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
            Award res = await service.GetAward(awardID);
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
        public async Task<ActionResult> Put([FromBody] Award award, int id)
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

