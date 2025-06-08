using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreCatalog_web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login() 
        { 
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginRequestDTO userDTO) 
        { 
            return View(); 
        }
        
        public IActionResult Logout() 
        { 
            return View(); 
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDTO userDTO)
        {
            var response = await _userService.Register <APIResponse>(userDTO);

            if (response != null && response.IsSuccess) 
            {
                return RedirectToAction("login");
            }
            return View();
        }


    }
}
