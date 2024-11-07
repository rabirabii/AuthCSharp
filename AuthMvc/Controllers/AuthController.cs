using AuthMvc.DataContext;
using AuthMvc.Models;

using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataContextAuth _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        public AuthController(DataContextAuth context, IConfiguration configuration, TokenService tokenService)
        {
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        // Partial View Register
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            if (_context.Users.Any(u => u.Email == request.Email))
            {
                ModelState.AddModelError("CustomError", "User Already Exists");
                return View(request);
            }

            var newUser = new UserModel
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHas = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // Partial View Login
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }    
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(request);
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHas))
            {
                return View(request);
            }
            var token = _tokenService.GenerateToken(user);
            Response.Cookies.Append("AuthToken", token, new CookieOptions { HttpOnly = true, Secure = true });

            return RedirectToAction("Index", "Home");
        }
    }
}
