using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.BL.Data.DTO;
using App.BL.Data.ViewModel;

namespace App.BL.Interfaces
{
    public interface IUserManagementRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByEmailAsync(string email, bool asNoTracking);
        Task<ApplicationUser> GetSingleAsync(bool asNoTracking, Expression<Func<ApplicationUser, bool>> predicate,
                    params Expression<Func<ApplicationUser, object>>[] includeProperties);
        Task<List<ApplicationUser>> GetAsync(Expression<Func<ApplicationUser, bool>> predicate, bool asNoTracking);
        Task<List<ApplicationRole>> GetRolesAsync();
        Task<bool> IsUserExists(string email);

        #region Account Management
        Task<LoginSuccessViewModel> LoginAsync(LoginViewModel model);
        Task<KeyValuePair<int, string>> RegisterAsync(RegisterViewModel model);
        Task<bool> ConfirmEmailAsync(string email, string code);
        Task<KeyValuePair<int, LoginSuccessViewModel>> SetPasswordAsync(SetPasswordViewModel model);
        Task<bool> ForgotPasswordAsync(string email);
        Task<KeyValuePair<int, LoginSuccessViewModel>> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<KeyValuePair<int, object>> ChangePasswordAsync(ChangePasswordViewModel model, ApplicationUser user);
        Task LogoutAsync();
        #endregion
    }
}