using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalService.Dtos;
using RentalService.Models;
using RentalService.Services;

namespace RentalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthUserService _userService;

        //Temporary user model to test register and login feature
        public static UserViewModel user = new UserViewModel();

        public AuthController(IConfiguration configuration, IAuthUserService userService) 
        {
            _configuration = configuration;
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] UserRegisterDtos request)
        {
            _userService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.UserEmail = request.UserEmail;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return Ok("User Registered Successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDto request)
        {
            if (user.UserEmail != request.UserEmail)
            {
                return BadRequest("User not found.");
            }

            if (!_userService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = _userService.CreateToken(user);

            var refreshToken = _userService.GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            return Ok("User Loggedin Successfully");
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
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
            SetRefreshToken(newRefreshToken);

            return Ok("JWT Token Refreshed");
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }
    }
}
