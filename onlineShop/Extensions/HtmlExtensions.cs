using Microsoft.AspNetCore.Html;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;

namespace onlineShop.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlString EnumDisplayNameFor(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());

            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
                return new HtmlString(displayName.Name);

            return new HtmlString(item.ToString());
        }

        public static string SetLengthLimit(this String text, int maxLen)
        {
            return (text.Length <= maxLen ? text : text.Substring(0, maxLen).TrimEnd() + "...");
        }

        public static bool IsValidEmailAddress(this String email)
        {
            try
            {
                var emailAddress = new MailAddress(email);
                return String.Equals(emailAddress.Address, email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static DateTime TryToLocalDateTime(this DateTime? nullableDateTime)
        {
            return (nullableDateTime.HasValue) ? 
                ((DateTime)nullableDateTime).ToLocalTime() : 
                DateTime.MinValue.ToLocalTime();
        }

        public static DateTime NullableToDateTime(this DateTime? nullableDateTime)
        {
            return (nullableDateTime.HasValue) ?
                ((DateTime)nullableDateTime) :
                DateTime.MinValue;
        }
    }
}
