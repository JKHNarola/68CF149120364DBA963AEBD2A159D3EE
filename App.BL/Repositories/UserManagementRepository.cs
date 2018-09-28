using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.BL.Data.DTO;
using App.BL.Interfaces;
using App.BL.Data;

namespace App.BL.Repositories
{
    public class UserManagementRepository : GenericRepository<ApplicationUser>, IUserManagementRepository
    {
        private readonly ApplicationDbContext _db;
        public UserManagementRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email, bool asNoTracking) => await GetAll(asNoTracking).FirstOrDefaultAsync(x => x.Email == email);

        public async Task<ApplicationUser> GetSingleUserAsync(bool asNoTracking, Expression<Func<ApplicationUser, bool>> predicate, params Expression<Func<ApplicationUser, object>>[] includeProperties)
        {
            if (asNoTracking)
                return await GetSingleWithoutTrackingAsync(predicate, includeProperties);
            else
                return await GetSingleAsync(predicate, includeProperties);
        }

        public async Task<List<ApplicationUser>> GetUsersAsync(Expression<Func<ApplicationUser, bool>> predicate, bool asNoTracking) => await ListAsync(predicate, asNoTracking);

        public async Task<List<ApplicationRole>> GetRoleListAsync()
        {
            var rolesList = _db.ApplicationRoles.ToListAsync();
            return await rolesList;
        }

        public async Task<bool> IsUserExists(string email)
        {
            var cnt = await CountAsync(x => x.Email == email);
            return cnt != 0;
        }
    }
}
