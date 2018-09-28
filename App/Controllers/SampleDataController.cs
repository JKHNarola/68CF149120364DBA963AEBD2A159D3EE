using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Controllers
{
    [Route("api/demo")]
    public class SampleDataController : BaseController
    {
        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return OKResult(1, "sample", new { x = 50 });
        }

        [HttpGet]
        [Route("test/error")]
        public IActionResult TestError()
        {
            return OtherResult(HttpStatusCode.InternalServerError, "error in data", new { x = "50" });
        }

        [HttpGet]
        [Route("test/statuscode")]
        public IActionResult TestCode()
        {
            return OtherResult(HttpStatusCode.BadRequest);
        }

    }
}
