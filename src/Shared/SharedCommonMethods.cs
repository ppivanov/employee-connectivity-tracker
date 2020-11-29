using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EctBlazorApp.Shared
{
    public static class SharedCommonMethods
    {
        public static DateTime NewDateTimeFromString(string dateString)                             // Expects the string to be in yyyy-MM-dd format
        {
            var dateArray = dateString.Split("-");

            DateTime newDate = new DateTime(
                int.Parse(dateArray[0]), 
                int.Parse(dateArray[1]), 
                int.Parse(dateArray[2]));

            return newDate;
        }

        public static string FormatFullNameAndEmail(string fullName, string email)
        {
            return $"{fullName} <{email}>";
        }

        // Code source: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format#example
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }

                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
