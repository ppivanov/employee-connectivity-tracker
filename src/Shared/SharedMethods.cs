using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EctBlazorApp.Shared
{
    public static class SharedMethods
    {
        public static DateTime NewDateTimeFromString(string dateString)                                                 // Expects the string to be in yyyy-MM-dd format
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

        public static string GetFullNameFromFormattedString(string formattedString)                                     // Expects format: Full Name <email@email.com>
        {
            string fullName = formattedString.Split("<")[0].Trim();
            return fullName;
        }

        public static string GetEmailFromFormattedString(string formattedString)                                        // Expects format: Full Name <email@email.com>
        {
            string fullName = formattedString.Split("<")[1].Split(">")[0].Trim();
            return fullName;
        }

        public static bool IsStringInMemberFormat(string member)
        {
            try
            {
                return Regex.IsMatch(member, @"^([a-z A-Z]{1,} <.+>)$");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetDateRangeQueryString(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            string queryString = $"?fromDate={fromDate.ToString("yyyy-MM-dd")}&toDate={toDate.ToString("yyyy-MM-dd")}";
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
            int totalMinutes = timeSpan.Hours * 60 + timeSpan.Minutes;
            return totalMinutes;
        }

        public static int GetSecondsFromDateTimeRange(DateTime from, DateTime to)
        {
            TimeSpan timeSpan = to - from;
            return (int)timeSpan.TotalSeconds;
        }

        /***************************************************************************************
	    *    Usage: Used
	    *    Title: How to verify that strings are in valid email format
	    *    Author: adegeo et al. [Microsoft Docs]
	    *	 Date posted: 30 June 2020
	    *	 Type: Source code
	    *    Availability: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
	    *    Accessed on: 20 April 2021
	    *
	    ***************************************************************************************/
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));                     // Normalize the domain
                string DomainMapper(Match match)                                                                                                // Examines the domain part of the email and normalizes it.
                {
                    var idn = new IdnMapping();                                                                                                 // Use IdnMapping class to convert Unicode domain names.
                    string domainName = idn.GetAscii(match.Groups[2].Value);                                                                    // Pull out and process domain name (throws ArgumentException on invalid)

                    return match.Groups[1].Value + domainName;
                }

                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
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

        /***************************************************************************************
	    *    Usage: Used
	    *    Title: Compute SHA256 Hash In C#
	    *    Author: Mahesh Chand / @mcbeniwal [C# Corner]
	    *	 Date posted: 16 April 2020
	    *	 Type: Source code
	    *    Availability: https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/
	    *    Accessed on: 26 Feb 2021
	    *
	    ***************************************************************************************/
        public static string ComputeSha256Hash(string rawData)
        {
            using SHA256 sha256Hash = SHA256.Create();                                                                  // Create a SHA256   
            
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));                                     // ComputeHash - returns byte array  

            StringBuilder builder = new StringBuilder();                                                                // Convert byte array to a string
            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));
            
            return builder.ToString();
        }
    }
}
