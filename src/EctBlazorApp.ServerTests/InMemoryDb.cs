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

            EctUser alice = GetAlice();
            EctUser bob = GetHomer();

            List<EctUser> teamOneMembers = new List<EctUser> { alice, bob };
            EctTeam teamOne = GetTeamForParameters(alice, teamOneMembers);
            
            dbContext.Teams.Add(teamOne);

            dbContext.Administrators.AddRange(
                GetAdmins()
            );

            dbContext.SaveChanges();
            return dbContext;
        }

        private static List<EctAdmin> GetAdmins()
        {
            List<EctAdmin> admins = new List<EctAdmin>
            {
                new EctAdmin
                {
                    Id = 1,
                    User = new EctUser
                    {
                        Id = 2,
                        Email = "admin@ect.ie",
                        FullName = "Admin AdminS",
                        LastSignIn = new DateTime(2020, 11, 1), // Nov 1, 2020 12am
                    }
                }
            };
            return admins;
        }
        private static EctUser GetAlice()
        {
            EctUser alice =
                new EctUser
                {
                    Id = 1,
                    Email = "alice@ect.ie",
                    FullName = "Alice AliceS",
                    LastSignIn = new DateTime(2020, 11, 1), // Nov 1, 2020 12am
                    CalendarEvents = GetEventsForAlice(),
                    SentEmails = GetSentMailForAlice(),
                    ReceivedEmails = GetReceivedMailForAlice()
                }
            ;
            return alice;
        }

        private static EctUser GetHomer()
        {
            EctUser bob =
                new EctUser
                {
                    Id = 3,
                    Email = "homer@ect.ie",
                    FullName = "Homer HomerS",
                    LastSignIn = new DateTime(2020, 11, 1) // Nov 1, 2020 12am
                }
            ;
            return bob;
        }

        private static List<CalendarEvent> GetEventsForAlice()
        {
            List<CalendarEvent> events = new List<CalendarEvent>
            {
                new CalendarEvent
                {
                    Id = 1,
                    Start = new DateTime(2020, 11, 5, 9, 0, 0), // 5 Nov, 2020 9:00am
                    End = new DateTime(2020, 11, 5, 9, 30, 0),  // 5 Nov, 2020 9:30am
                    Subject = "Standup",
                    Organizer = "Roger RogerS <roger@ect.ie>",
                    EctUserId = 1
                },
                new CalendarEvent
                {
                    Id = 2,
                    Start = new DateTime(2020, 9, 25, 10, 0, 0), // 25 Sep, 2020 10:00am
                    End = new DateTime(2020, 9, 25, 11, 30, 0),  // 25 Sep, 2020 11:30am
                    Subject = "Refinement & Planning",
                    Organizer = "Bob BobS <bob@ect.ie>",
                    EctUserId = 1
                }
            };

            return events;
        }

        private static List<SentMail> GetSentMailForAlice()
        {
            List<SentMail> sentEmails = new List<SentMail> {
                new SentMail
                {
                    Id = 1,
                    SentAt = new DateTime(2020, 11, 5, 10, 0, 0),  // 5 Nov, 2020 10am
                    Subject = "Team outing proposal",
                    Recipients = new List<string> { "Roger RogerS <roger@ect.ie>" },
                    EctUserId = 1
                },
                new SentMail
                {
                    Id = 2,
                    SentAt = new DateTime(2020, 9, 25, 11, 40, 0),  // 25 Sep, 2020 11:40am
                    Subject = "Meeting notes",
                    Recipients = new List<string> { "Roger RogerS <roger@ect.ie>", "Bob BobS <bob@ect.ie>" },
                    EctUserId = 1
                }
            };

            return sentEmails;
        }

        private static List<ReceivedMail> GetReceivedMailForAlice()
        {
            List<ReceivedMail> receivedEmails = new List<ReceivedMail> {
                new ReceivedMail
                {
                    Id = 1,
                    ReceivedAt = new DateTime(2020, 11, 5, 10, 10, 0),  // 5 Nov, 2020 10:10am
                    Subject = "Team outing approval",
                    From = "Roger RogerS <roger@ect.ie>",
                    EctUserId = 1
                },
                new ReceivedMail
                {
                    Id = 2,
                    ReceivedAt = new DateTime(2020, 9, 25, 11, 50, 0),  // 25 Sep, 2020 11:50am
                    Subject = "Re: Meeting notes",
                    From = "Roger RogerS <roger@ect.ie>",
                    EctUserId = 1
                },
                new ReceivedMail
                {
                    Id = 3,
                    ReceivedAt = new DateTime(2020, 9, 25, 11, 53, 0),  // 25 Sep, 2020 11:53am
                    Subject = "Re: Meeting notes",
                    From = "Bob BobS <bob@ect.ie>",
                    EctUserId = 1
                }

            };

            return receivedEmails;
        }

        private static EctTeam GetTeamForParameters(EctUser lead, List<EctUser> members)
        {
            EctTeam team = new EctTeam
            {
                Id = 1,
                Name = "Team One",
                Leader = lead,
                Members = members
            };

            return team;
        }
    }
}
