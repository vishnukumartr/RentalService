using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentalService.Data;
using RentalService.Dtos;
using RentalService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RentalService.Services.AuthUserService
{
    public class AuthUserService : IAuthUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly RentalDbContext _context;
        private readonly ILogger<AuthUserService> _logger;

        public AuthUserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, 
                                RentalDbContext dbContext, ILogger<AuthUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _context = dbContext;
            _logger = logger;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Secrets:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;

        }


        public async Task<User> RegisterUser(UserRegisterDto registerDto)
        {
            try
            {
                if (_context.Users.Any(u => u.UserEmail == registerDto.UserEmail))
                {
                    throw new InvalidOperationException("Email already exists.");
                }

                CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var user = new User
                {
                    UserEmail = registerDto.UserEmail,
                    Username = registerDto.UserEmail.Substring(0, registerDto.UserEmail.IndexOf("@")),
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    FullName = string.Concat(registerDto.FirstName, " ", registerDto.LastName),
                    PhoneNumber = registerDto.PhoneNumber,
                    UserRole = registerDto.UserRole,
                };

                if (registerDto.UserRole != null && registerDto.UserRole == "Seller")
                {
                    user.IsSeller = true;
                }

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return user;
            }

            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while Registering the User.");
                throw new Exception(ex.Message);
            }
            
        }

    }
}
