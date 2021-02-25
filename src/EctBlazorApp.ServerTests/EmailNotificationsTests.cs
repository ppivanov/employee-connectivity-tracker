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
        private EctDbContext _dbContext;

        public EmailNotificationsTests()
        {
            _dbContext = InMemoryDb.InitInMemoryDbContext();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void GetCommunicationPointsForUserId()
        {
            var homer = GetHomerFromInMemoryDb();
            DateTime fromDate = new DateTime(2021, 1, 1);
            DateTime toDate = new DateTime(2021, 1, 8);
            int expectedResult = 73;

            int actualResult = _dbContext.GetCommunicationPointsForUserId(homer.Id, fromDate, toDate);

            Assert.AreEqual(expectedResult, actualResult);
        }

        private EctUser GetHomerFromInMemoryDb()
        {
            var homer = _dbContext.Users.First(u => u.Id == 3);
            return homer;
        }
    }
}
