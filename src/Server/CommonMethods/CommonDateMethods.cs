using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EctBlazorApp.Server.CommonMethods
{
    public static class CommonDateMethods
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
    }
}
