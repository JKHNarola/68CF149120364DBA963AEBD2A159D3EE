using System.Threading.Tasks;
using App.BL.Data.DTO;
using App.BL.Data.ViewModel;

namespace App.BL.Interfaces
{
    public interface IProductManagementRepository : IGenericRepository<Product>
    {
        Task<DataSourceResult<ProductViewModel>> ListAsync(Query q);
    }
}