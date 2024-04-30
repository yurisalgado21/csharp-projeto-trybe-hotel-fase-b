using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("city")]
    public class CityController : Controller
    {
        private readonly ICityRepository _repository;
        public CityController(ICityRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        public IActionResult GetCities(){
            return Ok(_repository.GetCities());
        }

        [HttpPost]
        public IActionResult PostCity([FromBody] City city){
            try
            {
                var newCity = _repository.AddCity(city);
                return Created("", newCity);
            }
            catch (Exception err)
            {
                
                throw new Exception(err.Message);
            }
        }
    }
}