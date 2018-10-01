using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace App.BL
{
    public static class AppCommon
    {
        public static JsonSerializerSettings SerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                };
            }
        }
        public const string AppName = "App";

        private static readonly string currDirectory = Directory.GetCurrentDirectory();

        private const string applicationFilesFolderName = "ApplicationFiles";
        private const string emailTemplatesFolderName = "EmailTemplates";

        private const string userFilesFolderName = "UserFiles";
        private const string userFilesRequestName = "/Userfiles";

        public static string UserFilesFolderName { get { return userFilesFolderName; } }
        public static string UserFilesRequestName { get { return userFilesRequestName; } }

        public static string ConfirmEmailTemplateFilePath
        {
            get
            {
                var filePath = Path.Combine(currDirectory, applicationFilesFolderName, emailTemplatesFolderName, "ConfirmEmailTemplate.html");
                if (File.Exists(filePath))
                    return filePath;
                else
                    return "";
            }
        }

        public static string SetPasswordEmailTemplateFilePath
        {
            get
            {
                var filePath = Path.Combine(currDirectory, applicationFilesFolderName, emailTemplatesFolderName, "SetPasswordEmailTemplate.html");
                if (File.Exists(filePath))
                    return filePath;
                else
                    return "";
            }
        }

        public static string ResetPasswordEmailTemplateFilePath
        {
            get
            {
                var filePath = Path.Combine(currDirectory, applicationFilesFolderName, emailTemplatesFolderName, "ResetPasswordEmailTemplate.html");
                if (File.Exists(filePath))
                    return filePath;
                else
                    return "";
            }
        }

        public static string ExceptionEmailTemplateFilePath
        {
            get
            {
                var filePath = Path.Combine(currDirectory, applicationFilesFolderName, emailTemplatesFolderName, "ExceptionEmailTemplate.html");
                if (File.Exists(filePath))
                    return filePath;
                else
                    return "";
            }
        }

        public static string SendEmailResponseTemplateFilePath
        {
            get
            {
                var filePath = Path.Combine(currDirectory, applicationFilesFolderName, "SendmailResponseTemplate.html");
                if (File.Exists(filePath))
                    return filePath;
                else
                    return "";
            }
        }
    }
}
