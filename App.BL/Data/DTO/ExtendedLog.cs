using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace App.BL.Data.DTO
{
    public class ExtendedLog : Log
    {
        public ExtendedLog(IHttpContextAccessor accessor)
        {
            var context = accessor?.HttpContext;

            string browser = context?.Request?.Headers["User-Agent"];
            if (!string.IsNullOrEmpty(browser) && (browser.Length > 255))
                browser = browser.Substring(0, 255);

            Browser = browser;

            var isLocal = context?.Request?.IsLocal();
            if (isLocal.HasValue && isLocal.Value)
                ReqIp = "local";
            else
                ReqIp = context?.Connection?.RemoteIpAddress?.ToString();

            UserId = context?.User?.GetUserId();
            ReqPath = context?.Request.Path;
            ReqMethod = context?.Request.Method;

            var request = context?.Request;

            var contentType = request?.ContentType;
            if (context != null && request != null)
            {
                try
                {
                    if (ReqMethod == "GET" && request.QueryString.HasValue)
                    {
                        var content = request.QueryString.Value;
                        if (!string.IsNullOrEmpty(content) && content.Length > 5000)
                            content = content.Substring(5000) + ".....";
                        ReqPayload = content;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(contentType) || contentType == "text/plain" || contentType == "application/json")
                        {
                            var reader = new StreamReader(context.Request.Body);
                            context.Request.Body.Position = 0;
                            var content = reader.ReadToEnd();
                            reader.Close();
                            if (content.Length > 5000)
                                content = content.Substring(5000) + ".....";
                            ReqPayload = content;
                        }
                        else if (contentType == "application/octet-stream")
                            ReqPayload = "File is posted with request.";
                        else
                            ReqPayload = "Data other than 'text/plain', 'application/json' and 'application/octet-stream' posted.";
                    }
                }
                catch { }
            }

            ReqHeaders = "";
            try
            {
                if (request != null)
                    foreach (var x in request.Headers)
                        ReqHeaders += x.Key + " = " + x.Value + Environment.NewLine;
            }
            catch { }
        }

        protected ExtendedLog()
        {
        }

        public string Browser { get; set; }
        public string UserId { get; set; }
        public string ReqIp { get; set; }
        public string ReqPath { get; set; }
        public string ReqHeaders { get; set; }
        public string ReqPayload { get; set; }
        public string ReqMethod { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
