using EctBlazorApp.Server;
using EctBlazorApp.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EctBlazorApp.ServerTests
{
    public static class InMemoryDb
    {
        public static EctDbContext InitInMemoryDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EctDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            EctDbContext dbContext = new EctDbContext(optionsBuilder.Options);
            dbContext.Users.AddRange(
                GetUsers()
            );
            
            dbContext.SaveChanges();
            return dbContext;
        }
        private static List<EctUser> GetUsers()
        {
            List<EctUser> users = new List<EctUser>
        {
            new EctUser
                {
                    Id = 1,
                    Email = "alice@ect.ie",
                    FullName = "Alice AliceS",
                    LastSignIn = new DateTime(2020, 11, 1) // Nov 1, 2020
                }
        };

            return users;
        }
    }
}
