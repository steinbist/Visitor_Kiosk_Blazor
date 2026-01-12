using KioskCheckIn.API.DTO;
using KioskCheckIn.Data.Models;

namespace KioskCheckIn.API.Repository
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private KioskContext _context;

        public UserSessionRepository(KioskContext context)
        {
            _context = context;
        }

        public async Task<UserSession> InsertSession(UserDTO userDTO)
        {
            var userSession = new UserSession();
            userSession.SessionStart = DateTime.Now;
            userSession.ClientId = Guid.Parse(userDTO.ClientId);
            userSession.Username = userDTO.Username;
            userSession.UserId = userDTO.UserId;

            await _context.AddAsync(userSession);
            _context.SaveChanges();

            return userSession;
        }

        public async Task<bool> EndSession(int id)
        {
            return true;
        }

        public async Task<UserSession> GetSession(int id)
        {
            return new UserSession();
        }
    }
}
