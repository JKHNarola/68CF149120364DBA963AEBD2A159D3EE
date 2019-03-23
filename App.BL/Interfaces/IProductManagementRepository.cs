using System.Collections.Generic;
using System.Threading.Tasks;
using App.BL.Data.DTO;

namespace App.BL.Interfaces
{
    public interface IProductManagementRepository : IGenericRepository<Product>
    {
        Task<List<Product>> ListAsync();
    }
}