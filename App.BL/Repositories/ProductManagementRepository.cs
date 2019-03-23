using System.Threading.Tasks;
using App.BL.Data.DTO;
using App.BL.Interfaces;
using App.BL.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

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
            //var pNameWhr = DynamicLinqHelper.CreateDynamicExpression<Product>("ProductName",
            //        Operator.Contains, "Boysenberry", typeof(string));
            //var qtyWhr = DynamicLinqHelper.CreateDynamicExpression<Product>("QuantityPerUnit",
            //        Operator.StartsWith, "12", typeof(string));

            //Expression.And(pNameWhr, qtyWhr);



            return await AllIncluding(false, x => x.Category).Where("").ToListAsync();
        }
    }
}
