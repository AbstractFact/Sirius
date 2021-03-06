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
        private UserSeriesListService userListService;

        public UserController(UserService _service, UserSeriesListService _userListService)
        {
            service = _service;
            userListService = _userListService;
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
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            UserDTO res = await service.Get(id);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] User user)
        {
            UserDTO res = await service.Login(user);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromBody] User u)
        {
            UserDTO user = await service.Post(u);
            if (user != null)
                return Ok(user);
            else
                return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> Put([FromBody] UserDTO user, int id)
        {
            UserDTO res = await service.Put(user, id);
            if (res != null)
                return Ok(res);
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
        public async Task<ActionResult<List<UserDTO>>> GetAllFriends(int userID)
        {
            List<UserDTO> res = await service.GetAllFriends(userID);
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
        public async Task<ActionResult> SendFriendRequest([FromBody] SendFriendRequestDTO sender, string receiverUsername)
        {
            int receiverId = await service.GetUserID(receiverUsername);
            if (receiverId == -1)
                return NotFound();

            List<UserDTO> friends = await service.GetAllFriends(sender.ID);
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

        [HttpPost("Subsribe/{userID}")]
        public async Task<ActionResult> Subsribe([FromBody] SubscribeDTO list, int userID)
        {
            bool res=true;
            foreach(string el in list.SubList)
            {
                res = res && (await service.SubsribeToGenre(userID, el));
            }

            foreach (string el in list.UnsubList)
            {
                res = res && (await service.UnsubscribeFromGenre(userID, el));
            }

            if (res)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("UnsubscribeFromGenre/{userID}/{genre}")]
        public async Task<ActionResult> UnsubscribeFromGenre(int userID, string genre)
        {
            bool res = await service.UnsubscribeFromGenre(userID, genre);

            if (res)
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete("DeleteRecommendation/{userID}")]
        public async Task<ActionResult> DeleteRecommendation([FromBody] RecommendationDTO message, int userID)
        {
            bool res = await service.DeleteRecommendation(userID, message);

            if (res)
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost("AcceptRecommendation/{userID}")]
        public async Task<ActionResult> AcceptRecommendation([FromBody] RecommendationDTO message, int userID)
        {
            bool res1 = await userListService.AddSeriesToList("Plan to Watch", false, userID, message.SeriesID);
            bool res2 = false;
            if(res1)
                res2 = await service.DeleteRecommendation(userID, message);

            if (res2)
                return Ok();
            else
                return BadRequest();
        }

        [HttpGet("GetUserSubsciptions/{userID}")]
        public async Task<ActionResult<List<string>>> GetUserSubsciptions(int userID)
        {
            List<string> res = await service.GetUserSubsciptions(userID);

            if (res!=null)
                return Ok(res);
            else
                return BadRequest();
        }


        [HttpGet("GetUserRecommendations/{userID}")]
        public async Task<ActionResult<List<RecommendationDTO>>> GetUserRecommendations(int userID)
        {
            List<RecommendationDTO> res = await service.GetUserRecommendations(userID);

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetAllFilteredFriends/{userID}/{filter}")]
        public async Task<ActionResult<List<UserDTO>>> GetAllFilteredFriends(int userID, string filter)
        {
            List<UserDTO> res = await service.GetAllFilteredFriends(userID, filter);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }
    }
}
