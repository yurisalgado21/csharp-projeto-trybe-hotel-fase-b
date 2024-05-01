using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : Controller
    {

        private readonly IUserRepository _repository;
        public LoginController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login){
            var createUser = _repository.Login(login);
            if (createUser != null)
            {
                string token = new TokenGenerator().Generate(createUser);
                return Ok(new {token});
            }

            return Unauthorized(new {message = "Incorrect e-mail or password"});
        }
    }
}