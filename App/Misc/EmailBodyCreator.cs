using App.BL;
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
        public static async Task<string> CreateExceptionEmailBody(Exception ex, string errorMsg, string reqPath, string reqMathod, string payload, string userId, string email, string remoteIp, DateTime errorDateTime)
        {
            var templateStr = "";

            if (string.IsNullOrEmpty(userId))
                userId = "N/A";

            if (string.IsNullOrEmpty(email))
                email = "N/A";

            if (string.IsNullOrEmpty(remoteIp))
                remoteIp = "N/A";

            if (string.IsNullOrEmpty(payload))
                payload = "N/A";

            templateStr =
                "<b>Datetime (UTC): </b>" + errorDateTime.ToString("MMM dd, yyyy HH:mm:ss") + "<br><br>" +
                "<b>Request " + reqMathod + ": </b>" + reqPath + "<br><br>" +
                "<b>Payload: </b>" + payload + "<br><br>" +
                "<b>Userid: </b>" + userId + "<br><br>" +
                "<b>Email: </b>" + email + "<br><br>" +
                "<b>Remote ip: </b>" + remoteIp + "<br><br>" +
                "<b>Error message: </b>" + errorMsg + "<br><br>" +
                "<b>Exception: </b>" + ex.ToString();

            try
            {
                var emailTemplatefile = AppCommon.ExceptionEmailTemplateFilePath;

                using (var reader = new StreamReader(emailTemplatefile))
                    templateStr = await reader.ReadToEndAsync();
            }
            catch { }

            templateStr =
                templateStr
                .Replace("[reqpath]", reqPath)
                .Replace("[reqmethod]", reqMathod)
                .Replace("[payload]", payload)
                .Replace("[remoteip]", remoteIp)
                .Replace("[email]", email)
                .Replace("[userid]", userId)
                .Replace("[errmsg]", errorMsg)
                .Replace("[errdt]", errorDateTime.ToString("MMM dd, yyyy HH:mm:ss"))
                .Replace("[exception]", ex.ToString());

            return templateStr;
        }
    }
}
