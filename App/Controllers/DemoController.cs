using App.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Controllers
{
    [Authorize]
    [Route("api/test")]
    public class DemoController : BaseController
    {
        [HttpGet]
        public IActionResult Get()
        {
            var userId = User.GetUserId();
            var email = User.GetEmail();
            var username = User.GetUsername();
            var role = User.GetRole();
            return OKResult(1, "Authorized user", new { Id = userId, Email = email, Username = username, Role = role.ToString() });
        }

        [HttpGet]
        [Route("error")]
        public IActionResult TestError()
        {
            return OtherResult(HttpStatusCode.InternalServerError, "error in data", new { x = 50 });
        }

        [HttpGet]
        [Route("statuscode")]
        public IActionResult TestCode()
        {
            return OtherResult(HttpStatusCode.BadRequest);
        }

    }
}
