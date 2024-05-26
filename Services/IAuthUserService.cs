using RentalService.Models;

namespace RentalService.Services
{
    public interface IAuthUserService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);

        string CreateToken(UserViewModel user);

        RefreshToken GenerateRefreshToken();
    }
}
