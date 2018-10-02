using App.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public IActionResult TestError([FromQuery] string m)
        {
            var x = Convert.ToInt32(m) / 0;
            return OKResult(1, new { x });
        }

        [Authorize]
        [HttpPost]
        [Route("error/post")]
        public IActionResult PostTestError([FromBody] Data d)
        {
            var x = Convert.ToInt32(d.X) / 0;
            return OKResult(1, new { x });
        }

        [HttpGet]
        [Route("statuscode")]
        public IActionResult TestCode()
        {
            return OtherResult(HttpStatusCode.BadRequest);
        }

        public class Data
        {
            public string X { get; set; }
            public string Y { get; set; }
        }

    }
}
