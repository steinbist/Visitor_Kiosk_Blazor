using System.ComponentModel.DataAnnotations;

namespace KioskCheckIn.Web.Shared.Models
{
    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string PIN { get; set; } = string.Empty;

        [Required]
        public string ClientId { get; set; }
    }
}
