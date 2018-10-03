using App.BL;
using App.BL.Interfaces;
using App.BL.Services;
using App.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace App.Controllers
{
    [Route("api/test")]
    public class DemoController : BaseController
    {
        private readonly EmailService _emailService;
        private readonly ILogger _logger;

        public DemoController(
           ILogger<DemoController> logger,
           IHttpContextAccessor httpContext,
           IOptions<EmailSettings> emailSettings,
           IUserManagementRepository userRepo
           ) : base(httpContext)
        {
            _emailService = new EmailService(emailSettings);
            _logger = logger;
        }

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
        public class Data
        {
            public string X { get; set; }
            public string Y { get; set; }
        }

        [Authorize]
        [HttpGet]
        [Route("statuscode")]
        public IActionResult TestCode()
        {
            return OtherResult(HttpStatusCode.BadRequest);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sendemail")]
        public async Task<IActionResult> SendEmailAsync(string name, string email)
        {
            var mailContent = await EmailBodyCreator.CreateConfirmEmailBody(GetCurrHost(), name, null, null);
            await _emailService.SendMailAsync(name, email, "", AppCommon.AppName + " - Test mail", mailContent, "");
            return OKResult(1, "mail sent to " + email);
        }
    }
}
