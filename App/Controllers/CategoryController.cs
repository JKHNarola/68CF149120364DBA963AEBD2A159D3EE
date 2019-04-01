using App.BL;
using App.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace App.Controllers
{
    [Route("api/category")]
    public class CategoryController : BaseController
    {
        private readonly ILogger _logger;
        private readonly ICategoryManagementRepository _catRepo;

        public CategoryController(
           ILogger<CategoryController> logger,
           ICategoryManagementRepository catRepo
           ) : base()
        {
            _logger = logger;
            _catRepo = catRepo;
        }

        [Authorize]
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> ListAsync(string q)
        {
            var query = JsonConvert.DeserializeObject<Query>(q, AppCommon.SerializerSettings);
            var res = await _catRepo.ListAsync(query);
            return OKResult(res);
        }
    }
}
