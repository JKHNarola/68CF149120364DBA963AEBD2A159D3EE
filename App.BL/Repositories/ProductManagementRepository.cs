using System.Threading.Tasks;
using App.BL.Data.DTO;
using App.BL.Interfaces;
using App.BL.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using App.BL.Data.ViewModel;

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

        public async Task<DataSourceResult<ProductViewModel>> ListAsync(Query q)
        {
            var query = from p in _db.Products
                        from c in _db.Categories.Where(x => p.CategoryID == x.CategoryID).DefaultIfEmpty()
                        from s in _db.Suppliers.Where(x => p.SupplierID == x.SupplierID).DefaultIfEmpty()
                        select new ProductViewModel()
                        {
                            CategoryID = c.CategoryID,
                            CategoryName = c.CategoryName,
                            CategoryPicture = c.Picture,
                            Discontinued = p.Discontinued,
                            ProductID = p.ProductID,
                            ProductName = p.ProductName,
                            QuantityPerUnit = p.QuantityPerUnit,
                            ReorderLevel = p.ReorderLevel,
                            SupplierAddress = s.Address,
                            SupplierID = s.SupplierID,
                            SupplierCompanyName = s.CompanyName,
                            SupplierContactName = s.ContactName,
                            SupplierContactTitle = s.ContactTitle,
                            UnitPrice = p.UnitPrice,
                            UnitsInStock = p.UnitsInStock,
                            UnitsOnOrder = p.UnitsOnOrder
                        };
            return await query.ToDatsSourceResultAsync(q);
        }
    }
}
