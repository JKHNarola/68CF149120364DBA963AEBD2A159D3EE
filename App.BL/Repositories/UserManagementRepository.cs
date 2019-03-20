using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.BL.Data.DTO;
using App.BL.Interfaces;
using App.BL.Data;
using App.BL.Data.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using App.BL.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace App.BL.Repositories
{
    public class UserManagementRepository : GenericRepository<ApplicationUser>, IUserManagementRepository
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly EmailService _emailService;
        private readonly AppSettings _appSetting;

        public UserManagementRepository(
            ILogger<UserManagementRepository> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db,
            IHttpContextAccessor httpContext,
            IOptions<EmailSettings> emailSettings,
            IOptions<AppSettings> appSettings
            ) : base(db)
        {
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContext = httpContext;
            _db = db;
            _emailService = new EmailService(emailSettings);
            _appSetting = appSettings.Value;
            _logger = logger;
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email, bool asNoTracking) => await GetAll(asNoTracking).FirstOrDefaultAsync(x => x.Email == email);

        public async Task<ApplicationUser> GetSingleAsync(bool asNoTracking, Expression<Func<ApplicationUser, bool>> predicate, params Expression<Func<ApplicationUser, object>>[] includeProperties)
        {
            if (asNoTracking)
                return await GetSingleWithoutTrackingAsync(predicate, includeProperties);
            else
                return await GetSingleAsync(predicate, includeProperties);
        }

        public async Task<List<ApplicationUser>> GetAsync(Expression<Func<ApplicationUser, bool>> predicate, bool asNoTracking) => await ListAsync(predicate, asNoTracking);

        public async Task<List<ApplicationRole>> GetRolesAsync() => await _db.ApplicationRoles.ToListAsync();

        public async Task<bool> IsUserExists(string email)
        {
            var cnt = await CountAsync(x => x.Email == email);
            return cnt != 0;
        }

        #region Account Management
        public async Task<LoginSuccessViewModel> LoginAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await GetSingleAsync(x => x.UserName == model.UserName);
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
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(AppCommon.SymmetricSecurityKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                var userData = new LoginSuccessViewModel
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

        public async Task<KeyValuePair<int, string>> RegisterAsync(RegisterViewModel model)
        {
            var isUserExixts = await IsUserExists(model.Email);
            if (isUserExixts)
                return new KeyValuePair<int, string>(-3, "User already exists for " + model.Email + " email.");

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
                return new KeyValuePair<int, string>(-2, resultCreateUser.Errors.FirstOrDefault()?.Description.ToLower());
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
                    return new KeyValuePair<int, string>(-7, "User successfully created but failed to set role, tried to remove user but error occured.");
                }

                return new KeyValuePair<int, string>(-8, "User successfully created but failed to set role, removed user.");
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var fullname = user.FirstName + " " + user.LastName;
            var mailContent = await EmailBodyCreator.CreateConfirmEmailBody(Utilities.GetCurrHost(_httpContext), fullname, user.Email, code);
            var fullName = user.FirstName + " " + user.LastName;
            try
            {
                await _emailService.SendMailAsync(new List<MailAddress>() { new MailAddress(user.Email, fullName) }, null, null, AppCommon.AppName + " - Verify Email", mailContent, null);
                return new KeyValuePair<int, string>(1, "User successfully created, email sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't sent mail. Error : " + ex.ToString());

                _logger.LogWarning("Removing user " + user.Email + " from role " + Role.WebUser.ToString());
                var resRemoveFromRole = await _userManager.RemoveFromRoleAsync(user, Role.WebUser.ToString());
                if (!resRemoveFromRole.Succeeded)
                {
                    _logger.LogError("Couldn't remove user " + user.Email + " from role " + Role.WebUser.ToString() + ", Error(s) : " + resRemoveFromRole.Errors.ToSerializedJsonString());
                    return new KeyValuePair<int, string>(-6, "User successfully created but failed to sent email, tried to remove user but error occured.");
                }

                _logger.LogWarning("Deleting user : " + user.Email);
                var resDeleteUser = await _userManager.DeleteAsync(user);
                if (!resDeleteUser.Succeeded)
                {
                    _logger.LogError("Couldn't delete user " + user.Email + ", Error(s) : " + resRemoveFromRole.Errors.ToSerializedJsonString());
                    return new KeyValuePair<int, string>(-6, "User successfully created but failed to sent email, tried to remove user but error occured.");
                }

                return new KeyValuePair<int, string>(-4, "User successfully created but failed to sent email, deleted user.");
            }
        }

        public async Task<bool> ConfirmEmailAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);

            code = WebUtility.UrlDecode(code);
            code = code.Replace(' ', '+');

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                var fullname = user.FirstName + " " + user.LastName;
                var mailContent = await EmailBodyCreator.CreateSetPasswordEmailBody(Utilities.GetCurrHost(_httpContext), fullname, user.Email, resetCode);
                var fullName = user.FirstName + " " + user.LastName;
                await _emailService.SendMailAsync(new List<MailAddress>() { new MailAddress(user.Email, fullName) }, null, null, AppCommon.AppName + " - Set Username & Password", mailContent, null);

                return true;
            }
            return false;
        }

        public async Task<KeyValuePair<int, LoginSuccessViewModel>> SetPasswordAsync(SetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new KeyValuePair<int, LoginSuccessViewModel>(0, null);

            user.UserName = model.UserName;
            model.Code = WebUtility.UrlDecode(model.Code);
            model.Code = model.Code.Replace(' ', '+');

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var loginRes = await LoginAsync(new LoginViewModel() { UserName = user.UserName, Password = model.Password });
                if (loginRes != null)
                    return new KeyValuePair<int, LoginSuccessViewModel>(1, loginRes);
            }
            return new KeyValuePair<int, LoginSuccessViewModel>(2, null);
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            var fullname = user.FirstName + " " + user.LastName;
            var mailContent = await EmailBodyCreator.CreateResetPasswordEmailBody(Utilities.GetCurrHost(_httpContext), fullname, user.Email, resetCode);
            var fullName = user.FirstName + " " + user.LastName;
            await _emailService.SendMailAsync(new List<MailAddress>() { new MailAddress(user.Email, fullName) }, null, null, AppCommon.AppName + " - Reset Password", mailContent, null);

            return true;
        }

        public async Task<KeyValuePair<int, LoginSuccessViewModel>> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new KeyValuePair<int, LoginSuccessViewModel>(0, null);

            model.Code = WebUtility.UrlDecode(model.Code);
            model.Code = model.Code.Replace(' ', '+');

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var loginRes = await LoginAsync(new LoginViewModel() { UserName = user.UserName, Password = model.NewPassword });
                if (loginRes != null)
                    return new KeyValuePair<int, LoginSuccessViewModel>(1, loginRes);
            }
            return new KeyValuePair<int, LoginSuccessViewModel>(2, null);
        }

        public async Task<KeyValuePair<int, object>> ChangePasswordAsync(ChangePasswordViewModel model, ApplicationUser user)
        {
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var loginRes = await LoginAsync(new LoginViewModel() { UserName = user.UserName, Password = model.NewPassword });
                if (loginRes != null)
                    return new KeyValuePair<int, object>(1, loginRes);
            }
            return new KeyValuePair<int, object>(2, result.Errors);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        #endregion
    }
}
