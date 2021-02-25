using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirius.Entities;
using Sirius.DTOs;
using Sirius.Services;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private UserService service;

        public UserController(UserService _service)
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

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            User res = await service.Get(id);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] User user)
        {
            User res = await service.Login(user);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] User u)
        {
            int userID = await service.Post(u);
            if (userID != -1)
                return Ok(userID);
            else
                return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] User user, int id)
        {
            User res = await service.Put(user, id);
            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            int res = await service.Delete(id);
            if(res!=-1)
                return Ok();
            else
                return BadRequest();
        }

        [HttpGet("GetAllFriends/{userID}")]
        public async Task<ActionResult<List<User>>> GetAllFriends(int userID)
        {
            List<User> res = await service.GetAllFriends(userID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("Befriend/{senderID}/{receiverID}/{requestID}")]
        public async Task<ActionResult> Befriend(int senderID, int receiverID, string requestID)
        {
            var res = await service.Befriend(senderID, receiverID, requestID);
            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("Unfriend/{user1ID}/{user2ID}")]
        public async Task<ActionResult> Unfriend(int user1ID, int user2ID)
        {
            var res = await service.Unfriend(user1ID, user2ID);
            if (res != null)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("SendFriendRequest/{receiverUsername}")]
        public async Task<ActionResult> SendFriendRequest([FromBody] Request sender, string receiverUsername)
        {
            int receiverId = await service.GetUserID(receiverUsername);
            if (receiverId == -1)
                return NotFound();

            List<User> friends = await service.GetAllFriends(sender.ID);
            if (friends.Count != 0)
                if (friends.FirstOrDefault(fr => fr.ID == receiverId) != null)
                    return BadRequest();

            IEnumerable<RequestDTO> existingReqs = await service.GetFriendRequests(receiverId);
            if (existingReqs.FirstOrDefault(req => req.Request.ID == sender.ID) != null)
                return NoContent();

            IEnumerable<RequestDTO> receivedReqs = await service.GetFriendRequests(sender.ID);
            if (receivedReqs.FirstOrDefault(req => req.Request.ID == receiverId) != null)
                return NoContent();

            bool res = await service.SendFriendRequest(sender, receiverId);
            return Ok();
        }


        [HttpGet("GetFriendRequests/{receiverId}")]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetFriendRequests(int receiverId)
        {
            IEnumerable<RequestDTO> res = await service.GetFriendRequests(receiverId);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpDelete("DeleteFriendRequest/{receiverId}/{requestId}/{senderId}")]
        public async Task<ActionResult> DeleteFriendRequest(int receiverId, string requestId, int senderId)
        {
            bool res = await service.DeleteFriendRequest(receiverId, requestId, senderId);
            if (res)
                return Ok();
            else
                return BadRequest();
        }

    }
}
