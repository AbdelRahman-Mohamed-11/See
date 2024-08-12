using Application.Interfaces;
using Core.DTOS;
using Core.Entities.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlassesApp.Controllers
{
    public class CitiesController : BaseApiController
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            this._cityService = cityService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Guid>> CreateCity([FromBody]
        string city)
        {
            return Ok(await _cityService.CreateCity(city));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<bool>> DeleteCity([FromBody] Guid cityId)
        {
            return Ok(await _cityService.DeleteCity(cityId));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<bool>> UpdateCity(
           [FromBody] UpdateCityDto city)
        {
            return Ok(await _cityService.UpdateCity(city.CityID, city.CityName));
        }

        [HttpGet]
        public async Task<ActionResult<List<City>>> GetCities()
        {
            return await _cityService.GetCities();
        }


    }
}
