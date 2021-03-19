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
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void GetCommunicationPointsForUserId__Homer_1_Jan_2021__73Points()
        {
            var homer = GetUserFromInMemoryDb("Homer");
            DateTime fromDate = new(2021, 1, 1);
            DateTime toDate = new(2021, 1, 8);
            List<int> expectedResult = new(){ 24, 31, 6, 0, 8, 4};

            List<int> actualResult = _dbContext.GetCommunicationPointsForUser(homer, fromDate, toDate);

            CollectionAssert.AreEquivalent(expectedResult, actualResult);
        }

        [TestMethod]
        public void GetCommunicationPointsForUserId__Alice_23_Sep_2020__30Points()
        {
            var alice = GetUserFromInMemoryDb("Alice");
            DateTime fromDate = new(2020, 9, 23);
            DateTime toDate = new(2020, 9, 30);
            List<int> expectedResult = new() { 0, 0, 30, 0, 0, 0, 0 };

            List<int> actualResult = _dbContext.GetCommunicationPointsForUser(alice, fromDate, toDate);


            CollectionAssert.AreEquivalent(expectedResult, actualResult);
        }

        private EctUser GetUserFromInMemoryDb(string name)
        {
            var homer = _dbContext.Users.FirstOrDefault(u => u.FullName.Contains(name));
            return homer;
        }
    }
}
