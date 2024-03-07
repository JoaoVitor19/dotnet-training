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

        [HttpGet("FindUserById/{Id}")]
        public async Task<ActionResult<User>> FindUserById(long Id)
        {
            var user = await _userService.FindByIdAsync(Id);

            if (user is null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpGet("FindAllUsers")]
        public async Task<ActionResult<List<User>>> FindAllUsers()
        {
            return Ok(await _userService.FindAllUsersAsync());
        }

        [HttpPost("Create")]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            if (user is null)
                return BadRequest("Data of user is needed");

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Username))
                return BadRequest("Email and username is needed");

            if (Helpers.Utils.IsValidPassword(user.Password))
                return BadRequest("Password not acceptable");

            var existingUser = await _userService.FindByEmailAsync(user.Email);
            if (existingUser is not null)
                return Conflict(new { message = "This email is not available" });

            existingUser = await _userService.FindByUsernameAsync(user.Username);
            if (existingUser is not null)
                return Conflict(new { message = "This username is not available" });

            var createdUser = await _userService.Create(user);

            return Created(string.Empty, createdUser);
        }

        [HttpPut("UpdateEmail/{Id}")]
        public async Task<ActionResult<User>> UpdateEmail(long Id, [FromBody] string newEmail)
        {
            //Validate if new email is valid;
            if (!Helpers.Utils.IsValidEmail(newEmail))
                return BadRequest(new { message = "New email is not valid" });

            var existingUser = await _userService.FindByIdAsync(Id);
            if (existingUser is null)
                return NotFound(new { message = "User not found" });

            //Verify if email is available;
            var existingUserWithEmail = await _userService.FindByEmailAsync(newEmail);
            if (existingUserWithEmail is not null)
            {
                if (existingUserWithEmail.Id == Id)
                    return Conflict(new { message = "New email is equal to old" });
                else
                    return Conflict(new { message = "New email is not available" });
            }

            await _userService.UpdateEmailAsync(existingUser, newEmail);

            return Ok();
        }


        [HttpPut("UpdateUsername/{Id}")]
        public async Task<ActionResult<User>> UpdateUsername(long Id, [FromBody] string newUsername)
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

            await _userService.UpdateUsernameAsync(existingUser, newUsername);

            return Ok();
        }

        [HttpPut("UpdatePassword/{Id}")]
        public async Task<ActionResult<User>> UpdatePassword(long Id, [FromBody] string newPassword)
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

            await _userService.UpdatePasswordAsync(existingUser, newPassword);

            return Ok();
        }

        [HttpPatch("UpdateRole/{Id}/{Role}")]
        public async Task<ActionResult<User>> UpdateRole(long Id, string Role)
        {
            if (string.IsNullOrEmpty(Role))
                return Conflict(new { message = "New role is not valid" });

            var existingUser = await _userService.FindByIdAsync(Id);
            if (existingUser is null)
                return NotFound(new { message = "User not found" });

            if (!string.IsNullOrEmpty(existingUser.Role) && existingUser.Role == Role)
                return Conflict(new { message = "New role is equals to old" });

            await _userService.UpdateRoleAsync(existingUser, Role);

            return NoContent();
        }

        [HttpDelete("Delete/{Id}")]
        public async Task<ActionResult<User>> Delete(long Id)
        {
            var existingUser = await _userService.FindByIdAsync(Id);

            if (existingUser is null)
                return NotFound(new { message = "User not found" });

            await _userService.DeteleAsync(existingUser);

            return NoContent();
        }

        [HttpGet("FindAllRootUsers")]
        [Authorize(Roles = "root")]
        public async Task<ActionResult<List<User>>> RoleRootUser() => await _userService.FindAllRootUsers();
    }
}
