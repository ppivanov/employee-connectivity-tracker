using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System;
using System.Linq;

namespace EctBlazorApp.ServerTests
{
    public static class MockObjects
    {
        public static GraphEventsResponse GetMockGraphEventResponseOneDayAfterLastLogin(EctUser contextUser, EventEmailAddress organiserDetails)
        {
            return new GraphEventsResponse
            {
                Value = new MicrosoftGraphEvent[]
                {
                    new MicrosoftGraphEvent
                    {
                        Subject = "Short project meeting",
                        Start = new DateTimeZone
                        {
                            DateTime = contextUser.LastSignIn.AddDays(3).ToString("o"),
                            TimeZone = "UTC"
                        },
                        End = new DateTimeZone
                        {
                            DateTime = contextUser.LastSignIn.AddDays(3).ToString("o"),
                            TimeZone = "UTC"
                        },
                        Organizer = new EventAttendee
                        {
                            emailAddress = organiserDetails
                        },
                        Attendees = new EventAttendee[]
                        {
                            new EventAttendee
                            {
                                emailAddress = organiserDetails
                            },
                            new EventAttendee
                            {
                                emailAddress = new EventEmailAddress
                                {
                                    Name = contextUser.FullName,
                                    Address = contextUser.Email
                                }
                            }
                        }
                    }
                }
            };
        }

        public static GraphUserResponse GetMockGraphUserResponse(string fullName)
        {
            return new GraphUserResponse()
            {
                DisplayName = fullName,
                Id = RandomString(),
                UserPrincipalName = GetEmailFromFullName(fullName)
            };
        }

        private static string RandomString()
        {
            const int length = 16;
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static EventEmailAddress GetTestUser(string fullName)
        {
            return new EventEmailAddress
            {
                Name = fullName,
                Address = GetEmailFromFullName(fullName)
            };
        }
        private static string GetEmailFromFullName(string fullName)
        {
            return $"{fullName.Split(" ")[0].ToLower()}@ect.ie";
        }
    }
}
