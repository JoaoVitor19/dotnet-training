using dotnet_training.Data;
using dotnet_training.Models;
using dotnet_training.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_training.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;
        public LoginController(AppDbContext context)
        {
            _userService = new UserService(context);
        }
        public record LoginRequest(string Email, string Password, string Username = null);
        public record LoginResponse(string Username, string Email, string Role, string Token);

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Authenticate([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Password))
                return BadRequest(new { message = "Password are required for login" });

            User user = null;

            if (string.IsNullOrEmpty(loginRequest.Username))
            {
                if (string.IsNullOrEmpty(loginRequest.Email))
                {
                    return BadRequest(new { message = "Email or Username are required for login" });
                }
                else
                {
                    user = await _userService.FindByEmailAsync(loginRequest.Email);
                }
            }
            else
            {
                user = await _userService.FindByUsernameAsync(loginRequest.Username);
            }

            if (user is null) return Unauthorized(new { message = "User not found" });

            if (user.Password != loginRequest.Password) return Unauthorized(new { message = "Invalid Credentials" });


            var tokenService = new TokenService();
            var token = tokenService.GenerateToken(user);

            return Ok(new LoginResponse(user.Username, user.Email, user.Role ?? string.Empty, token));
        }
    }
}
