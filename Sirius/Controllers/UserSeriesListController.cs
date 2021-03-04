using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirius.DTOs;
using Sirius.Services;

namespace Sirius.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserSeriesListController : ControllerBase
    {
        private UserSeriesListService service;

        public UserSeriesListController(UserSeriesListService _service)
        {
            service = _service;
        }

        [HttpGet("GetUserSeriesList/{userID}")]
        public async Task<ActionResult> GetUserSeriesList(int userID)
        {
            var res = await service.GetUserSeriesList(userID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetUserFavourites/{userID}")]
        public async Task<ActionResult> GetUserFavourites(int userID)
        {
            var res = await service.GetUserFavourites(userID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetRecommendations/{userID}")]
        public async Task<ActionResult<List<Object>>> GetRecommendations(int userID)
        {
            List<Object> res = await service.GetRecommendations(userID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetSeriesPopularity/{seriesID}")]
        public async Task<ActionResult> GetSeriesPopularity(int seriesID)
        {
            var res = await service.GetSeriesPopularity(seriesID);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetMostPopularSeries")]
        public async Task<ActionResult> GetMostPopularSeries()
        {
            var res = await service.GetMostPopularSeries();

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("GetListed/{id}")]
        public async Task<ActionResult> GetListed(int id)
        {
            var res = await service.GetListed(id);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        public async Task<int> GetListedSeries(int userID, int seriesID)
        {
            int res = await service.GetListedSeries(userID, seriesID);
            return res;
        }

        [HttpPost("AddSeriesToList/{userID}/{seriesID}/{status}/{fav}")]
        public async Task<ActionResult> AddSeriesToList(string status, bool fav, int userID, int seriesID)
        {
            bool res = await service.AddSeriesToList(status, fav, userID, seriesID);
            if (res)
                return Ok();
            else
                return BadRequest();
    
        }

        [HttpPut("{id}/{seriesID}")]
        public async Task<ActionResult> Put([FromBody] FavouriteSeriesEditDTO data, int id, int seriesID)
        {
            bool res = await service.Put(data, id, seriesID);
            if (res)
                return Ok();
            else
                return BadRequest();
        }

        public async Task<ActionResult> UpdateRating(int id, float rating)
        {
            bool res = await service.UpdateRating(id, rating);
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

        [HttpPost("GetUserSeriesFiltered/{userID}")]
        public async Task<ActionResult> GetUserSeriesFiltered([FromBody] UserSeriesListFilterDTO filter, int userID)
        {
            var res = await service.GetUserSeriesFiltered(userID, filter);
            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }
    }
}

