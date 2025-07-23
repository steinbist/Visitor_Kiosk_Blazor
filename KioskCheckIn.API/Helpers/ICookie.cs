using KioskCheckIn.Data.Models;
using System.Security.Claims;

namespace KioskCheckIn.API.Helpers
{
    public interface ICookie
    {
        ClaimsIdentity Identity { get; set; }
        ClaimsPrincipal Principal { get; set; }
        Task CreateCookie(User user, UserSession session, HttpContext httpContext);
        Task SignIn(ClaimsIdentity claimsIdentity, HttpContext httpContext);
    }
}
