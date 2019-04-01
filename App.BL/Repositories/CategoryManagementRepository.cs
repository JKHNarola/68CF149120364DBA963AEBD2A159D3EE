using System.Threading.Tasks;
using App.BL.Data.DTO;
using App.BL.Interfaces;
using App.BL.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using App.BL.Data.ViewModel;

namespace App.BL.Repositories
{
    public class CategoryManagementRepository : GenericRepository<Category>, ICategoryManagementRepository
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _db;

        public CategoryManagementRepository(
            ILogger<CategoryManagementRepository> logger,
            ApplicationDbContext db
            ) : base(db)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<DataSourceResult<CategoryViewModel>> ListAsync(Query q)
        {
            var withImages = q.GetExtraValue<bool>("WithImages");
            if (!withImages.HasValue) withImages = false;

            var query = from c in _db.Categories
                        select new CategoryViewModel()
                        {
                            CategoryID = c.CategoryID,
                            Name = c.CategoryName,
                            Description = c.Description,
                            Picture = withImages.Value ? c.Picture : null
                        };
            return await query.ToDatsSourceResultAsync(q);
        }
    }
}
