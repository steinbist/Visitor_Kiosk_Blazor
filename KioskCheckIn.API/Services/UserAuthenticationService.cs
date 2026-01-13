using AutoMapper;
using KioskCheckIn.API.DTO;
using KioskCheckIn.API.Helpers;
using KioskCheckIn.API.Repository;
using KioskCheckIn.API.Services.Interfaces;
using KioskCheckIn.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace KioskCheckIn.API.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IUserSessionRepository _usr;
        private readonly ICookie _cookie;
        private readonly IHttpContextAccessor _contextAccessor;
        private const string AuthFailureMessage = "Authentication failed. Please try again later."; 

        public UserAuthenticationService(IUserRepository repository, IUserSessionRepository usr, IMapper mapper, IHttpContextAccessor contextAccessor, ICookie cookie, ILogger<UserAuthenticationService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _usr = usr;
            _cookie = cookie;
            _contextAccessor = contextAccessor;
        }

        public async Task<AuthResult> AuthenticateUserAsync(UserDTO userDTO)
        {
            var result = new AuthResult();
            var httpContext = _contextAccessor.HttpContext;
            try
            {
                // Get the user with matching username and first name and last name
                var user = await _repository.GetUser(userDTO.Username);
                if (user == null || string.IsNullOrEmpty(user.PasswordHash) == true || string.IsNullOrEmpty(user.Salt) == true)
                {
                    result.IsAuthenticated = false;
                    result.ErrorMessage = AuthFailureMessage;

                    // Log message for this result
                    _logger.LogError("User not found.");
                    return  result;
                }

                // Hash the password that was passed in the request, include the salt with the hash.
                string hashedPassword = HashPassword(userDTO.Password, user.Salt);

                // Compare the two for authentication then create token if hash is validated. Else return the message.
                if (hashedPassword == user.PasswordHash)
                {
                    // The UserDTO gets passed to CreateSession because there is other data passed
                    // to the DTO in the request needed to create the session. 
                    userDTO.UserId = user.UserId;
                    var session = await CreateSession(userDTO);
                    if (session != null && httpContext != null)
                    {
                        // Use cookies for Blazor Server because of the statefulness of SignalR connection
                        // between browser and server.
                        await _cookie.CreateCookie(user, session, httpContext);
                        result.IsAuthenticated = true;
                    }
                }
                else
                {
                    result.ErrorMessage = AuthFailureMessage;
                }
                 
                return result;
            }
            catch (Exception ex)
            {
                // Log exception ex.Message which will be potentially the auth cookie was not created, or db not accessible for user data.
                // Stack trace will notbe long and therefore not overly verbose.
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                result.ErrorMessage = AuthFailureMessage;
                return result;
            }
        }

        private async Task<UserSession> CreateSession(UserDTO userDto)
        {
            return await _usr.InsertSession(userDto);
        }


        public Task<bool> CreateUserAccount(UserDTO userDTO)
        {
            try
            {
                var salt = PasswordHelper.GenerateSalt();
                var hashedPassword = PasswordHelper.HashPassword(userDTO.Password, salt);

                var user = _mapper.Map<User>(userDTO);
                user.PasswordHash = hashedPassword;
                user.Salt = salt;

                return Task.FromResult(_repository.InsertUser(user).Result);
            }
            catch (Exception ex)
            {
                return Task<bool>.FromException<bool>(ex);
            }
        }

        public string HashPassword(string password, string salt)
        {
            return PasswordHelper.HashPassword(password, salt);

        }

        /// <summary>
        /// private method that creates a token based on parameters
        /// CHANGED! Creates a cookie auth for Signal R connection since 
        /// this is Blazor Server - Blazor Web Assembly would utilize the JWT.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string CreateToken(User user, UserSession session)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Environment.GetEnvironmentVariable("JWT_SECRET");
            
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("JWT_SECRET environment variable not set.");
            }

            var claims = new List<Claim>()
            {
                new Claim("userId", $"{session.UserId}"),
                new Claim(ClaimTypes.Name, session.Username),
                new Claim("SessionStart", session.SessionStart.Value.ToString("yyyy-MM-dd HH:mm")),
                new Claim("ClientID", $"{session.ClientId}")
            };

            var signingKey = Encoding.UTF8.GetBytes(key);
            
            var claimsIdentity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(claimsIdentity);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                //Expires = DateTime.UtcNow.AddHours(12),
                Expires = session.SessionStart.Value.AddHours(9),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(signingKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
          
            // CreateToken(...) throws a null reference exception and logs the error.
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }




        private byte[] GetKeyBytes()
        {
            byte[] key = new byte[32]; // 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return key;
        }
    }
}
