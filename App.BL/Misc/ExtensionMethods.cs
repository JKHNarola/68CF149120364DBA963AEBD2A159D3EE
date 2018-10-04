using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

        public static string ToSerializedJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, AppCommon.SerializerSettings);
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }

        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.Identities.First().Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }

        public static Role? GetRole(this ClaimsPrincipal principal)
        {
            var roleStr = principal.Identities.First().Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(roleStr))
            {
                var role = Enum.Parse<Role>(roleStr);
                return role;
            }
            return null;
        }

        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req?.HttpContext?.Connection;
            if (connection?.RemoteIpAddress != null)
            {
                if (IPAddress.IsLoopback(connection?.RemoteIpAddress))
                {
                    return true;
                }
                else if (connection?.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection?.LocalIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection?.RemoteIpAddress == null && connection?.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }
    }
}
