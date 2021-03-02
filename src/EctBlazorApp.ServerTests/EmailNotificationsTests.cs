using EctBlazorApp.Server;
using EctBlazorApp.Server.Extensions;
using EctBlazorApp.Shared.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EctBlazorApp.ServerTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class EmailNotificationsTests : IDisposable
    {
        private readonly EctDbContext _dbContext;

        public EmailNotificationsTests()
        {
            _dbContext = InMemoryDb.InitInMemoryDbContext();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void GetCommunicationPointsForUserId__Homer_1_Jan_2021__73Points()
        {
            var homer = GetUserFromInMemoryDb("Homer");
            DateTime fromDate = new DateTime(2021, 1, 1);
            DateTime toDate = new DateTime(2021, 1, 8);
            int expectedResult = 73;

            int actualResult = _dbContext.GetCommunicationPointsForUserId(homer.Id, fromDate, toDate);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void GetCommunicationPointsForUserId__Alice_23_Sep_2020__30Points()
        {
            var alice = GetUserFromInMemoryDb("Alice");
            DateTime fromDate = new DateTime(2020, 9, 23);
            DateTime toDate = new DateTime(2020, 9, 30);
            int expectedResult = 30;

            int actualResult = _dbContext.GetCommunicationPointsForUserId(alice.Id, fromDate, toDate);

            Assert.AreEqual(expectedResult, actualResult);
        }

        private EctUser GetUserFromInMemoryDb(string name)
        {
            var homer = _dbContext.Users.First(u => u.FullName.Contains(name));
            return homer;
        }
    }
}
