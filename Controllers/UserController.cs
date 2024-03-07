using dotnet_training.Data;
using dotnet_training.Models;
using dotnet_training.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_training.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        public UserController(AppDbContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }
        public record UserResponse(User User, string Token);
        public record LoginRequest(string Email, string Password, string Username = null);
        public record LoginResponse(string Username, string Email, string Role, string Token);

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Authenticate([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Password))
                return BadRequest(new { message = "Password are required for login" });

            if (string.IsNullOrEmpty(loginRequest.Email) && string.IsNullOrEmpty(loginRequest.Username))
                return BadRequest(new { message = "Email or Username are required for login" });

            User user = await _userService.AuthenticateAsync(loginRequest.Email, loginRequest.Password, loginRequest.Username);

            if (user is null)
                return Unauthorized(new { message = "Invalid credentials!" });

            var tokenService = new TokenService();
            var token = tokenService.GenerateToken(user);

            return new LoginResponse(user.Username, user.Email, user.Role ?? string.Empty, token);
        }

        [HttpGet("FindUserById/{Id}")]
        public async Task<ActionResult<User>> FindUserById(long Id)
        {
            var user = await _userService.FindByIdAsync(Id);

            if (user is null)
                return NotFound("User not found");

            return user;
        }

        [HttpGet("FindAllUsers")]
        public async Task<ActionResult<List<User>>> FindAllUsers()
        {
            return await _userService.FindAllUsersAsync();
        }

        [HttpPost("Create")]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            var existingUser = await _userService.FindByEmailAsync(user.Email);
            if (existingUser is not null)
                return Conflict(new { message = "This email is not permitted" });

            existingUser = await _userService.FindByUsernameAsync(user.Username);
            if (existingUser is not null)
                return Conflict(new { message = "This username is not permitted" });

            var createdUser = await _userService.Create(user);

            return Created(string.Empty, createdUser);
        }

        [HttpPut("UpdateEmail/{Id}")]
        public async Task<ActionResult<User>> UpdateEmail(string Email, long Id)
        {
            //Validate if new email is valid;
            if (!Helpers.Utils.IsValidEmail(Email))
                return BadRequest(new { message = "New email is not valid" });

            var existingUser = await _userService.FindByIdAsync(Id);
            if (existingUser is null)
                return NotFound(new { message = "User not found" });

            //Verify if email is available;
            var existingUserWithEmail = await _userService.FindByEmailAsync(Email);
            if (existingUserWithEmail is not null)
            {
                if (existingUserWithEmail.Id == Id)
                    return Conflict(new { message = "New email is equal to old" });
                else
                    return Conflict(new { message = "New email is not available" });
            }

            var updatedUser = await _userService.UpdateEmailAsync(existingUser, Email);
            return Ok(updatedUser);
        }


        [HttpPut("UpdateUsername/{Id}")]
        public async Task<ActionResult<User>> UpdateUsername(string newUsername, long Id)
        {
            var existingUser = await _userService.FindByIdAsync(Id);
            if (existingUser is null)
                return NotFound(new { message = "User not found" });

            //Verify if username is available;
            var existingUserWithUsername = await _userService.FindByUsernameAsync(newUsername);
            if (existingUserWithUsername is not null)
            {
                if (existingUserWithUsername.Id == Id)
                    return Conflict(new { message = "New username is equal to old" });
                else
                    return Conflict(new { message = "New username is not available" });
            }

            var updatedUser = await _userService.UpdateUsernameAsync(existingUser, newUsername);
            return Ok(updatedUser);
        }

        [HttpPut("UpdatePassword/{Id}")]
        public async Task<ActionResult<User>> UpdatePassword(string newPassword, long Id)
        {
            //Validate if new email is valid;
            if (!Helpers.Utils.IsValidPassword(newPassword))
                return BadRequest(new { message = "New password is not valid" });

            var existingUser = await _userService.FindByIdAsync(Id);
            if (existingUser is null)
                return NotFound(new { message = "User not found" });


            //**Hash options in Utils for hasing password in one-way method**\\
            //
            //if (Helpers.Utils.VerifyPassword(newPassword, existingUser.Password))
            //    return Conflict(new { message = "New password is equals to old" });
            //

            if (existingUser.Password.Equals(newPassword))
                return Conflict(new { message = "New password is equals to old" });

            var updatedUser = await _userService.UpdatePasswordAsync(existingUser, newPassword);
            return Ok(updatedUser);
        }

        [HttpDelete("Delete/{Id}")]
        public async Task<ActionResult<User>> Delete(long Id)
        {
            var existingUser = await _userService.FindByIdAsync(Id);

            if (existingUser is null)
                return NotFound(new { message = "User not found" });

            var removedUser = await _userService.DeteleAsync(existingUser);

            return Ok(removedUser);
        }
    }
}
