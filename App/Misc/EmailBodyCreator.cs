using App.BL;
using App.Models;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace App.Misc
{
    public static class EmailBodyCreator
    {
        public static async Task<string> CreateConfirmEmailBody(string hostUrl, string fullname, string email, string code)
        {
            var templateStr = "";

            var currHostUrl = hostUrl;

            var confirmEmailRouteUrlPart = "/confirmemail?email=[email]&code=[code]";

            var callbackUrl = "javascript:void(0)";
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(email))
            {
                callbackUrl =
                    currHostUrl +
                    confirmEmailRouteUrlPart
                        .Replace("[email]", WebUtility.UrlEncode(email))
                        .Replace("[code]", WebUtility.UrlEncode(code));
            }

            templateStr =
                "Thanks for signing up with " + AppCommon.AppName + "! <br>" +
                "We encountered some issue generating proper email. But you can still verify your email account by clicking on following link.<br><br>" +
                "<a " +
                "href='[verifyaccounturl]' " +
                "target='_blank' " +
                ">[verifyaccounturl]</a>" +
                "<div style='margin-top:30px;'>Regards,</div>" +
                "<div>Admin</div>"
                ;
            try
            {
                var emailTemplatefile = AppCommon.ConfirmEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[verifyaccounturl]", callbackUrl)
                .Replace("[fullname]", fullname);

            return templateStr;
        }
        public static async Task<string> CreateSetPasswordEmailBody(string hostUrl, string fullname, string email, string code)
        {
            var templateStr = "";

            var currHostUrl = hostUrl;

            var setPasswordRouteUrlPart = "/setpassword?email=[email]&code=[code]";

            var callbackUrl = "javascript:void(0)";
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(email))
            {
                callbackUrl =
                    currHostUrl +
                    setPasswordRouteUrlPart
                        .Replace("[email]", WebUtility.UrlEncode(email))
                        .Replace("[code]", WebUtility.UrlEncode(code));
            }

            templateStr =
                "Thanks for confirming your email! <br>" +
                "We encountered some issue generating proper email. But you can still continue next step for setting your username and password by clicking on the following link.<br><br>" +
                "<a " +
                "href='[setpasswordurl]' " +
                "target='_blank' " +
                ">SET USERNAME & PASSWORD</a>" +
                "<div style='margin-top:30px;'>Regards,</div>" +
                "<div>Admin</div>";
            try
            {
                var emailTemplatefile = AppCommon.SetPasswordEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[setpasswordurl]", callbackUrl)
                .Replace("[fullname]", fullname);

            return templateStr;
        }
        public static async Task<string> CreateResetPasswordEmailBody(string hostUrl, string fullname, string email, string code)
        {
            var templateStr = "";

            var currHostUrl = hostUrl;

            var resetPasswordRouteUrlPart = "/resetpassword?email=[email]&code=[code]";

            var callbackUrl = "javascript:void(0)";
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(email))
            {
                callbackUrl =
                    currHostUrl +
                    resetPasswordRouteUrlPart
                        .Replace("[email]", WebUtility.UrlEncode(email))
                        .Replace("[code]", WebUtility.UrlEncode(code));
            }

            templateStr =
                "We have got a request to reset your password for App.<br>" +
                "We encountered some issue generating proper email. But you can still continue next step for resetting your password by clicking on the following link.<br><br>" +
                "<a " +
                "href='[resetpasswordurl]' " +
                "target='_blank' " +
                ">RESET PASSWORD</a>" +
                "<div style='margin-top:30px;'>Regards,</div>" +
                "<div>Admin</div>";
            try
            {
                var emailTemplatefile = AppCommon.ResetPasswordEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[resetpasswordurl]", callbackUrl)
                .Replace("[fullname]", fullname);

            return templateStr;
        }
        public static async Task<string> CreateExceptionEmailBody(ErrorDetail model)
        {
            var templateStr = "";

            if (string.IsNullOrEmpty(model.Userid))
                model.Userid = "N/A";

            if (string.IsNullOrEmpty(model.UserEmail))
                model.UserEmail = "N/A";

            if (string.IsNullOrEmpty(model.RemoteIp))
                model.RemoteIp = "N/A";

            if (string.IsNullOrEmpty(model.Payload))
                model.Payload = "N/A";

            if (string.IsNullOrEmpty(model.ConnectionId))
                model.ConnectionId = "N/A";

            var reqHeaders = "";
            foreach (var x in model.Request.Headers)
                reqHeaders += x.Key + " = " + x.Value + "<br>";

            if (string.IsNullOrEmpty(reqHeaders))
                reqHeaders = "N/A";

            templateStr =
                "<b>Datetime (" + model.TimezoneName + "): </b>" + model.DateTime.ToString("MMM dd, yyyy hh:mm:ss tt") + "<br><br>" +
                "<b>Error message: </b>" + model.Ex.Message + "<br><br>" +
                "<b>Exception: </b>" + model.Ex.ToString() +
                "<b>Connection id: </b>" + model.ConnectionId + "<br><br>" +
                "<b>Request " + model.RequestMethod + ": </b>" + model.RequestUrl + "<br><br>" +
                "<b>Payload: </b>" + model.Payload + "<br><br>" +
                "<b>Request headers: </b>" + model.Payload + "<br><br>" +
                "<b>Userid: </b>" + model.Userid + "<br><br>" +
                "<b>Email: </b>" + model.UserEmail + "<br><br>" +
                "<b>Remote ip: </b>" + model.RemoteIp + "<br><br>";

            try
            {
                var emailTemplatefile = AppCommon.ExceptionEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[reqpath]", model.RequestUrl)
                .Replace("[timezone]", model.TimezoneName)
                .Replace("[reqmethod]", model.RequestMethod)
                .Replace("[payload]", model.Payload)
                .Replace("[connid]", model.ConnectionId)
                .Replace("[reqheaders]", reqHeaders)
                .Replace("[remoteip]", model.RemoteIp)
                .Replace("[email]", model.UserEmail)
                .Replace("[userid]", model.Userid)
                .Replace("[errmsg]", model.Ex.Message)
                .Replace("[errdt]", model.DateTime.ToString("MMM dd, yyyy hh:mm:ss tt"))
                .Replace("[exception]", model.Ex.ToString());

            return templateStr;
        }
    }
}
