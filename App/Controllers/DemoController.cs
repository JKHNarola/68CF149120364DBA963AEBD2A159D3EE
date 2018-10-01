using App.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Controllers
{
    [Route("api/test")]
    public class DemoController : BaseController
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var userId = User.GetUserId();
            var email = User.GetEmail();
            var username = User.GetUsername();
            var role = User.GetRole();
            return OKResult(1, "Authorized user", new { Id = userId, Email = email, Username = username, Role = role.ToString() });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("auth/role")]
        public IActionResult RoleTest()
        {
            var userId = User.GetUserId();
            var email = User.GetEmail();
            var username = User.GetUsername();
            var role = User.GetRole();
            return OKResult(1, "Authorized admin", new { Id = userId, Email = email, Username = username, Role = role.ToString() });
        }

        [Authorize]
        [HttpGet]
        [Route("error")]
        public IActionResult TestError()
        {
            var m = 100;
            var x = m / 0;
            return OtherResult(HttpStatusCode.InternalServerError, "error in data", new { x });
        }

        [HttpGet]
        [Route("statuscode")]
        public IActionResult TestCode()
        {
            return OtherResult(HttpStatusCode.BadRequest);
        }

    }
}
