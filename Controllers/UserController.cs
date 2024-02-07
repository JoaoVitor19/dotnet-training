using dotnet_training.Models;
using dotnet_training.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_training.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        public record UserResponse(User User, string Token);

        [HttpPost]
        [Route("LoginWithUsername")]
        [AllowAnonymous]
        public ActionResult<UserResponse> AuthenticateWithUsername([FromBody] User model)
        {

            if (model.Username is null || model.Password is null)
                return NotFound(new { message = "username and password is needed for login" });

            var userService = new UserService();
            var tokenService = new TokenService();

            User user = userService.GetWithUsername(model.Username, model.Password);

            if (user is null)
                return NotFound(new { message = "username or password is invalid" });

            var token = tokenService.GenerateToken(user);
            user.Password = "";

            return new UserResponse(user, token);
        }

        [HttpPost]
        [Route("LoginWithEmail")]
        [AllowAnonymous]
        public ActionResult<UserResponse> AuthenticateWithEmail([FromBody] User model)
        {
            if (model.Email is null || model.Password is null)
                return NotFound(new { message = "email and password is needed for login" });

            var userService = new UserService();
            var tokenService = new TokenService();

            User user = userService.GetWithEmail(model.Email, model.Password);

            if (user is null)
                return NotFound(new { message = "email or password is invalid" });

            var token = tokenService.GenerateToken(user);
            user.Password = "";

            return new UserResponse(user, token);
        }

        [HttpGet]
        [Route("GetLoggedName")]
        [Authorize]
        public ActionResult<string> GetLoggedName() => User.Identity.Name;

        [HttpGet]
        [Route("IsSupport")]
        [Authorize(Roles = "support")]
        public ActionResult<string> IsSupport() => string.Format("This member is support : {0}", User.Identity.Name);

        [HttpGet]
        [Route("IsDeveloper")]
        [Authorize(Roles = "developer")]
        public ActionResult<string> IsDeveloper() => string.Format("This member is developer : {0}", User.Identity.Name);
    }
}
