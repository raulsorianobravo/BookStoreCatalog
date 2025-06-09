using BookStoreCatalog_web.Models;
using BookStoreCatalog_web.Models.DTO;
using BookStoreCatalog_web.Services.IServices;
using BookStoreCatalogUtils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> Login(LoginRequestDTO userDTO) 
        {
            var response = await _userService.Login<APIResponse>(userDTO);
            if (response != null && response.IsSuccess == true)
            {
                LoginResponseDTO loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));
                HttpContext.Session.SetString(ClassDefinitions.SessionToken, loginResponse.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Error Messages", response.ErrorMessages.FirstOrDefault());
                return View(userDTO);
            }
        }
        
        public async Task<IActionResult> Logout() 
        { 
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(ClassDefinitions.SessionToken , "");
            return View("Index", "Home"); 
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
