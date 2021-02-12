using System;
using System.Collections.Generic;
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

        public static string GetFullNameFromFormattedString(string formattedString)                 // Expects format: Full Name <email@email.com>
        {
            string fullName = formattedString.Split("<")[0].Trim();
            return fullName;
        }

        public static string GetDateRangeQueryString(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            string queryString = $"?fromDate={fromDate.Date.ToString("yyyy-MM-dd")}&toDate={toDate.Date.ToString("yyyy-MM-dd")}";
            return queryString;
        }

        public static string FormatSecondsToHoursAndMinutes(double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            string formattedTime = $"{timeSpan.Hours} hours, {timeSpan.Minutes} minutes";
            return formattedTime;
        }

        public static int GetMinutesFromSeconds(double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.Minutes;
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

        public static List<DateTime> SplitDateRangeToChunks(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            DateTime startDate = fromDate.Date;
            DateTime endDate = toDate.Date;
            List<DateTime> dateTimeChunks = new List<DateTime>();

            while (startDate <= endDate)
            {
                dateTimeChunks.Add(startDate);
                startDate = startDate.AddDays(1);
            }

            return dateTimeChunks;
        }
    }
}
