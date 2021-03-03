using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Sirius.Entities;
using Sirius.Services;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private PersonService service;

        public PersonController(PersonService _service)
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

        [HttpGet("GetAllActors")]
        public async Task<ActionResult> GetAllActors()
        {
            var res = await service.GetAllActors();
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetAllDirectors")]
        public async Task<ActionResult> GetAllDirectors()
        {
            var res = await service.GetAllDirectors();
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetPerson/{personID}")]
        public async Task<ActionResult> GetPerson(int personID)
        {
            Person res = await service.GetPerson(personID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Person p)
        {
            bool res = await service.Post(p);

            if (res)
                return Ok();
            else
                return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] Person person, int id)
        {
            bool res = await service.Put(person, id);

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

