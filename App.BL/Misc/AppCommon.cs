using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace App.BL
{
    public static class AppCommon
    {
        static AppCommon()
        {
            Directory.CreateDirectory(UserfilesFolderPath);
        }

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

        private const string appfilesFolderName = "Appfiles";
        private const string emailtemplatesFolderName = "Emailtemplates";

        private const string userfilesFolderName = "Userfiles";
        private const string userfilesRequestName = "/userfiles";

        public static string UserfilesFolderName { get { return userfilesFolderName; } }
        public static string UserfilesFolderPath { get { return Path.Combine(currDirectory, userfilesFolderName); } }
        public static string UserfilesRequestName { get { return userfilesRequestName; } }

        public static string ConfirmEmailTemplateFilePath
        {
            get
            {
                var filePath = Path.Combine(currDirectory, appfilesFolderName, emailtemplatesFolderName, "ConfirmEmail.html");
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
                var filePath = Path.Combine(currDirectory, appfilesFolderName, emailtemplatesFolderName, "SetPassword.html");
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
                var filePath = Path.Combine(currDirectory, appfilesFolderName, emailtemplatesFolderName, "ResetPassword.html");
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
                var filePath = Path.Combine(currDirectory, appfilesFolderName, emailtemplatesFolderName, "Exception.html");
                if (File.Exists(filePath))
                    return filePath;
                else
                    return "";
            }
        }
    }
}
