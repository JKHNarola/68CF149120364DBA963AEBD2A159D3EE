using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using App.BL;
using App.BL.Interfaces;
using App.BL.Data.ViewModel;

namespace App.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IUserManagementRepository _userService;

        public AccountController(
            ILogger<AccountController> logger,
            IUserManagementRepository userService
            ) : base()
        {
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginViewModel userModel)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);
            try
            {
                var result = await _userService.LoginAsync(userModel);
                if (result == null)
                    return OKResult(0, "invalid username or password");
                else
                    return OKResult(1, "success", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return OtherResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("check/loggedin")]
        public IActionResult CheckAlreadyLoggedIn()
        {
            if (User.Identity.IsAuthenticated)
                return OKResult(1, "already loggedin");

            return OKResult(0, "not loggedin");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);

            var result = await _userService.RegisterAsync(model);
            return OKResult(result.Key, result.Value);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string email, string code)
        {
            if (string.IsNullOrEmpty(email) || code == null)
                return OtherResult(HttpStatusCode.BadRequest, "Email is required.");

            var result = await _userService.ConfirmEmailAsync(email, code);
            if (result)
                return OKResult(1, "email confirmed, mail sent for set password");

            return OKResult(0, "link expired");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("setpassword")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);

            var result = await _userService.SetPasswordAsync(model);
            switch (result.Key)
            {
                case 0:
                    return OtherResult(HttpStatusCode.BadRequest, "User not found for provided email.");
                case 1:
                    return OKResult(1, "password successfully changed. login successfull", result.Value);
                case 2:
                    return OKResult(2, "link exipred");
            }

            //Will never come to this
            throw new AppException("Something went wrong.");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromQuery]string email)
        {
            if (string.IsNullOrEmpty(email))
                return OtherResult(HttpStatusCode.BadRequest, "Email is required.");

            var result = await _userService.ForgotPasswordAsync(email);

            if (result)
                return OKResult(1, "email sent for resetting password");

            return OtherResult(HttpStatusCode.BadRequest, "User not found for provided email.");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);

            var result = await _userService.ResetPasswordAsync(model);
            switch (result.Key)
            {
                case 0:
                    return OtherResult(HttpStatusCode.BadRequest, "User not found for provided email.");
                case 1:
                    return OKResult(1, "password successfully reset. login successfull", result.Value);
                case 2:
                    return OKResult(2, "link expired");
            }

            //Will never come to this
            throw new AppException("Something went wrong.");
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);

            var userid = User.GetUserId();
            if (string.IsNullOrEmpty(userid))
                return OtherResult(HttpStatusCode.BadRequest, "Authorized user not found.");

            var user = await _userService.GetSingleAsync(x => x.Id == userid);
            if (user == null)
                return OtherResult(HttpStatusCode.BadRequest, "Authorized user not found.");

            var result = await _userService.ChangePasswordAsync(model, user);
            if (result.Key == 1)
                return OKResult(result.Key, "password successfully changed. login successfull", result.Value);

            return OKResult(result.Key, "change password falied", result.Value);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return OKResult(1, "logout success");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("check/usernameexist")]
        public async Task<IActionResult> IsUserNameExistAsync(string userName)
        {
            var userDetail = await _userService.GetSingleAsync(true, x => x.UserName == userName);
            if (userDetail != null)
                return OKResult(1, "username already exist");

            return OKResult(0, "username not found");
        }
    }
}
