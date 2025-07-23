using KioskCheckIn.API.DTO;
using KioskCheckIn.API.Helpers;
using KioskCheckIn.Data.Models;

namespace KioskCheckIn.API.Services.Interfaces
{
    public interface IUserAuthenticationService
    {
        Task<AuthResult> AuthenticateUserAsync(UserDTO userDTO);
        Task<bool> CreateUserAccount(UserDTO userDTO);
        string HashPassword(string password, string salt);
    }
}
