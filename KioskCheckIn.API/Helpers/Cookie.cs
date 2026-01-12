using KioskCheckIn.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace KioskCheckIn.API.Helpers
{
    public class Cookie : ICookie
    {
        public ClaimsIdentity Identity { get; set; }
        public ClaimsPrincipal Principal { get; set; }

        public async Task CreateCookie(User user, UserSession session, HttpContext httpContext)
        {
            var claims = new List<Claim>()
            {
                new Claim("userId", $"{session.UserId}"),
                new Claim(ClaimTypes.Name, session.Username),
                new Claim("SessionStart", session.SessionStart.Value.ToString("yyyy-MM-dd HH:mm")),
                new Claim("ClientID", $"{session.ClientId}")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await SignIn(claimsIdentity, httpContext);
        }

        public async Task SignIn(ClaimsIdentity identity, HttpContext httpContext)
        {
           var principal = new ClaimsPrincipal(identity);
           await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
