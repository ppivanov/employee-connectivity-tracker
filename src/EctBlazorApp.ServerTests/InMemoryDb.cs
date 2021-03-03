using EctBlazorApp.Server;
using EctBlazorApp.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static EctBlazorApp.Shared.SharedMethods;

namespace EctBlazorApp.ServerTests
{
    [ExcludeFromCodeCoverage]
    public static class InMemoryDb
    {
        public static EctDbContext InitInMemoryDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EctDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            EctDbContext dbContext = new(optionsBuilder.Options);

            EctUser alice = GetAlice();
            EctUser homer = GetHomer();
            EctTeam teamOne = GetTeamForParameters(alice, new List<EctUser> { alice, homer });

            dbContext.Teams.Add(teamOne);
            dbContext.Administrators.AddRange(
                GetAdmins()
            );
            dbContext.CommunicationPoints.AddRange(
                GetCommunicationPoints()
            );

            dbContext.SaveChanges();
            return dbContext;
        }

        private static List<EctAdmin> GetAdmins()
        {
            List<EctAdmin> admins = new()
            {
                new()
                {
                    Id = 1,
                    User = new()
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
            EctUser alice = new()
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
            EctUser homer = new()
            {
                Id = 3,
                Email = "homer@ect.ie",
                FullName = "Homer HomerS",
                LastSignIn = new DateTime(2020, 11, 1), // Nov 1, 2020 12am
                CalendarEvents = GetEventsForHomer(),
                SentEmails = GetSentMailForHomer(),
                ReceivedEmails = GetReceivedMailForHomer()
            };
            return homer;
        }

        private static List<CommunicationPoint> GetCommunicationPoints()
        {
            List<CommunicationPoint> communicationPoints = new()
            {
                new()
                {
                    Medium = "Email",
                    Points = 1
                },
                new()
                {
                    Medium = "Meetings",
                    Points = 3
                }
            };

            return communicationPoints;
        }

        private static List<CalendarEvent> GetEventsForAlice()
        {
            List<CalendarEvent> events = new()
            {
                new()
                {
                    Id = 1,
                    Start = new DateTime(2020, 11, 5, 9, 0, 0), // 5 Nov, 2020 9:00am
                    End = new DateTime(2020, 11, 5, 9, 30, 0),  // 5 Nov, 2020 9:30am
                    Subject = "Standup",
                    Organizer = "Roger RogerS <roger@ect.ie>",
                    EctUserId = 1
                },
                new()
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
            List<SentMail> sentEmails = new()
            {
                new()
                {
                    Id = 1,
                    SentAt = new DateTime(2020, 11, 5, 10, 0, 0),  // 5 Nov, 2020 10am
                    Subject = "Team outing proposal",
                    Recipients = new List<string> { "Roger RogerS <roger@ect.ie>" },
                    EctUserId = 1
                },
                new()
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
            List<ReceivedMail> receivedEmails = new()
            {
                new()
                {
                    Id = 1,
                    ReceivedAt = new DateTime(2020, 11, 5, 10, 10, 0),  // 5 Nov, 2020 10:10am
                    Subject = "Team outing approval",
                    From = "Roger RogerS <roger@ect.ie>",
                    EctUserId = 1
                },
                new()
                {
                    Id = 2,
                    ReceivedAt = new DateTime(2020, 9, 25, 11, 50, 0),  // 25 Sep, 2020 11:50am
                    Subject = "Re: Meeting notes",
                    From = "Roger RogerS <roger@ect.ie>",
                    EctUserId = 1
                },
                new()
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

        private static List<CalendarEvent> GetEventsForHomer() // Total 2 hrs 50 minutes == 170 minutes
        {
            List<CalendarEvent> events = new()
            {
                new()
                {
                    Id = 3,
                    Start = new DateTime(2021, 1, 1, 12, 00, 00),
                    End = new DateTime(2021, 1, 1, 13, 00, 00)           // 1 hr
                },
                new()
                {
                    Id = 4,
                    Start = new DateTime(2021, 1, 2, 12, 00, 00),
                    End = new DateTime(2021, 1, 2, 13, 30, 00)           // 1 hr, 30 min
                },

                new()
                {
                    Id = 5,
                    Start = new DateTime(2021, 1, 3, 14, 00, 00),
                    End = new DateTime(2021, 1, 3, 14, 20, 00)           // 20 min
                }
            };
            return events;
        }
        private static List<SentMail> GetSentMailForHomer()    // 10 emails - Jan 1st to 7th
        {
            List<SentMail> sentMail = new()
            {
                new()
                {
                    Id = 3,
                    SentAt = new DateTime(2021, 1, 1, 11, 05, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 4,
                    SentAt = new DateTime(2021, 1, 1, 12, 00, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 5,
                    SentAt = new DateTime(2021, 1, 1, 12, 32, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 6,
                    SentAt = new DateTime(2021, 1, 2, 11, 05, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 7,
                    SentAt = new DateTime(2021, 1, 2, 12, 00, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 8,
                    SentAt = new DateTime(2021, 1, 5, 12, 32, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 9,
                    SentAt = new DateTime(2021, 1, 5, 11, 05, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 10,
                    SentAt = new DateTime(2021, 1, 5, 12, 00, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 11,
                    SentAt = new DateTime(2021, 1, 5, 12, 32, 00),
                    Recipients = new List<string>()
                },
                new()
                {
                    Id = 12,
                    SentAt = new DateTime(2021, 1, 7, 11, 05, 00),
                    Recipients = new List<string>()
                }
            };
            return sentMail;
        }
        private static List<ReceivedMail> GetReceivedMailForHomer()    // 12 emails - Jan 1st to 7th
        {
            List<ReceivedMail> receivedMail = new()
            {
                new()
                {
                    Id = 4,
                    ReceivedAt = new DateTime(2021, 1, 1, 11, 05, 00)
                },
                new()
                {
                    Id = 5,
                    ReceivedAt = new DateTime(2021, 1, 1, 12, 00, 00)
                },
                new()
                {
                    Id = 6,
                    ReceivedAt = new DateTime(2021, 1, 1, 12, 32, 00)
                },
                new()
                {
                    Id = 7, ReceivedAt = new DateTime(2021, 1, 2, 11, 05, 00)
                },
                new()
                {
                    Id = 8, ReceivedAt = new DateTime(2021, 1, 2, 12, 00, 00)
                },
                new()
                {
                    Id = 9, ReceivedAt = new DateTime(2021, 1, 5, 12, 32, 00)
                },
                new()
                {
                    Id = 10, ReceivedAt = new DateTime(2021, 1, 5, 11, 05, 00)
                },
                new()
                {
                    Id = 11, ReceivedAt = new DateTime(2021, 1, 5, 12, 00, 00)
                },
                new()
                {
                    Id = 12, ReceivedAt = new DateTime(2021, 1, 5, 12, 32, 00)
                },
                new()
                {
                    Id = 13, ReceivedAt = new DateTime(2021, 1, 7, 11, 05, 00)
                },
                new()
                {
                    Id = 14, ReceivedAt = new DateTime(2021, 1, 7, 12, 00, 00)
                },
                new()
                {
                    Id = 15, ReceivedAt = new DateTime(2021, 1, 7, 12, 32, 00)
                },
            };
            return receivedMail;
        }

        private static EctTeam GetTeamForParameters(EctUser lead, List<EctUser> members)
        {
            EctTeam team = new()
            {
                Id = 1,
                Name = "Team One",
                Leader = lead,
                Members = members,
                PointsThreshold = 0,
                MarginForNotification = 0,
                AdditionalUsersToNotify = new() { FormatFullNameAndEmail(lead.FullName, lead.Email) }
            };

            return team;
        }
    }
}
