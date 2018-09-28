using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.BL.Data.DTO;

namespace App.BL.Interfaces
{
	public interface IUserManagementRepository : IGenericRepository<ApplicationUser>
	{
		Task<ApplicationUser> GetUserByEmailAsync(string email, bool asNoTracking);
		Task<ApplicationUser> GetSingleUserAsync(bool asNoTracking, Expression<Func<ApplicationUser, bool>> predicate,
					params Expression<Func<ApplicationUser, object>>[] includeProperties);
		Task<List<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate, bool asNoTracking);
		Task<bool> IsUserExists(string email);
    }

}