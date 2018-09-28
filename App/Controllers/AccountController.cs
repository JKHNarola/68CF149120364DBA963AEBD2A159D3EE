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
using System.IO;
using App.BAL.Data.DTO;
using App.BAL.Data;
using App.Models;
using App.BL.Services;
using App.BL;
using App.BL.Interfaces;

namespace App.Controllers
{
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
            )
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
        [HttpPost("[action]")]
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
                        new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
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

        [Authorize]
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
                var mailContent = await PrepareConfirmEmailTemplate(fullname, user.Email, code);
                var fullName = user.FirstName + " " + user.LastName;
                try
                {
                    await _emailService.SendMailAsync(fullName, user.Email, "", AppCommon.AppName + " - Confirm Email", mailContent, "");
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
                var mailContent = await PrepareSetPasswordEmailTemplate(fullname, user.Email, resetCode);
                var fullName = user.FirstName + " " + user.LastName;
                await _emailService.SendMailAsync(fullName, user.Email, "", AppCommon.AppName + " - Set Username & Password", mailContent, "");

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
            if(user == null)
                return OtherResult(HttpStatusCode.BadRequest, "User not found for provided email.");

            var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            var fullname = user.FirstName + " " + user.LastName;
            var mailContent = await PrepareResetPasswordEmailTemplate(fullname, user.Email, resetCode);
            var fullName = user.FirstName + " " + user.LastName;
            await _emailService.SendMailAsync(fullName, user.Email, "", AppCommon.AppName + " - Reset Password", mailContent, "");

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
                    return OKResult(1, "password successfully changed. login successfull", loginRes);
            }
            return OKResult(2, "link expired");
        }


        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return OKResult(1, "logout success");
        }

        [HttpGet]
        [Route("check/usernameexist")]
        public async Task<IActionResult> IsUserNameExistAsync(string userName)
        {
            var userDetail = await _userRepo.GetSingleUserAsync(true, x => x.UserName == userName);
            if (userDetail != null)
                return OKResult(1, "username already exist");

            return OKResult(0, "username not found");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sendemail")]
        public async Task<IActionResult> SendEmailAsync(string name, string email)
        {
            var mailContent = await PrepareConfirmEmailTemplate(name, null, null);

            var color = "";
            var content = "";
            var mailstatus = "";
            try
            {
                await _emailService.SendMailAsync(name, email, "", AppCommon.AppName + " - Test mail", mailContent, "");
                color = "#02815b";
                mailstatus = "Mail successfully sent";
                content = "Email successfully sent to <b>" + email + "</b> with name <b>" + name + "</b>. Please check the mailbox.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                color = "#b71f1f";
                mailstatus = "Mail couldn't be sent";
                content = "Some error occured while sending email.";
            }

            var responseStr = "";
            var templatefile = AppCommon.SendEmailResponseTemplateFilePath;
            using (var reader = new StreamReader(templatefile))
                responseStr = await reader.ReadToEndAsync();

            responseStr =
                responseStr
                .Replace("[color]", color)
                .Replace("[mailstatus]", mailstatus)
                .Replace("[content]", content);

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = responseStr
            };
        }

        #region Prepare email
        private async Task<string> PrepareConfirmEmailTemplate(string fullname, string email, string code)
        {
            var templateStr = "";

            var currHttpScheme = _httpContext.HttpContext.Request.Scheme;
            var currHost = _httpContext.HttpContext.Request.Host.Value;
            var currHostUrl = currHttpScheme + "://" + currHost;

            var confirmEmailRouteUrlPart = "/confirmemail?email=[email]&code=[code]";

            var callbackUrl = "javascript:void(0)";
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(email))
            {
                callbackUrl =
                    currHostUrl +
                    confirmEmailRouteUrlPart
                        .Replace("[email]", WebUtility.UrlEncode(email))
                        .Replace("[code]", WebUtility.UrlEncode(code));
            }

            templateStr =
                "Thanks for signing up with " + AppCommon.AppName + "! <br>" +
                "We encountered some issue generating proper email. But you can still verify your email account by clicking on following link.<br><br>" +
                "<a " +
                "href='[verifyaccounturl]' " +
                "target='_blank' " +
                ">VERIFY ACCOUNT</a>";
            try
            {
                var emailTemplatefile = AppCommon.ConfirmEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[verifyaccounturl]", callbackUrl)
                .Replace("[fullname]", fullname);

            return templateStr;
        }
        private async Task<string> PrepareSetPasswordEmailTemplate(string fullname, string email, string code)
        {
            var templateStr = "";

            var currHttpScheme = _httpContext.HttpContext.Request.Scheme;
            var currHost = _httpContext.HttpContext.Request.Host.Value;
            var currHostUrl = currHttpScheme + "://" + currHost;

            var setPasswordRouteUrlPart = "/setpassword?email=[email]&code=[code]";

            var callbackUrl = "javascript:void(0)";
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(email))
            {
                callbackUrl =
                    currHostUrl +
                    setPasswordRouteUrlPart
                        .Replace("[email]", WebUtility.UrlEncode(email))
                        .Replace("[code]", WebUtility.UrlEncode(code));
            }

            templateStr =
                "Thanks for confirming your email! <br>" +
                "We encountered some issue generating proper email. But you can still continue next step for setting your username and password by clicking on the following link.<br><br>" +
                "<a " +
                "href='[setpasswordurl]' " +
                "target='_blank' " +
                ">SET USERNAME & PASSWORD</a>";
            try
            {
                var emailTemplatefile = AppCommon.SetPasswordEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[setpasswordurl]", callbackUrl)
                .Replace("[fullname]", fullname);

            return templateStr;
        }
        private async Task<string> PrepareResetPasswordEmailTemplate(string fullname, string email, string code)
        {
            var templateStr = "";

            var currHttpScheme = _httpContext.HttpContext.Request.Scheme;
            var currHost = _httpContext.HttpContext.Request.Host.Value;
            var currHostUrl = currHttpScheme + "://" + currHost;

            var resetPasswordRouteUrlPart = "/resetpassword?email=[email]&code=[code]";

            var callbackUrl = "javascript:void(0)";
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(email))
            {
                callbackUrl =
                    currHostUrl +
                    resetPasswordRouteUrlPart
                        .Replace("[email]", WebUtility.UrlEncode(email))
                        .Replace("[code]", WebUtility.UrlEncode(code));
            }

            templateStr =
                "We have got a request to reset your password for App.<br>" +
                "We encountered some issue generating proper email. But you can still continue next step for resetting your password by clicking on the following link.<br><br>" +
                "<a " +
                "href='[resetpasswordurl]' " +
                "target='_blank' " +
                ">RESET PASSWORD</a>";
            try
            {
                var emailTemplatefile = AppCommon.ResetPasswordEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[resetpasswordurl]", callbackUrl)
                .Replace("[fullname]", fullname);

            return templateStr;
        }
        #endregion
    }
}
