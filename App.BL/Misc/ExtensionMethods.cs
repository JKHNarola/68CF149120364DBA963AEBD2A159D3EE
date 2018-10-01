using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;

namespace App.BL
{
    public static class ExtensionMethods
    {
        public static string Description(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(false);

            dynamic displayAttribute = null;

            if (attributes.Any())
            {
                displayAttribute = attributes.ElementAt(0);
            }

            return displayAttribute?.Description ?? null;
        }

        public static string DescriptionAttr<T>(this T source)
        {
            var fi = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }

        public static string ToSerializedJsonString(this object src)
        {
            try
            {
                return JsonConvert.SerializeObject(src, AppCommon.SerializerSettings);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().Claims.First(x => x.Type == ClaimTypes.Sid).Value;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().Claims.First(x => x.Type == ClaimTypes.Email).Value;
        }

        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().Claims.First(x => x.Type == ClaimTypes.Name).Value;
        }

        public static Role GetRole(this ClaimsPrincipal principal)
        {
            var roleStr = principal.Identities.First().Claims.First(x => x.Type == ClaimTypes.Role).Value;
            var role = Enum.Parse<Role>(roleStr);
            return role;
        }
    }
}
