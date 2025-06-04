using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Models.DTO;
using BookStoreCatalog_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookStoreCatalog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private APIResponse _response;

        public UserController(IUserRepo userRepo) 
        {
            _userRepo = userRepo;
            _response = new APIResponse();
        }

        [HttpPost("login")]  //api/user/login
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO userDTO)
        {
            var loginResponse = await _userRepo.Login(userDTO);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or Password incorrects");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]  //api/user/login
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO userDTO)
        {
            return null;
        }
    }
}
