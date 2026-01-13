using KioskCheckIn.API.DTO;
using KioskCheckIn.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KioskCheckIn.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private UserAuthenticationService _userService;

        public LoginController(UserAuthenticationService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Login endpoint to Authenticate user
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody] UserDTO userDto)
        {
            if (userDto == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _userService.AuthenticateUserAsync(userDto);
            return result.IsAuthenticated
                ?  Ok(result)
                : Unauthorized();
        }
    }
}
