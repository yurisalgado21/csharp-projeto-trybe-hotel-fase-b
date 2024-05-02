using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("room")]
    public class RoomController : Controller
    {
        private readonly IRoomRepository _repository;
        public RoomController(IRoomRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{HotelId}")]
        public IActionResult GetRoom(int HotelId){
            try
            {
                return Ok(_repository.GetRooms(HotelId));
            }
            catch (Exception err)
            {
                
                throw new Exception(err.Message);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Admin")]
        public IActionResult PostRoom([FromBody] Room room){
            try
            {
                var newRoom = _repository.AddRoom(room);
                return Created("", newRoom);
            }
            catch (Exception err)
            {
                
                throw new Exception(err.Message);
            }
        }

        [HttpDelete("{RoomId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize("Admin")]
        public IActionResult Delete(int RoomId)
        {
            try
            {
                _repository.DeleteRoom(RoomId);
                return NoContent();
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}