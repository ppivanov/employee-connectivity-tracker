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

            //dbContext.CalendarEvents.AddRange(
            //    GetEvents()
            //);

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
                        LastSignIn = new DateTime(2020, 11, 1), // Nov 1, 2020
                        CalendarEvents = GetEvents()
                    }
            };
            return users;
        }

        private static List<CalendarEvent> GetEvents()
        {
            List<CalendarEvent> events = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    Id = 1,
                    Start = new DateTime(2020, 11, 5, 9, 0, 0), // 5 Nov, 2020 9:00
                    End = new DateTime(2020, 11, 5, 9, 30, 0),  // 5 Nov, 2020 9:30
                    Subject = "Standup",
                    Organizer = "Roger RogerS <roger@ect.ie>"
                }
            };

            return events;
        }
    }
}
