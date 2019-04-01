using System.Threading.Tasks;
using App.BL.Data.DTO;
using App.BL.Data.ViewModel;

namespace App.BL.Interfaces
{
    public interface ICategoryManagementRepository : IGenericRepository<Category>
    {
        Task<DataSourceResult<CategoryViewModel>> ListAsync(Query q);
    }
}