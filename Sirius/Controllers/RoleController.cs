using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sirius.Services;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private RoleService service;

        public RoleController(RoleService _service)
        {
            service = _service;
        }

        [HttpGet("GetSeriesRoles/{seriesID}")]
        public async Task<ActionResult> GetSeriesRoles(int seriesID)
        {
            var res = await service.GetSeriesRoles(seriesID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetActorRoles/{actorID}")]
        public async Task<ActionResult> GetActorRoles(int actorID)
        {
            var res = await service.GetActorRoles(actorID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetRole/{id}")]
        public async Task<ActionResult> GetRole(int id)
        {
            var res = await service.GetRole(id);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("AddRole/{actorID}/{role}/{seriesID}")]
        public async Task<ActionResult> AddRole(int actorID, string role, int seriesID)
        {
            bool res = await service.AddRole(actorID, role, seriesID);
            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string role, int id)
        {
            bool res = await service.Put(role, id);
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

