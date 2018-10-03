using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Net;
using App.BL.Data.DTO;
using App.BL.Data;
using App.Models;
using App.BL.Services;
using App.BL;
using App.BL.Interfaces;
using App.Misc;
using System.Collections.Generic;

namespace App.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/account")]
    public class AccountController : BaseController
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _db;
        private readonly EmailService _emailService;
        private readonly AppSettings _appSetting;
        private readonly IUserManagementRepository _userRepo;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db,
            IHttpContextAccessor httpContext,
            IOptions<EmailSettings> emailSettings,
            IOptions<AppSettings> appSettings,
            IUserManagementRepository userRepo
            ) : base(httpContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _httpContext = httpContext;
            _db = db;
            _emailService = new EmailService(emailSettings);
            _appSetting = appSettings.Value;
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginModel userModel)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);
            try
            {
                var result = await PerformLogin(userModel);
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
        private async Task<LoginSuccessModel> PerformLogin(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userRepo.GetSingleAsync(x => x.UserName == model.UserName);
                var roles = await _userManager.GetRolesAsync(user);
                var role = (Role)Enum.Parse(typeof(Role), roles[0]);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, roles[0])
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(_appSetting.TokenAliveTime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Startup.SymmetricSecurityKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                var userData = new LoginSuccessModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Token = tokenString,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleId = (int)role
                };

                return userData;
            }
            return null;
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
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return InvalidModelStateResult(ModelState);

                var isUserExixts = await _userRepo.IsUserExists(model.Email);
                if (isUserExixts)
                    return OKResult(-3, "user already exists for email", model.Email);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = false,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var resultCreateUser = await _userManager.CreateAsync(user, _appSetting.DefaultPassword);
                if (!resultCreateUser.Succeeded)
                {
                    _logger.LogInformation("A new user account " + model.Email + " created with default password " + _appSetting.DefaultPassword);
                    return OKResult(-2, resultCreateUser.Errors.FirstOrDefault()?.Description.ToLower());
                }

                var roleRes = await _userManager.AddToRoleAsync(user, Role.WebUser.ToString());
                if (!roleRes.Succeeded)
                {
                    _logger.LogError("Couldn't add user to role. " + roleRes.Errors.ToSerializedJsonString());
                    _logger.LogWarning("Deleting user : " + user.Email);
                    var resDeleteUser = await _userManager.DeleteAsync(user);
                    if (!resDeleteUser.Succeeded)
                    {
                        _logger.LogError("Couldn't delete user " + user.Email + ", Error(s) : " + roleRes.Errors.ToSerializedJsonString());
                        return OKResult(-7, "user successfully created but failed to set role, tried to remove user but error occured");
                    }

                    return OKResult(-8, "user successfully created but failed to set role, removed user");
                }
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var fullname = user.FirstName + " " + user.LastName;
                var mailContent = await EmailBodyCreator.CreateConfirmEmailBody(GetCurrHost(), fullname, user.Email, code);
                var fullName = user.FirstName + " " + user.LastName;
                try
                {
                    await _emailService.SendMailAsync(new List<(string email, string displayName)>() { (user.Email, fullName) }, null, null, AppCommon.AppName + " - Confirm Email", mailContent, null);
                    return OKResult(1, "user successfully created, email sent");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Couldn't sent mail. Error : " + ex.ToString());

                    _logger.LogWarning("Removing user " + user.Email + " from role " + Role.WebUser.ToString());
                    var resRemoveFromRole = await _userManager.RemoveFromRoleAsync(user, Role.WebUser.ToString());
                    if (!resRemoveFromRole.Succeeded)
                    {
                        _logger.LogError("Couldn't remove user " + user.Email + " from role " + Role.WebUser.ToString() + ", Error(s) : " + resRemoveFromRole.Errors.ToSerializedJsonString());
                        return OKResult(-6, "user successfully created but failed to sent email, tried to remove user but error occured");
                    }

                    _logger.LogWarning("Deleting user : " + user.Email);
                    var resDeleteUser = await _userManager.DeleteAsync(user);
                    if (!resDeleteUser.Succeeded)
                    {
                        _logger.LogError("Couldn't delete user " + user.Email + ", Error(s) : " + resRemoveFromRole.Errors.ToSerializedJsonString());
                        return OKResult(-6, "user successfully created but failed to sent email, tried to remove user but error occured");
                    }

                    return OKResult(-4, "user successfully created but failed to sent email, deleted user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return OtherResult(HttpStatusCode.InternalServerError, "some error occured", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string email, string code)
        {
            if (string.IsNullOrEmpty(email) || code == null)
                return OtherResult(HttpStatusCode.BadRequest, "Email is required.");

            var user = await _userManager.FindByEmailAsync(email);

            code = WebUtility.UrlDecode(code);
            code = code.Replace(' ', '+');

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                var fullname = user.FirstName + " " + user.LastName;
                var mailContent = await EmailBodyCreator.CreateSetPasswordEmailBody(GetCurrHost(), fullname, user.Email, resetCode);
                var fullName = user.FirstName + " " + user.LastName;
                await _emailService.SendMailAsync(new List<(string email, string displayName)>() { (user.Email, fullName) }, null, null, AppCommon.AppName + " - Set Username & Password", mailContent, null);

                return OKResult(1, "email confirmed, mail sent for set password");
            }
            return OKResult(0, "link expired");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("setpassword")]
        public async Task<IActionResult> SetPassword([FromBody]SetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return OtherResult(HttpStatusCode.BadRequest, "User not found for provided email.");

            user.UserName = model.UserName;
            model.Code = WebUtility.UrlDecode(model.Code);
            model.Code = model.Code.Replace(' ', '+');

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var loginRes = await PerformLogin(new LoginModel() { UserName = user.UserName, Password = model.Password });
                if (loginRes != null)
                    return OKResult(1, "password successfully changed. login successfull", loginRes);
            }
            return OKResult(2, "link expired");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromQuery]string email)
        {
            if (string.IsNullOrEmpty(email))
                return OtherResult(HttpStatusCode.BadRequest, "Email is required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return OtherResult(HttpStatusCode.BadRequest, "User not found for provided email.");

            var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            var fullname = user.FirstName + " " + user.LastName;
            var mailContent = await EmailBodyCreator.CreateResetPasswordEmailBody(GetCurrHost(), fullname, user.Email, resetCode);
            var fullName = user.FirstName + " " + user.LastName;
            await _emailService.SendMailAsync(new List<(string email, string displayName)>() { (user.Email, fullName) }, null, null, AppCommon.AppName + " - Reset Password", mailContent, null);

            return OKResult(1, "email sent for resetting password");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return OtherResult(HttpStatusCode.BadRequest, "User not found for provided email.");

            model.Code = WebUtility.UrlDecode(model.Code);
            model.Code = model.Code.Replace(' ', '+');

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var loginRes = await PerformLogin(new LoginModel() { UserName = user.UserName, Password = model.NewPassword });
                if (loginRes != null)
                    return OKResult(1, "password successfully reset. login successfull", loginRes);
            }
            return OKResult(2, "link expired");
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelStateResult(ModelState);

            var userid = User.GetUserId();
            if (string.IsNullOrEmpty(userid))
                return OtherResult(HttpStatusCode.BadRequest, "Authorized user not found.");

            var user = await _userRepo.GetSingleAsync(x => x.Id == userid);
            if (user == null)
                return OtherResult(HttpStatusCode.BadRequest, "Authorized user not found.");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var loginRes = await PerformLogin(new LoginModel() { UserName = user.UserName, Password = model.NewPassword });
                if (loginRes != null)
                    return OKResult(1, "password successfully changed. login successfull", loginRes);
            }
            return OKResult(2, "change password falied", result.Errors);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return OKResult(1, "logout success");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("check/usernameexist")]
        public async Task<IActionResult> IsUserNameExistAsync(string userName)
        {
            var userDetail = await _userRepo.GetSingleUserAsync(true, x => x.UserName == userName);
            if (userDetail != null)
                return OKResult(1, "username already exist");

            return OKResult(0, "username not found");
        }
    }
}
