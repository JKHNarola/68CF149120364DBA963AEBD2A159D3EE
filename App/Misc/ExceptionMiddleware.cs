﻿using App.BL;
using App.BL.Services;
using App.Misc;
using App.Models;
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
            var model = new ErrorDetail()
            {
                ConnectionId = context.Connection.Id,
                RequestUrl = context.Request.Path,
                Userid = context.User?.GetUserId(),
                UserEmail = context.User?.GetEmail(),
                RemoteIp = context.Connection?.RemoteIpAddress?.ToString(),
                Ex = contextFeature.Error,
                DateTime = DateTime.UtcNow,
                RequestMethod = context.Request.Method,
                Request = context.Request
            };

            var contentType = context.Request.ContentType;
            if (model.RequestMethod == "GET" && context.Request.QueryString.HasValue)
            {
                var content = context.Request.QueryString.Value;
                if (content.Length > 3000)
                    content = content.Substring(3000) + ".....";
                model.Payload = content;
            }
            else
            {
                if (string.IsNullOrEmpty(contentType) || contentType == "text/plain" || contentType == "application/json")
                {
                    var reader = new StreamReader(context.Request.Body);
                    context.Request.Body.Position = 0;
                    var content = reader.ReadToEnd();
                    reader.Close();
                    if (content.Length > 3000)
                        content = content.Substring(3000) + ".....";
                    model.Payload = content;
                }
                else if (contentType == "application/octet-stream")
                {
                    model.Payload = "File is posted with request.";
                }
                else
                {
                    model.Payload = "Data other than 'text/plain', 'application/json' and 'application/octet-stream' posted.";
                }
            }

            var emailSettings = new EmailSettings();
            config.GetSection("EmailSettings").Bind(emailSettings);
            var appSettings = new AppSettings();
            config.GetSection("AppSettings").Bind(appSettings);

            var emailService = new EmailService(emailSettings);
            var body = await EmailBodyCreator.CreateExceptionEmailBody(model);
            await emailService.SendMailAsync(appSettings.ExceptionEmailSendToName, appSettings.ExceptionEmailSendTo, null, "App - Exception", body, null);
        }
    }
}
