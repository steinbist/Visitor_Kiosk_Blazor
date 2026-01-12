using KioskCheckIn.API.DTO;
using KioskCheckIn.Data.Models;

namespace KioskCheckIn.API.Repository
{
    public interface IUserSessionRepository
    {
        Task<UserSession> InsertSession(UserDTO userDto);
        Task<UserSession> GetSession(int sessionId);
        Task<bool> EndSession(int sessionId);
    }
}
