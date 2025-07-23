namespace KioskCheckIn.API.Helpers
{
#pragma warning disable
    public class AuthResult
    {
        public bool IsAuthenticated { get; set; } = false;
        public string Token { get; set; }
        public string AuthCookie { get; set; }
        public string ErrorMessage { get; set; }
    }
}
