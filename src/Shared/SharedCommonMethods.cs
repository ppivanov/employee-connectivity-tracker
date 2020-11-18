using System;

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
    }
}
