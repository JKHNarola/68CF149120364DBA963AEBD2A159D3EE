using App.BL;
using App.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace App.Controllers
{
    [Route("api/product")]
    public class ProductController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IProductManagementRepository _prodRepo;

        public ProductController(
           ILogger<ProductController> logger,
           IProductManagementRepository prodRepo
           ) : base()
        {
            _logger = logger;
            _prodRepo = prodRepo;
        }

        [Authorize]
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> ListAsync(string q)
        {
            var query = JsonConvert.DeserializeObject<Query>(q, AppCommon.SerializerSettings);
            var res = await _prodRepo.ListAsync(query);
            return OKResult(res);
        }
    }
}
