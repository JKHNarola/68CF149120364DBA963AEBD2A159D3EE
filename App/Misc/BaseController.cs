﻿using App.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace App
{
    public class BaseController : Controller
    {
        public BaseController() { }

        #region Api Result Helpers
        public IActionResult OKResult()
        {
            return StatusCode((int)HttpStatusCode.OK);
        }
        public IActionResult OKResult(int status)
        {
            return Json(PrepareResultObject<object>(status, null, null), AppCommon.SerializerSettings);
        }
        public IActionResult OKResult<T>(T data) where T : class
        {
            return Json(PrepareResultObject(null, null, data), AppCommon.SerializerSettings);
        }
        public IActionResult OKResult(string message)
        {
            return Json(PrepareResultObject(null, message, (object)null), AppCommon.SerializerSettings);
        }
        public IActionResult OKResult<T>(string message, T data) where T : class
        {
            return Json(PrepareResultObject<object>(null, message, data), AppCommon.SerializerSettings);
        }
        public IActionResult OKResult<T>(int status, T data) where T : class
        {
            return Json(PrepareResultObject(status, null, data), AppCommon.SerializerSettings);
        }
        public IActionResult OKResult(int status, string message)
        {
            return Json(PrepareResultObject(status, message, (object)null), AppCommon.SerializerSettings);
        }
        public IActionResult OKResult<T>(int status, string message, T data) where T : class
        {
            return Json(PrepareResultObject(status, message, data), AppCommon.SerializerSettings);
        }

        public IActionResult OtherResult(HttpStatusCode code)
        {
            return StatusCode((int)code);
        }
        public IActionResult OtherResult(HttpStatusCode code, int status)
        {
            var res = new JsonResult(PrepareResultObject(status, null, (object)null), AppCommon.SerializerSettings)
            {
                StatusCode = (int)code,
                ContentType = "application/json",
            };

            return res;
        }
        public IActionResult OtherResult(HttpStatusCode code, int status, string message)
        {
            var res = new JsonResult(PrepareResultObject(status, message, (object)null), AppCommon.SerializerSettings)
            {
                StatusCode = (int)code,
                ContentType = "application/json",
            };

            return res;
        }
        public IActionResult OtherResult(HttpStatusCode code, string message)
        {
            var res = new JsonResult(PrepareResultObject(null, message, (object)null), AppCommon.SerializerSettings)
            {
                StatusCode = (int)code,
                ContentType = "application/json",
            };

            return res;
        }
        public IActionResult OtherResult<T>(HttpStatusCode code, int status, T data) where T : class
        {
            var res = new JsonResult(PrepareResultObject(status, null, data), AppCommon.SerializerSettings)
            {
                StatusCode = (int)code,
                ContentType = "application/json",
            };

            return res;
        }
        public IActionResult OtherResult<T>(HttpStatusCode code, T data) where T : class
        {
            var res = new JsonResult(PrepareResultObject(null, null, data), AppCommon.SerializerSettings)
            {
                StatusCode = (int)code,
                ContentType = "application/json",
            };

            return res;
        }
        public IActionResult OtherResult<T>(HttpStatusCode code, int status, string message, T data) where T : class
        {
            var res = new JsonResult(PrepareResultObject(status, message, data), AppCommon.SerializerSettings)
            {
                StatusCode = (int)code,
                ContentType = "application/json",
            };

            return res;
        }
        public IActionResult OtherResult<T>(HttpStatusCode code, string message, T data) where T : class
        {
            var res = new JsonResult(PrepareResultObject(null, message, data), AppCommon.SerializerSettings)
            {
                StatusCode = (int)code,
                ContentType = "application/json",
            };

            return res;
        }

        public IActionResult InvalidModelStateResult(ModelStateDictionary modelState)
        {
            var res = new JsonResult(PrepareResultObject(null, null, modelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList()), AppCommon.SerializerSettings)
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ContentType = "application/json",
            };

            return res;
        }

        public async Task<IActionResult> FileResultAsync(string filepath, string contentDispositionHeaderValue = "attachment")
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filepath, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            var filename = Path.GetFileName(filepath);
            var contentDisposition = new ContentDispositionHeaderValue(contentDispositionHeaderValue);
            contentDisposition.SetHttpFileName(filename);
            Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();

            return File(memory, contentType, filename);
        }

        private ApiResult<T> PrepareResultObject<T>(int? status, string message, T data) where T : class
        {
            var resObj = new ApiResult<T>()
            {
                Data = data,
                Message = message,
                Status = status
            };

            return resObj;
        }
        #endregion

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
    }

    public class ApiResult<T> where T : class
    {
        public int? Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
