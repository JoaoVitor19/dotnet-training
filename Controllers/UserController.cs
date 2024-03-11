using dotnet_training.Data;
using dotnet_training.Models;
using dotnet_training.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_training.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        public UserController(AppDbContext context)
        {
            _userService = new UserService(context);
        }
        public record UserResponse(User User, string Token);

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

            var validator = new UserValidator();
            var ValidationResults = validator.Validate(user);
            if (ValidationResults.IsValid is false)
            {
                return BadRequest(string.Join(", ", ValidationResults.Errors.Select(s => s.ErrorMessage)) + ".");
            }


            var existingUser = await _userService.FindByEmailAsync(user.Email);
            if (existingUser is not null)
                return Conflict(new { message = "This email is not available" });

            existingUser = await _userService.FindByUsernameAsync(user.Username);
            if (existingUser is not null)
                return Conflict(new { message = "This username is not available" });

            var createdUser = await _userService.CreateAsync(user);

            return Created(string.Empty, createdUser);
        }

        [HttpPut("UpdateEmail/{Id}")]
        public async Task<ActionResult<User>> UpdateEmail(long Id, [FromBody] string newEmail)
        {
            //Validate if new email is valid;
            if (!Helpers.Validators.IsValidEmail(newEmail))
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
            //Validate if new username is valid;
            if (!Helpers.Validators.IsValidUsername(newUsername))
                return BadRequest(new { message = "New username is not valid" });

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
            //Validate if new password is valid;
            if (!Helpers.Validators.IsValidPassword(newPassword))
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
    }
}
