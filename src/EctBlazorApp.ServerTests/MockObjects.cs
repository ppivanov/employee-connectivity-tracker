using EctBlazorApp.Shared;
using EctBlazorApp.Shared.GraphModels;
using System;
using System.Linq;

namespace EctBlazorApp.ServerTests
{
    public static class MockObjects
    {
        public static GraphEventsResponse GetMockGraphEventResponseOneDayAfterLastLogin(EctUser contextUser, MicrosoftGraphEmailAddress[] organiserDetails)
        {
            MicrosoftGraphEvent[] mockGraphEvents = new MicrosoftGraphEvent[organiserDetails.Length];
            for (int i = 0; i < mockGraphEvents.Length; i++)
            {
                var eventOrganiser = organiserDetails[i];
                mockGraphEvents[i] = new MicrosoftGraphEvent
                {
                    Subject = "Short project meeting",
                    Start = new DateTimeZone
                    {
                        DateTime = contextUser.LastSignIn.AddDays(1).ToString("o"),
                        TimeZone = "UTC"
                    },
                    End = new DateTimeZone
                    {
                        DateTime = contextUser.LastSignIn.AddDays(1).ToString("o"),
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
                Value = mockGraphEvents
            };
        }

        public static GraphReceivedMailResponse GetMockGraphReceivedMailResponseOneDayAfterLastLogin(EctUser contextUser, MicrosoftGraphEmailAddress[] senderDetails)
        {
            MicrosoftGraphReceivedMail[] mockGraphMail = new MicrosoftGraphReceivedMail[senderDetails.Length];
            for (int i = 0; i < mockGraphMail.Length; i++)
            {
                var mailSender = senderDetails[i];
                mockGraphMail[i] = new MicrosoftGraphReceivedMail
                {
                    ReceivedDateTime = contextUser.LastSignIn.AddDays(1),
                    Subject = "",
                    Sender = new MicrosoftGraphPerson
                    {
                        emailAddress = mailSender
                    }
                };
            }

            return new GraphReceivedMailResponse
            {
                Value = mockGraphMail
            };
        }

        public static GraphSentMailResponse GetMockGraphSentMailResponseOneDayAfterLastLogin(EctUser contextUser, MicrosoftGraphEmailAddress[] senderDetails)
        {
            MicrosoftGraphSentMail[] mockGraphMail = new MicrosoftGraphSentMail[senderDetails.Length];
            for (int i = 0; i < mockGraphMail.Length; i++)
            {
                var mailSender = senderDetails[i];
                mockGraphMail[i] = new MicrosoftGraphSentMail
                {
                    SentDateTime = contextUser.LastSignIn.AddDays(1),
                    Subject = "",
                    ToRecipients = new MicrosoftGraphPerson[]
                    {
                        new MicrosoftGraphPerson
                        {
                            emailAddress = mailSender
                        }
                    }
                };
            }

            return new GraphSentMailResponse
            {
                Value = mockGraphMail
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
