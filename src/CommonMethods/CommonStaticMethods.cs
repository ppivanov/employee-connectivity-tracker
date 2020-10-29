namespace EctWebApp.CommonMethods
{
    using Microsoft.Graph;
    using System;
    using System.Collections.Generic;

    public static class CommonStaticMethods
    {
        public static List<QueryOption> GetCalendarEventViewOptions(DateTime fromDate, DateTime toDate)
        {
            List<QueryOption> viewOptions = new List<QueryOption>
            {
                new QueryOption("startDateTime", fromDate.ToString("o")),                                       // Graph API accepts only DateTime query parameters
                new QueryOption("endDateTime", toDate.ToString("o"))                                            // in the format 2008-10-01T17:04:32.0000000
            };
            return viewOptions;
        }

        public static string GetReceivedMailDateRangeQueryString(DateTime fromDate, DateTime toDate)
        {
            string filterString = $"ReceivedDateTime ge {fromDate.Date:yyyy-MM-dd} and ReceivedDateTime le {toDate:o}";     // Queries with DateTime arguments passed in as date are seen as 12:00am on that date.
            return filterString;
        }
        public static string GetSentMailDateRangeQueryString(DateTime fromDate, DateTime toDate)
        {
            string filterString = $"SentDateTime ge {fromDate.Date:yyyy-MM-dd} and SentDateTime le {toDate:o}";
            return filterString;
        }

        public static DateTime GetUtcStartOfWeekInTimeZone(TimeZoneInfo timeZone)
        {
            DateTime today = DateTime.Today;
            int diff = System.DayOfWeek.Monday - today.DayOfWeek;                                               // Assumes Monday as first day of week
            DateTime unspecifiedStart = DateTime.SpecifyKind(today.AddDays(diff), DateTimeKind.Unspecified);    // create date as unspecified kind
            return TimeZoneInfo.ConvertTimeToUtc(unspecifiedStart, timeZone);                                   // convert to UTC
        }

        public static string FormatNameAndEmailAddressParms(string name, string emailAddress)
        {
            string formattedNameAndEmailAddress = $"{name} <{emailAddress}>";
            return formattedNameAndEmailAddress;
        }

        public static DateTime GetEndOfDayForParm(DateTime date)
        {
            DateTime endOfDay = date.AddDays(1).AddMinutes(-1);
            return endOfDay;
        }

        public static DateTime GetDefaultDateTime()
        {
            return DateTime.Today;
        }

        public static string FormatDateTimeToMinutePrecision(DateTime date)
        {
            string formattedDate = $"{date:dd MMMM yyyy HH:mm}";
            return formattedDate;
        }
    }
}
