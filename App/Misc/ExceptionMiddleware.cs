using App.BL;
using App.BL.Services;
using App.Misc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace App
{
    public static class ExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        await SendExceptionEmail(context, contextFeature, config);

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(
                            new ApiResult<object>
                            {
                                Message = contextFeature.Error.Message
                            }, AppCommon.SerializerSettings));
                    }
                });
            });

        }

        private static async Task SendExceptionEmail(HttpContext context, IExceptionHandlerFeature contextFeature, IConfiguration config)
        {
            var path = context.Request.Path;
            var userId = context.User?.GetUserId();
            var email = context.User?.GetEmail();
            var remoteIp = context.Connection?.RemoteIpAddress?.ToString();
            var exception = contextFeature.Error;
            var errorMsg = exception.Message;
            var date = DateTime.UtcNow;
            var httpMethod = context.Request.Method;
            var payload = "";
            if (httpMethod == "POST")
            {
                var stream = context.Request.Body;
                stream.Position = 0;
                var reader = new StreamReader(stream);
                payload = reader.ReadToEnd();
            }
            else if (httpMethod == "GET")
            {
                payload = context.Request.QueryString.Value;
            }

            var emailSettings = new EmailSettings();
            config.GetSection("EmailSettings").Bind(emailSettings);
            var appSettings = new AppSettings();
            config.GetSection("AppSettings").Bind(appSettings);

            var emailService = new EmailService(emailSettings);
            var body = await EmailBodyCreator.CreateExceptionEmailBody(exception, errorMsg, path, httpMethod, payload, userId, email, remoteIp, date);
            await emailService.SendMailAsync(appSettings.ExceptionEmailSendToName, appSettings.ExceptionEmailSendTo, null, "App - Exception", body, null);
        }
    }
}
