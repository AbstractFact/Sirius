using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sirius.Entities;
using Sirius.Services;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorController : ControllerBase
    {
        private ActorService service;

        public ActorController(ActorService _service)
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

        [HttpGet("GetActor/{actorID}")]
        public async Task<ActionResult> GetActor(int actorID)
        {
            Actor res = await service.GetActor(actorID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Actor a)
        {
            bool res = await service.Post(a);

            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] Actor actor, int id)
        {
            bool res = await service.Put(actor, id);

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

