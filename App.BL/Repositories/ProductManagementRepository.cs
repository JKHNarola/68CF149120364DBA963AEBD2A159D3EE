using System.Threading.Tasks;
using App.BL.Data.DTO;
using App.BL.Interfaces;
using App.BL.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace App.BL.Repositories
{
    public class ProductManagementRepository : GenericRepository<Product>, IProductManagementRepository
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _db;

        public ProductManagementRepository(
            ILogger<ProductManagementRepository> logger,
            ApplicationDbContext db
            ) : base(db)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<Product>> ListAsync()
        {
            var q = new Q(0, 0);
            q.AddCondition("UnitPrice", 20, Operator.Ge);
            var query = GetAll(true);
            var result = await query.ToDatsSourceResultAsync(q);
            return result.Data;
        }
    }
}
