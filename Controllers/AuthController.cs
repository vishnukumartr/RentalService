using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalService.Data;
using RentalService.Dtos;
using RentalService.Models;
using RentalService.Services.AuthUserService;
using System.Linq;

namespace RentalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RentalDbContext _context;
        private readonly IAuthUserService _userService;
        public const string Seller = "Seller";
        public const string Buyer = "Buyer";

        public AuthController(RentalDbContext context, IAuthUserService userService) 
        {
            _context = context;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] UserRegisterDtos request)
        {
            if (_context.Users.Any(u => u.UserEmail == request.UserEmail))
            {
                return Conflict("Email already exists.");
            }

            _userService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User();
            user.UserEmail = request.UserEmail;
            user.Username = request.UserEmail.Substring(0, request.UserEmail.IndexOf("@"));
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName; 
            user.FullName = string.Concat(user.FirstName, " ", request.LastName);
            user.PhoneNumber = request.PhoneNumber;
            user.UserRole = request.UserRole;

            if(string.IsNullOrEmpty(request.UserRole) || (request.UserRole != "Seller" && request.UserRole != "Buyer"))
            {
                return BadRequest("User Role is Empty | User Role allows only - Seller or Buyer");
            }
            
            if(request.UserRole != null && request.UserRole == "Seller") 
            {
                user.IsSeller = true;
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok("User Registered Successfully");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserEmail.ToLower().Equals(request.UserEmail.ToLower()));

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (!_userService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = _userService.CreateToken(user);

            var refreshToken = _userService.GenerateRefreshToken();

            SetRefreshToken(refreshToken, ref user);

            await _context.SaveChangesAsync();

            return Ok("User Loggedin Successfully, Token :" + token);
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken(User user)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = _userService.CreateToken(user);
            var newRefreshToken = _userService.GenerateRefreshToken();

            SetRefreshToken(newRefreshToken, ref user);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok("JWT Token Refreshed");
        }

        private void SetRefreshToken(RefreshToken refreshToken, ref User user) 
        {

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpires = refreshToken.Expires;
        }


    }
}
