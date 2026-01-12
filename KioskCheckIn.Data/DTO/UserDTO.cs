namespace KioskCheckIn.API.DTO
{
#pragma warning disable
    public class UserDTO
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public bool Administrator { get; set; } = false;
        public string ClientId {  get; set; }
    }
}
