using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EctBlazorApp.ServerTests
{
    public static class MockObjects
    {
        public static GraphEventsResponse GetMockGraphEventResponseOneDayAfterLastLogin(EctUser contextUser, MicrosoftGraphEmailAddress[] organiserDetails)
        {
            MicrosoftGraphEvent[] graphEvents = new MicrosoftGraphEvent[organiserDetails.Length];
            for (int i = 0; i < graphEvents.Length; i++)
            {
                var eventOrganiser = organiserDetails[i];
                graphEvents[i] = new MicrosoftGraphEvent
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
                    Organizer = new MicrosoftGraphPerson
                    {
                        emailAddress = eventOrganiser
                    },
                    Attendees = new MicrosoftGraphPerson[]
                    {
                        new MicrosoftGraphPerson
                        {
                            emailAddress = eventOrganiser
                        },
                        new MicrosoftGraphPerson
                        {
                            emailAddress = new MicrosoftGraphEmailAddress
                            {
                                Name = contextUser.FullName,
                                Address = contextUser.Email
                            }
                        }
                    }
                };
            }

            return new GraphEventsResponse
            {
                Value = graphEvents
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

        public static MicrosoftGraphEmailAddress GetTestUser(string fullName)
        {
            return new MicrosoftGraphEmailAddress
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
