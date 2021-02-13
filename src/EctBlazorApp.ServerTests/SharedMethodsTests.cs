using Microsoft.VisualStudio.TestTools.UnitTesting;
using EctBlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.ServerTests
{
    [TestClass]
    public class SharedMethodsTests
    {
        [DataTestMethod]
        [DataRow(2019, 1, 2)]
        [DataRow(2020, 12, 12)]
        [DataRow(2021, 10, 25)]
        public void NewDateTimeFromStringTest(int year, int month, int day)
        {
            string dateStringIn = $"{year}-{month}-{day}";
            DateTime expectedDateTime = new DateTime(year, month, day);

            DateTime actualDateTime = NewDateTimeFromString(dateStringIn);

            Assert.AreEqual(expectedDateTime.ToString(), actualDateTime.ToString());
        }

        [DataTestMethod]
        [DataRow("Pavel P", "x00149863@outlook.com")]
        [DataRow("X", "x")]
        public void FormatFullNameAndEmailTest(string fullName, string email)
        {
            string expectedResult = $"{fullName} <{email}>";

            string actualResult = FormatFullNameAndEmail(fullName, email);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow("Pavel P <x00149863@outlook.com>", "Pavel P")]
        [DataRow("X <x>", "X")]
        public void GetFullNameFromFormattedStringTest(string formattedString, string expectedResult)
        {
            string actualResult = GetFullNameFromFormattedString(formattedString);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(2019, 1, 2, 2019, 2, 1)]
        [DataRow(2020, 10, 20, 2020, 12, 23)]
        [DataRow(2020, 9, 4, 2021, 9, 5)]
        public void GetDateRangeQueryStringTest(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay)
        {
            DateTimeOffset fromDate = new DateTimeOffset(new DateTime(fromYear, fromMonth, fromDay));
            DateTimeOffset toDate = new DateTimeOffset(new DateTime(toYear, toMonth, toDay));
            string expectedResult = $"?fromDate={fromDate.ToString("yyyy-MM-dd")}&toDate={toDate.ToString("yyyy-MM-dd")}";

            string actualResult = GetDateRangeQueryString(fromDate, toDate);

            Assert.AreEqual(expectedResult, actualResult);
        }
    
        [DataTestMethod]
        [DataRow(600, 10)]
        [DataRow(180, 3)]
        [DataRow(660, 11)]
        public void GetMinutesFromSecondsTest(int seconds, int expectedResult)
        {
            int actualResult = GetMinutesFromSeconds(seconds);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void SplitDateRangeToChunksTest()
        {
            const int daysToAdd = 10;
            DateTime startDateTime = new DateTime(2020, 12, 12);
            DateTime endDateTime = startDateTime.AddDays(daysToAdd);
            DateTimeOffset fromDate = new DateTimeOffset(startDateTime);
            DateTimeOffset toDate = new DateTimeOffset(endDateTime);

            List<DateTime> result = SplitDateRangeToChunks(fromDate, toDate);

            for (int i = 0; i <= daysToAdd; i++)
            {
                int expectedDay = startDateTime.Day;
                int expectedMonth = startDateTime.Month;
                int expectedYear = startDateTime.Year;

                int actualDay = result[0].Day;
                int actualMonth = result[0].Month;
                int actualYear = result[0].Year;

                Assert.AreEqual(expectedDay, actualDay);
                Assert.AreEqual(expectedMonth, actualMonth);
                Assert.AreEqual(expectedYear, actualYear);

                startDateTime.AddDays(1);
            }
        }
    }
}